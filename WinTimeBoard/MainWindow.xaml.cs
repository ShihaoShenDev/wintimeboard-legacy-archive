using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Windowing;
using WinRT.Interop;
using Microsoft.UI.Dispatching;
using System.Runtime.InteropServices;
using System.Reflection;
using WinTimeBoard.Services;
using WinTimeBoard.Models;
using WinTimeBoard.Dialogs;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinTimeBoard
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private DispatcherTimer? _timer;
        private bool _isInitialized = false;

        public MainWindow()
        {
            InitializeComponent();

            // Populate About information (find TextBlocks inside flyout's content)
            try
            {
                if (MenuButton?.Flyout is Flyout flyout && flyout.Content is FrameworkElement root)
                {
                    var nameTb = root.FindName("AppNameText") as TextBlock;
                    var verTb = root.FindName("AppVersionText") as TextBlock;
                    if (nameTb != null) nameTb.Text = "WinTimeBoard";
                    if (verTb != null) verTb.Text = GetApplicationVersion();
                }
            }
            catch
            {
                // ignore
            }

            // 初始化设置菜单
            InitializeSettingsMenu();

            // Make the window fullscreen
            try
            {
                var hwnd = WindowNative.GetWindowHandle(this);
                var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
                var appWindow = AppWindow.GetFromWindowId(windowId);
                if (appWindow != null)
                {
                    appWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
                }
            }
            catch
            {
                // If anything fails, silently ignore to avoid crashing at startup
            }

            // Initialize and start timer to update time and date
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            UpdateTimeAndDate();
            _timer.Start();

            // 监听设置变更
            App.SettingsService.SettingsChanged += OnSettingsChanged;
            
            _isInitialized = true;
        }

        private string GetApplicationVersion()
        {
            try
            {
                var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
                var name = assembly.GetName();
                var version = name.Version;
                if (version != null)
                {
                    return $"Ver {version.ToString()}";
                }
            }
            catch
            {
                // ignore
            }
            return "Ver 1.0.0";
        }

        private void Timer_Tick(object? sender, object e)
        {
            UpdateTimeAndDate();
        }

        private void UpdateTimeAndDate()
        {
            try
            {
                var settings = App.SettingsService.CurrentSettings;
                TimeText.Text = DateTime.Now.ToString(settings.TimeDisplay.TimeFormat);
                DateText.Text = DateTime.Now.ToString(settings.TimeDisplay.DateFormat);
                
                // 应用字体设置
                ApplyFontSettings();
            }
            catch
            {
                // 使用默认格式
                TimeText.Text = DateTime.Now.ToString("HH:mm:ss");
                DateText.Text = DateTime.Now.ToString("yyyy-MM-dd dddd");
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Close the window (will exit the app if this is the main window)
                this.Close();
            }
            catch
            {
                // fallback: post WM_CLOSE to the window
                try
                {
                    var hwnd = WindowNative.GetWindowHandle(this);
                    PostMessage(hwnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                }
                catch
                {
                    // ignore
                }
            }
        }

        private void HideButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var hwnd = WindowNative.GetWindowHandle(this);
                ShowWindow(hwnd, SW_MINIMIZE);
            }
            catch
            {
                // ignore
            }
        }

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        private const int SW_MINIMIZE = 6;
        private const uint WM_CLOSE = 0x0010;
        
        /// <summary>
        /// 初始化设置菜单
        /// </summary>
        private void InitializeSettingsMenu()
        {
            try
            {
                var settings = App.SettingsService.CurrentSettings;
                
                // 设置主题选项（避免触发SelectionChanged事件）
                _isInitialized = false;
                foreach (RadioButton radio in ThemeRadioButtons.Items)
                {
                    if (radio.Tag?.ToString() == settings.Theme.Mode.ToString())
                    {
                        radio.IsChecked = true;
                        break;
                    }
                }
                
                // 设置背景材质选项（避免触发SelectionChanged事件）
                foreach (RadioButton radio in MaterialRadioButtons.Items)
                {
                    if (radio.Tag?.ToString() == settings.Material.Material.ToString())
                    {
                        radio.IsChecked = true;
                        break;
                    }
                }
                _isInitialized = true;
            }
            catch (Exception ex)
            {
                // 使用默认设置，忽略初始化错误
                System.Diagnostics.Debug.WriteLine($"InitializeSettingsMenu failed: {ex.Message}");
                _isInitialized = true;
            }
        }
        
        /// <summary>
        /// 应用字体设置
        /// </summary>
        private void ApplyFontSettings()
        {
            try
            {
                var settings = App.SettingsService.CurrentSettings;
                
                // 应用时间字体
                TimeText.FontFamily = new FontFamily(settings.TimeDisplay.TimeFont.FontFamily);
                TimeText.FontSize = settings.TimeDisplay.TimeFont.FontSize;
                TimeText.FontWeight = new Windows.UI.Text.FontWeight { Weight = settings.TimeDisplay.TimeFont.FontWeight.Weight };
                
                // 应用日期字体
                DateText.FontFamily = new FontFamily(settings.TimeDisplay.DateFont.FontFamily);
                DateText.FontSize = settings.TimeDisplay.DateFont.FontSize;
                DateText.FontWeight = new Windows.UI.Text.FontWeight { Weight = settings.TimeDisplay.DateFont.FontWeight.Weight };
            }
            catch
            {
                // 忽略字体设置错误
            }
        }
        
        /// <summary>
        /// 设置变更事件处理
        /// </summary>
        private void OnSettingsChanged(object? sender, AppSettings settings)
        {
            try
            {
                // 使用批处理更新机制，避免闪烁
                DispatcherQueue.TryEnqueue(() =>
                {
                    // 暂时禁用初始化标志，避免递归调用
                    var wasInitialized = _isInitialized;
                    _isInitialized = false;
                    
                    try
                    {
                        // 先更新菜单状态
                        UpdateMenuStates(settings);
                        
                        // 再应用主题和材质
                        App.ThemeService.ApplyTheme(settings.Theme.Mode);
                        App.MaterialService.ApplyMaterial(this, settings.Material.Material);
                        
                        // 最后应用字体设置
                        ApplyFontSettings();
                    }
                    finally
                    {
                        // 恢复初始化标志
                        _isInitialized = wasInitialized;
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"OnSettingsChanged failed: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 更新菜单状态（不触发事件）
        /// </summary>
        private void UpdateMenuStates(AppSettings settings)
        {
            try
            {
                // 更新主题选项
                foreach (RadioButton radio in ThemeRadioButtons.Items)
                {
                    radio.IsChecked = radio.Tag?.ToString() == settings.Theme.Mode.ToString();
                }
                
                // 更新背景材质选项
                foreach (RadioButton radio in MaterialRadioButtons.Items)
                {
                    radio.IsChecked = radio.Tag?.ToString() == settings.Material.Material.ToString();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UpdateMenuStates failed: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 主题选择变更事件
        /// </summary>
        private async void ThemeRadioButtons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized || ThemeRadioButtons.SelectedItem is not RadioButton selectedRadio)
                return;
                
            try
            {
                var themeModeString = selectedRadio.Tag?.ToString() ?? "FollowSystem";
                var themeMode = App.ThemeService.ParseThemeMode(themeModeString);
                
                // 使用平滑过渡更新，避免闪烁
                await ApplySmoothThemeChange(themeMode);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ThemeRadioButtons_SelectionChanged failed: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 背景材质选择变更事件
        /// </summary>
        private async void MaterialRadioButtons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized || MaterialRadioButtons.SelectedItem is not RadioButton selectedRadio)
                return;
                
            try
            {
                var materialString = selectedRadio.Tag?.ToString() ?? "Mica";
                var material = App.MaterialService.ParseMaterial(materialString);
                
                // 使用平滑过渡更新，避免闪烁
                await ApplySmoothMaterialChange(material);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"MaterialRadioButtons_SelectionChanged failed: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 平滑主题切换
        /// </summary>
        private async Task ApplySmoothThemeChange(ThemeMode themeMode)
        {
            try
            {
                // 批处理更新，避免中间状态
                await Task.Delay(10); // 等待UI线程稳定
                await App.SettingsService.UpdateThemeAsync(themeMode);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ApplySmoothThemeChange failed: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 平滑材质切换
        /// </summary>
        private async Task ApplySmoothMaterialChange(BackgroundMaterial material)
        {
            try
            {
                // 批处理更新，避免中间状态
                await Task.Delay(10); // 等待UI线程稳定
                await App.SettingsService.UpdateMaterialAsync(material);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ApplySmoothMaterialChange failed: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 时间格式设置按钮点击事件
        /// </summary>
        private async void TimeFormatButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new TimeFormatDialog
                {
                    XamlRoot = this.Content.XamlRoot,
                    TimeFormat = App.SettingsService.CurrentSettings.TimeDisplay.TimeFormat
                };
                
                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    var newTimeFormat = dialog.GetSelectedTimeFormat();
                    var currentSettings = App.SettingsService.CurrentSettings;
                    currentSettings.TimeDisplay.TimeFormat = newTimeFormat;
                    await App.SettingsService.UpdateTimeDisplayAsync(currentSettings.TimeDisplay);
                }
                
                // 关闭菜单
                MenuFlyout.Hide();
            }
            catch
            {
                // 忽略错误
            }
        }
        
        /// <summary>
        /// 日期格式设置按钮点击事件
        /// </summary>
        private async void DateFormatButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new DateFormatDialog
                {
                    XamlRoot = this.Content.XamlRoot,
                    DateFormat = App.SettingsService.CurrentSettings.TimeDisplay.DateFormat
                };
                
                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    var newDateFormat = dialog.GetSelectedDateFormat();
                    var currentSettings = App.SettingsService.CurrentSettings;
                    currentSettings.TimeDisplay.DateFormat = newDateFormat;
                    await App.SettingsService.UpdateTimeDisplayAsync(currentSettings.TimeDisplay);
                }
                
                // 关闭菜单
                MenuFlyout.Hide();
            }
            catch
            {
                // 忽略错误
            }
        }
        
        /// <summary>
        /// 字体设置按钮点击事件
        /// </summary>
        private async void FontSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new FontSettingsDialog
                {
                    XamlRoot = this.Content.XamlRoot,
                    TimeFont = App.SettingsService.CurrentSettings.TimeDisplay.TimeFont,
                    DateFont = App.SettingsService.CurrentSettings.TimeDisplay.DateFont
                };
                
                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    var (timeFont, dateFont) = dialog.GetFontSettings();
                    var currentSettings = App.SettingsService.CurrentSettings;
                    currentSettings.TimeDisplay.TimeFont = timeFont;
                    currentSettings.TimeDisplay.DateFont = dateFont;
                    await App.SettingsService.UpdateTimeDisplayAsync(currentSettings.TimeDisplay);
                }
                
                // 关闭菜单
                MenuFlyout.Hide();
            }
            catch
            {
                // 忽略错误
            }
        }
    }
}
