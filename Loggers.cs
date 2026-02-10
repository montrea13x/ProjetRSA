using System;
using System.IO;
using System.Globalization;
using System.Text;
using System.Runtime.CompilerServices;

namespace ProjetRSA;

public static class Loggers
{
    private static readonly object _lock = new();
    private static readonly string _logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");
    private static readonly string _logFilePath = Path.Combine(_logDirectory, "app.log");

    public static void LogInfo(string message,
        [CallerFilePath] string filePath = "")
    {
        WriteLog("INFO", message, filePath);
    }

    public static void LogError(string message,
        [CallerFilePath] string filePath = "")
    {
        WriteLog("ERROR", message, filePath);
    }

    private static void WriteLog(string level, string message, string filePath)
    {
        string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff 'UTC'", CultureInfo.InvariantCulture);
        string relativePath = string.IsNullOrWhiteSpace(filePath)
            ? "unknown"
            : Path.GetRelativePath(Directory.GetCurrentDirectory(), filePath)
                .Replace(Path.DirectorySeparatorChar, '/');
        string line = $"[{timestamp}] [{level}] [{relativePath}] {message}{Environment.NewLine}";

        lock (_lock)
        {
            Directory.CreateDirectory(_logDirectory);
            File.AppendAllText(_logFilePath, line, Encoding.UTF8);
        }
    }
}