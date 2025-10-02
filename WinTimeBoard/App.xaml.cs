using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinTimeBoard.Services;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinTimeBoard
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private Window? _window;

        /// <summary>
        /// 静态主窗口引用，供服务使用
        /// </summary>
        public static Window? MainWindow { get; private set; }

        /// <summary>
        /// 设置服务
        /// </summary>
        public static SettingsService SettingsService { get; private set; } = new();

        /// <summary>
        /// 主题服务
        /// </summary>
        public static ThemeService ThemeService { get; private set; } = new();

        /// <summary>
        /// 背景材质服务
        /// </summary>
        public static MaterialService MaterialService { get; private set; } = new();

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            _window = new MainWindow();
            MainWindow = _window;
            
            // 加载设置
            await SettingsService.LoadSettingsAsync();
            
            // 应用初始设置
            ThemeService.ApplyTheme(SettingsService.CurrentSettings.Theme.Mode);
            MaterialService.ApplyMaterial(_window, SettingsService.CurrentSettings.Material.Material);
            
            _window.Activate();
        }
    }
}
