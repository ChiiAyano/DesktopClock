using Microsoft.Win32;
using System.Reflection;

namespace DesktopClock;
public class StartupRegister
{
    private const string RegistryPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private readonly General _general;

    public StartupRegister(General general)
    {
        _general = general;
    }

    public void Register()
    {
        // アプリケーション名を取得
        var appName = _general.ApplicationName;
        // スタートアップレジストリキーを取得
        using var key = Registry.CurrentUser.CreateSubKey(RegistryPath);

        // アプリケーションのパスを取得
        var appPath = Assembly.GetExecutingAssembly().Location;
        // スタートアップに登録
        key.SetValue(appName, appPath);
    }

    public void Unregister()
    {
        // アプリケーション名を取得
        var appName = _general.ApplicationName;
        // スタートアップレジストリキーを取得
        using var key = Registry.CurrentUser.CreateSubKey(RegistryPath);
        // スタートアップから削除
        key.DeleteValue(appName, false);
    }

    public bool IsRegistered()
    {
        // アプリケーション名を取得
        var appName = _general.ApplicationName;
        // スタートアップレジストリキーを取得
        using var key = Registry.CurrentUser.OpenSubKey(RegistryPath);
        // アプリケーションが登録されているか確認
        return key?.GetValue(appName) != null;
    }
}
