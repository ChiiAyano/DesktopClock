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

    public async Task<SettingModel> LoadSettingsAsync()
    {
        if (!File.Exists(SettingsFilePath))
        {
            return new SettingModel();
        }

        await using var stream = new FileStream(SettingsFilePath, FileMode.Open, FileAccess.Read);
        return await JsonSerializer.DeserializeAsync<SettingModel>(stream) ?? new SettingModel();
    }

    public async Task SaveSettingsAsync(SettingModel settings)
    {
        await using var stream = new FileStream(SettingsFilePath, FileMode.Create, FileAccess.Write);
        await JsonSerializer.SerializeAsync(stream, settings);
    }
}
