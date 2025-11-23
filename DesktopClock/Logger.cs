using System.IO;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Text;

namespace DesktopClock;

internal class Logger(General general) : IDisposable
{
    private string LogDirectoryPath => Path.Combine(general.ApplicationRootPath, "logs");
    private string LogFilePath => Path.Combine(LogDirectoryPath, $"log_{DateTimeOffset.Now:yyyyMMdd}.txt");

    private readonly SemaphoreSlim _loggerSemaphore = new(1, 1);
    private readonly SemaphoreSlim _collectorSemaphore = new(1, 1);

    public async Task LogAsync(string message, [CallerMemberName] string callerName = "")
    {
        await _loggerSemaphore.WaitAsync();

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
        finally
        {
            _loggerSemaphore.Release();
        }
    }

    public async Task CollectAsync()
    {
        await _collectorSemaphore.WaitAsync();

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

            try
            {
                using var zipArchive = ZipFile.Open(logFilePath, ZipArchiveMode.Create);
                foreach (var logFile in logFiles)
                {
                    var entryName = Path.GetFileName(logFile);
                    zipArchive.CreateEntryFromFile(logFile, entryName);
                }
            }
            catch (Exception ex)
            {
                await LogAsync($"ログの圧縮に失敗しました: {ex.GetType()} - {ex.Message}");

                // 圧縮に失敗した場合は途中でできたZIPファイルを削除
                if (File.Exists(logFilePath))
                {
                    File.Delete(logFilePath);
                }

                return;
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
        finally
        {
            _collectorSemaphore.Release();
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _loggerSemaphore.Dispose();
            _collectorSemaphore.Dispose();
        }
    }

    public void Dispose()
    {
        Dispose(true);
    }
}
