using System.Diagnostics;

namespace DesktopClock;
public class General
{
    public string ApplicationRootPath { get; } = AppContext.BaseDirectory;
    public string ExecutingAssemblyPath { get; } = Process.GetCurrentProcess().MainModule?.FileName ?? string.Empty;
    public string ApplicationName { get; } = "DesktopClock";
}
