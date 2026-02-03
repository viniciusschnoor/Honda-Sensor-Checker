using HondaSensorChecker.Data;
using HondaSensorChecker.Data.Context;
using HondaSensorChecker.Data.UnitOfWork;
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
            // Caminho do banco (C:\ProgramData\HondaSensorChecker.db)
            var folder = Environment.SpecialFolder.CommonApplicationData;
            var basePath = Environment.GetFolderPath(folder);
            DbPath = Path.Combine(basePath, "HondaSensorChecker.db");
            var dbExists = File.Exists(DbPath);

            ApplicationConfiguration.Initialize();

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

            Application.Run(ServiceProvider.GetRequiredService<HSCMainForm>());
        }

        static IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
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
