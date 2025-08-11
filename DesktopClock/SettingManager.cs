using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DesktopClock;
public class SettingManager(General general)
{
    public class SettingModel
    {
        [JsonPropertyName("window_top")]
        public double? WindowTop { get; set; } = null;

        [JsonPropertyName("window_left")]
        public double? WindowLeft { get; set; } = null;
    }

    private const string SettingsFileName = "settings.json";
    private string SettingsFilePath => Path.Combine(general.ApplicationRootPath, SettingsFileName);

    public SettingModel? Setting { get; private set; }

    public async Task<SettingModel> LoadSettingsAsync()
    {
        if (this.Setting is not null)
        {
            return this.Setting;
        }

        if (!File.Exists(SettingsFilePath))
        {
            return this.Setting = new SettingModel();
        }

        await using var stream = new FileStream(SettingsFilePath, FileMode.Open, FileAccess.Read);
        return this.Setting = await JsonSerializer.DeserializeAsync<SettingModel>(stream) ?? new SettingModel();
    }

    public async Task SaveSettingsAsync()
    {
        await using var stream = new FileStream(SettingsFilePath, FileMode.Create, FileAccess.Write);
        await JsonSerializer.SerializeAsync(stream, this.Setting);
    }
}
