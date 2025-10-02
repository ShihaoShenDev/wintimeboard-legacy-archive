using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using WinTimeBoard.Models;

namespace WinTimeBoard.Services
{
    /// <summary>
    /// 设置服务，负责配置的保存、加载和应用
    /// </summary>
    public class SettingsService
    {
        private const string SettingsFileName = "appsettings.json";
        private readonly JsonSerializerOptions _jsonOptions;
        private AppSettings _currentSettings;

        public SettingsService()
        {
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            _currentSettings = new AppSettings();
        }

        /// <summary>
        /// 当前设置
        /// </summary>
        public AppSettings CurrentSettings => _currentSettings;

        /// <summary>
        /// 设置发生变化事件
        /// </summary>
        public event EventHandler<AppSettings>? SettingsChanged;

        /// <summary>
        /// 加载设置
        /// </summary>
        /// <returns></returns>
        public async Task LoadSettingsAsync()
        {
            try
            {
                var localFolder = ApplicationData.Current.LocalFolder;
                var settingsFile = await localFolder.TryGetItemAsync(SettingsFileName) as StorageFile;
                
                if (settingsFile != null)
                {
                    var json = await FileIO.ReadTextAsync(settingsFile);
                    var settings = JsonSerializer.Deserialize<AppSettings>(json, _jsonOptions);
                    if (settings != null)
                    {
                        _currentSettings = settings;
                    }
                }
            }
            catch (Exception ex)
            {
                // 加载失败时使用默认设置
                System.Diagnostics.Debug.WriteLine($"Failed to load settings: {ex.Message}");
                _currentSettings = new AppSettings();
            }
        }

        /// <summary>
        /// 保存设置
        /// </summary>
        /// <returns></returns>
        public async Task SaveSettingsAsync()
        {
            try
            {
                var localFolder = ApplicationData.Current.LocalFolder;
                var settingsFile = await localFolder.CreateFileAsync(SettingsFileName, CreationCollisionOption.ReplaceExisting);
                
                var json = JsonSerializer.Serialize(_currentSettings, _jsonOptions);
                await FileIO.WriteTextAsync(settingsFile, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to save settings: {ex.Message}");
            }
        }

        /// <summary>
        /// 更新设置并保存
        /// </summary>
        /// <param name="settings">新的设置</param>
        /// <returns></returns>
        public async Task UpdateSettingsAsync(AppSettings settings)
        {
            _currentSettings = settings;
            await SaveSettingsAsync();
            SettingsChanged?.Invoke(this, _currentSettings);
        }

        /// <summary>
        /// 更新主题设置
        /// </summary>
        /// <param name="themeMode">主题模式</param>
        /// <returns></returns>
        public async Task UpdateThemeAsync(ThemeMode themeMode)
        {
            _currentSettings.Theme.Mode = themeMode;
            await SaveSettingsAsync();
            SettingsChanged?.Invoke(this, _currentSettings);
        }

        /// <summary>
        /// 更新背景材质设置
        /// </summary>
        /// <param name="material">背景材质</param>
        /// <returns></returns>
        public async Task UpdateMaterialAsync(BackgroundMaterial material)
        {
            _currentSettings.Material.Material = material;
            await SaveSettingsAsync();
            SettingsChanged?.Invoke(this, _currentSettings);
        }

        /// <summary>
        /// 更新时间显示设置
        /// </summary>
        /// <param name="timeDisplaySettings">时间显示设置</param>
        /// <returns></returns>
        public async Task UpdateTimeDisplayAsync(TimeDisplaySettings timeDisplaySettings)
        {
            _currentSettings.TimeDisplay = timeDisplaySettings;
            await SaveSettingsAsync();
            SettingsChanged?.Invoke(this, _currentSettings);
        }

        /// <summary>
        /// 重置为默认设置
        /// </summary>
        /// <returns></returns>
        public async Task ResetToDefaultAsync()
        {
            _currentSettings = new AppSettings();
            await SaveSettingsAsync();
            SettingsChanged?.Invoke(this, _currentSettings);
        }
    }
}