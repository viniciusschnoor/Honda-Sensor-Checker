using HondaSensorChecker.Data;
using HondaSensorChecker.Data.Context;
using HondaSensorChecker.Data.UnitOfWork;
using HondaSensorChecker.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HondaSensorChecker
{
    internal static class Program
    {
        public static IServiceProvider ServiceProvider { get; private set; }
        public static string DbPath { get; private set; }

        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            using var singleInstanceMutex = new Mutex(
                initiallyOwned: true,
                name: @"Global\HondaSensorChecker.SingleInstance",
                createdNew: out var isFirstInstance);

            if (!isFirstInstance)
            {
                MessageBox.Show(
                    "O Honda Sensor Checker já está aberto nesta estação.",
                    "Sensor Checker",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            var folder = Environment.SpecialFolder.CommonApplicationData;
            var basePath = Environment.GetFolderPath(folder);
            var applicationDataPath = Path.Combine(basePath, "HondaSensorChecker");
            ApplicationFileLogger.Initialize(Path.Combine(applicationDataPath, "Logs"));
            RegisterGlobalExceptionLogging();
            DbPath = ResolveDatabasePath(basePath, applicationDataPath);
            var dbExists = File.Exists(DbPath);
            ApplicationFileLogger.Information(
                "Application.Starting",
                "Honda Sensor Checker is starting.",
                new Dictionary<string, object?>
                {
                    ["DatabasePath"] = DbPath,
                    ["DatabaseAlreadyExists"] = dbExists,
                    ["ApplicationVersion"] = typeof(Program).Assembly.GetName().Version?.ToString()
                });

            try
            {
                var host = CreateHostBuilder().Build();
                ServiceProvider = host.Services;

                // 🔹 Runtime only
                // Migrations em runtime são OK,
                // EF Tools (dotnet ef) IGNORA isso por causa do DataContextFactory
                using (var scope = ServiceProvider.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                    db.Database.Migrate();

                    DatabaseInitializer.SeedIfMissing(db, dbExists);
                }

                ApplicationFileLogger.Information("Application.Started",
                    "Application initialization completed successfully.");
                Application.Run(ServiceProvider.GetRequiredService<HSCMainForm>());
                ApplicationFileLogger.Information("Application.Stopped",
                    "Application closed normally.");
            }
            catch (Exception ex)
            {
                ApplicationFileLogger.Critical("Application.StartupFailed",
                    "Application startup or main message loop failed.", ex,
                    new Dictionary<string, object?> { ["DatabasePath"] = DbPath });
                MessageBox.Show(
                    $"Falha crítica ao iniciar a aplicação.\n\n{ex.Message}\n\n" +
                    $"Consulte os logs em:\n{ApplicationFileLogger.LogDirectory}",
                    "Sensor Checker",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private static string ResolveDatabasePath(string commonApplicationDataPath, string applicationDataPath)
        {
            var databaseDirectory = Path.Combine(applicationDataPath, "Database");
            var databasePath = Path.Combine(databaseDirectory, "HondaSensorChecker.db");
            var legacyDatabasePath = Path.Combine(commonApplicationDataPath, "HondaSensorChecker.db");

            try
            {
                Directory.CreateDirectory(databaseDirectory);

                if (!File.Exists(databasePath) && File.Exists(legacyDatabasePath))
                {
                    try
                    {
                        MoveLegacyDatabaseFiles(legacyDatabasePath, databasePath);
                    }
                    catch (Exception ex)
                    {
                        ApplicationFileLogger.Critical(
                            "Database.LegacyDatabaseMoveFailed",
                            "Unable to move the legacy database to the new database directory.",
                            ex,
                            new Dictionary<string, object?>
                            {
                                ["LegacyDatabasePath"] = legacyDatabasePath,
                                ["DatabasePath"] = databasePath
                            });
                        throw;
                    }

                    ApplicationFileLogger.Information(
                        "Database.LegacyDatabaseMoved",
                        "The legacy database was moved to the new application database directory.",
                        new Dictionary<string, object?>
                        {
                            ["LegacyDatabasePath"] = legacyDatabasePath,
                            ["DatabasePath"] = databasePath,
                            ["DatabaseSizeBytes"] = new FileInfo(databasePath).Length
                        });
                }

                return databasePath;
            }
            catch (Exception ex)
            {
                ApplicationFileLogger.Critical(
                    "Database.PathPreparationFailed",
                    "Unable to prepare or migrate the application database path.",
                    ex,
                    new Dictionary<string, object?>
                    {
                        ["LegacyDatabasePath"] = legacyDatabasePath,
                        ["DatabasePath"] = databasePath
                    });
                throw;
            }
        }

        private static void MoveLegacyDatabaseFiles(
            string legacyDatabasePath,
            string databasePath)
        {
            var files = new List<(string Source, string Destination)>
            {
                (legacyDatabasePath, databasePath)
            };

            foreach (var suffix in new[] { "-wal", "-shm" })
            {
                var source = legacyDatabasePath + suffix;
                if (File.Exists(source))
                    files.Add((source, databasePath + suffix));
            }

            var movedFiles = new List<(string Source, string Destination)>();
            try
            {
                foreach (var file in files)
                {
                    File.Move(file.Source, file.Destination);
                    movedFiles.Add(file);
                }
            }
            catch
            {
                foreach (var file in movedFiles.AsEnumerable().Reverse())
                {
                    try
                    {
                        if (File.Exists(file.Destination) && !File.Exists(file.Source))
                            File.Move(file.Destination, file.Source);
                    }
                    catch (Exception rollbackException)
                    {
                        ApplicationFileLogger.Critical(
                            "Database.LegacyDatabaseMoveRollbackFailed",
                            "Unable to roll back a partially moved database file.",
                            rollbackException,
                            new Dictionary<string, object?>
                            {
                                ["Source"] = file.Source,
                                ["Destination"] = file.Destination
                            });
                    }
                }

                throw;
            }
        }

        private static void RegisterGlobalExceptionLogging()
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (_, args) =>
                ApplicationFileLogger.Critical("Application.UiThreadException",
                    "Unhandled exception on the Windows Forms UI thread.", args.Exception);

            AppDomain.CurrentDomain.UnhandledException += (_, args) =>
                ApplicationFileLogger.Critical("Application.UnhandledException",
                    $"Unhandled application exception. IsTerminating={args.IsTerminating}.",
                    args.ExceptionObject as Exception);

            TaskScheduler.UnobservedTaskException += (_, args) =>
            {
                ApplicationFileLogger.Error("Application.UnobservedTaskException",
                    "An unobserved task exception was raised.", args.Exception);
                args.SetObserved();
            };
        }

        static IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    var accSection = context.Configuration.GetSection("Acc");
                    var accSettings = new HondaSensorChecker.Configuration.AccSettings
                    {
                        IpAddress = accSection["IpAddress"] ?? string.Empty,
                        Port = int.TryParse(accSection["Port"], out var accPort) ? accPort : 0,
                        DllVersion = accSection["DllVersion"] ?? string.Empty,
                        ProductType = accSection["ProductType"] ?? string.Empty,
                        Station = accSection["Station"] ?? string.Empty
                    };
                    services.AddSingleton(accSettings);

                    // ============================
                    // DbContext
                    // ============================
                    services.AddDbContext<DataContext>(options =>
                    {
                        options.UseSqlite($"Data Source={DbPath}");
                    });

                    // ============================
                    // Unit of Work & Factories
                    // ============================
                    services.AddScoped<IUnitOfWork, UnitOfWorkRepository>();
                    services.AddScoped<IFinishedBoxFactory, FinishedBoxFactory>();

                    // ============================
                    // Forms
                    // ============================
                    services.AddTransient<HSCMainForm>();
                    services.AddTransient<Users>();
                    services.AddTransient<Products>();
                });
        }

    }
}
