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
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Windowing;
using WinRT.Interop;
using Microsoft.UI.Dispatching;
using System.Runtime.InteropServices;

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

        public MainWindow()
        {
            InitializeComponent();

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
        }

        private void Timer_Tick(object? sender, object e)
        {
            UpdateTimeAndDate();
        }

        private void UpdateTimeAndDate()
        {
            try
            {
                TimeText.Text = DateTime.Now.ToString("HH:mm:ss");
                DateText.Text = DateTime.Now.ToString("yyyy-MM-dd dddd");
            }
            catch
            {
                // ignore UI update errors
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
    }
}
