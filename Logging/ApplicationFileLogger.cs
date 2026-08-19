using System.Collections;
using System.Diagnostics;
using System.Text;

namespace HondaSensorChecker.Logging
{
    public enum ApplicationLogLevel
    {
        Debug,
        Information,
        Warning,
        Error,
        Critical
    }

    public static class ApplicationFileLogger
    {
        private static readonly object Sync = new();

        private static string _logDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "HondaSensorChecker",
            "Logs");

        public static string LogDirectory => _logDirectory;

        public static void Initialize(string logDirectory, int retentionDays = 90)
        {
            if (!string.IsNullOrWhiteSpace(logDirectory))
                _logDirectory = logDirectory;

            try
            {
                Directory.CreateDirectory(_logDirectory);
                DeleteExpiredFiles(retentionDays);
                Information("Application.LoggingInitialized",
                    "File logging initialized.",
                    new Dictionary<string, object?>
                    {
                        ["LogDirectory"] = _logDirectory,
                        ["RetentionDays"] = retentionDays
                    });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to initialize application file logging: {ex}");
            }
        }

        public static void DebugLog(string eventName, string message,
            IReadOnlyDictionary<string, object?>? context = null) =>
            Write(ApplicationLogLevel.Debug, eventName, message, null, context);

        public static void Information(string eventName, string message,
            IReadOnlyDictionary<string, object?>? context = null) =>
            Write(ApplicationLogLevel.Information, eventName, message, null, context);

        public static void Warning(string eventName, string message,
            IReadOnlyDictionary<string, object?>? context = null) =>
            Write(ApplicationLogLevel.Warning, eventName, message, null, context);

        public static void Error(string eventName, string message, Exception? exception = null,
            IReadOnlyDictionary<string, object?>? context = null) =>
            Write(ApplicationLogLevel.Error, eventName, message, exception, context);

        public static void Critical(string eventName, string message, Exception? exception = null,
            IReadOnlyDictionary<string, object?>? context = null) =>
            Write(ApplicationLogLevel.Critical, eventName, message, exception, context);

        public static void Write(
            ApplicationLogLevel level,
            string eventName,
            string message,
            Exception? exception = null,
            IReadOnlyDictionary<string, object?>? context = null)
        {
            try
            {
                var entry = FormatEntry(level, eventName, message, exception, context);
                var path = Path.Combine(_logDirectory,
                    $"HondaSensorChecker-{DateTime.Now:yyyy-MM-dd}.log");

                lock (Sync)
                {
                    Directory.CreateDirectory(_logDirectory);
                    File.AppendAllText(path, entry, new UTF8Encoding(false));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to write application log: {ex}");
            }
        }

        private static void DeleteExpiredFiles(int retentionDays)
        {
            if (retentionDays <= 0 || !Directory.Exists(_logDirectory))
                return;

            var limit = DateTime.Now.AddDays(-retentionDays);
            var paths = Directory
                .EnumerateFiles(_logDirectory, "HondaSensorChecker-*.log", SearchOption.TopDirectoryOnly)
                .Concat(Directory.EnumerateFiles(
                    _logDirectory, "HondaSensorChecker-*.jsonl", SearchOption.TopDirectoryOnly));

            foreach (var path in paths)
            {
                try
                {
                    if (File.GetLastWriteTime(path) < limit)
                        File.Delete(path);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Unable to remove expired log '{path}': {ex.Message}");
                }
            }
        }

        private static string FormatEntry(
            ApplicationLogLevel level,
            string eventName,
            string message,
            Exception? exception,
            IReadOnlyDictionary<string, object?>? context)
        {
            var builder = new StringBuilder();
            builder.Append('[').Append(DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss.fff zzz"))
                .Append("] [").Append(level.ToString().ToUpperInvariant()).Append("] ")
                .AppendLine(string.IsNullOrWhiteSpace(eventName) ? "Application.Event" : eventName);
            builder.Append("Message : ").AppendLine(NormalizeSingleLine(message));
            builder.Append("System  : Machine=").Append(Environment.MachineName)
                .Append(" | WindowsUser=").Append(Environment.UserName)
                .Append(" | ProcessId=").Append(Environment.ProcessId)
                .Append(" | ThreadId=").AppendLine(Environment.CurrentManagedThreadId.ToString());

            if (context is { Count: > 0 })
            {
                builder.AppendLine("Context :");
                foreach (var item in context.OrderBy(item => item.Key, StringComparer.OrdinalIgnoreCase))
                    builder.Append("  ").Append(item.Key).Append(" = ")
                        .AppendLine(FormatValue(item.Value));
            }

            if (exception != null)
            {
                builder.AppendLine("Exception:");
                builder.Append("  Type    = ")
                    .AppendLine(exception.GetType().FullName ?? exception.GetType().Name);
                builder.Append("  Message = ").AppendLine(NormalizeSingleLine(exception.Message));
                builder.AppendLine("  Details =");
                foreach (var line in exception.ToString().Replace("\r\n", "\n").Split('\n'))
                    builder.Append("    ").AppendLine(line);
            }

            builder.AppendLine(new string('-', 100));
            return builder.ToString();
        }

        private static string FormatValue(object? value)
        {
            if (value == null)
                return "(null)";

            if (value is string text)
                return NormalizeSingleLine(text);

            if (value is IEnumerable items)
            {
                var values = new List<string>();
                foreach (var item in items)
                    values.Add(NormalizeSingleLine(item?.ToString() ?? "(null)"));
                return $"[{string.Join(", ", values)}]";
            }

            return NormalizeSingleLine(value.ToString() ?? string.Empty);
        }

        private static string NormalizeSingleLine(string? value) =>
            (value ?? string.Empty)
            .Replace("\r\n", " | ")
            .Replace('\r', ' ')
            .Replace('\n', ' ');
    }
}
