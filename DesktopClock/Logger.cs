using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Text;

namespace DesktopClock;

internal class Logger(General general)
{
    private string LogDirectoryPath => Path.Combine(general.ApplicationRootPath, "logs");
    private string LogFilePath => Path.Combine(LogDirectoryPath, $"log_{DateTimeOffset.Now:yyyyMMdd}.txt");

    public async Task LogAsync(string message, [CallerMemberName] string callerName = "")
    {
        try
        {
            if (!Directory.Exists(LogDirectoryPath))
            {
                Directory.CreateDirectory(LogDirectoryPath);
            }

            var logMessage = $"{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss} - {callerName} - {message}{Environment.NewLine}";
            await File.AppendAllTextAsync(LogFilePath, logMessage, Encoding.UTF8);
        }
        catch
        {
            // ログの書き込みに失敗してもアプリケーションの動作を妨げない
        }
    }

    public async Task CollectAsync()
    {
        // 前月のログはZIP圧縮して保存する
        try
        {
            if (!Directory.Exists(LogDirectoryPath))
            {
                return;
            }

            var now = DateTimeOffset.Now;
            var previousMonth = now.AddMonths(-1);
            var archiveFileName = $"log_{previousMonth:yyyyMM}.zip";
            var logFilePath = Path.Combine(LogDirectoryPath, archiveFileName);
            if (File.Exists(logFilePath))
            {
                // 既に圧縮済みの場合はスキップ
                return;
            }

            var logFiles = Directory.GetFiles(LogDirectoryPath, $"log_{previousMonth:yyyyMM}*.txt");
            if (logFiles.Length == 0)
            {
                return;
            }

            using var zipArchive = ZipFile.Open(logFilePath, ZipArchiveMode.Create);
            foreach (var logFile in logFiles)
            {
                var entryName = Path.GetFileName(logFile);
                zipArchive.CreateEntryFromFile(logFile, entryName);
            }

            // Delete log files only after the archive is successfully created and closed
            foreach (var logFile in logFiles)
            {
                File.Delete(logFile);
            }
        }
        catch
        {
            // ログの収集に失敗してもアプリケーションの動作を妨げない
        }
    }
}
