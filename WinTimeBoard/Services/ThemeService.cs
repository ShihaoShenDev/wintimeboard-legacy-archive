using System;
using Microsoft.UI.Xaml;
using WinTimeBoard.Models;

namespace WinTimeBoard.Services
{
    /// <summary>
    /// 主题服务，处理主题切换逻辑
    /// </summary>
    public class ThemeService
    {
        /// <summary>
        /// 应用主题设置
        /// </summary>
        /// <param name="themeMode">主题模式</param>
        public void ApplyTheme(ThemeMode themeMode)
        {
            var requestedTheme = themeMode switch
            {
                ThemeMode.Light => ElementTheme.Light,
                ThemeMode.Dark => ElementTheme.Dark,
                ThemeMode.FollowSystem => ElementTheme.Default,
                _ => ElementTheme.Default
            };

            // 获取主窗口并应用主题
            if (App.MainWindow?.Content is FrameworkElement rootElement)
            {
                rootElement.RequestedTheme = requestedTheme;
                
                // 特殊处理：当背景材质为"无"时，强制设置窗口级别的主题
                // 这确保主题设置优先于系统设置
                if (App.MainWindow.SystemBackdrop == null && themeMode != ThemeMode.FollowSystem)
                {
                    // 直接设置窗口的主题，而不是依赖内容元素
                    try
                    {
                        // 通过设置Application级别的主题来强制应用
                        var app = Application.Current;
                        if (app != null)
                        {
                            var appTheme = themeMode switch
                            {
                                ThemeMode.Light => ApplicationTheme.Light,
                                ThemeMode.Dark => ApplicationTheme.Dark,
                                _ => app.RequestedTheme
                            };
                            
                            // 只有当主题确实需要改变时才设置
                            if (themeMode != ThemeMode.FollowSystem && 
                                ((themeMode == ThemeMode.Light && app.RequestedTheme != ApplicationTheme.Light) ||
                                 (themeMode == ThemeMode.Dark && app.RequestedTheme != ApplicationTheme.Dark)))
                            {
                                // 通过Content元素强制主题应用
                                rootElement.RequestedTheme = requestedTheme;
                                
                                // 强制刷新
                                System.Diagnostics.Debug.WriteLine($"Force applied theme: {requestedTheme} for no-backdrop window");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Failed to force theme application: {ex.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// 获取当前系统主题
        /// </summary>
        /// <returns>当前系统主题</returns>
        public ElementTheme GetSystemTheme()
        {
            // 通过应用程序设置获取系统主题
            var app = Application.Current;
            return app.RequestedTheme == ApplicationTheme.Dark ? ElementTheme.Dark : ElementTheme.Light;
        }

        /// <summary>
        /// 获取主题模式的显示名称
        /// </summary>
        /// <param name="themeMode">主题模式</param>
        /// <returns>显示名称</returns>
        public string GetThemeModeDisplayName(ThemeMode themeMode)
        {
            return themeMode switch
            {
                ThemeMode.FollowSystem => "跟随系统",
                ThemeMode.Light => "浅色模式",
                ThemeMode.Dark => "深色模式",
                _ => "未知"
            };
        }

        /// <summary>
        /// 从字符串解析主题模式
        /// </summary>
        /// <param name="themeModeString">主题模式字符串</param>
        /// <returns>主题模式</returns>
        public ThemeMode ParseThemeMode(string themeModeString)
        {
            return themeModeString switch
            {
                "FollowSystem" => ThemeMode.FollowSystem,
                "Light" => ThemeMode.Light,
                "Dark" => ThemeMode.Dark,
                _ => ThemeMode.FollowSystem
            };
        }
    }
}