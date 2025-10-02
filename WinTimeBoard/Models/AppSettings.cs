using Microsoft.UI.Text;
using Windows.UI.Text;

namespace WinTimeBoard.Models
{
    /// <summary>
    /// 应用程序设置模型
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// 主题设置
        /// </summary>
        public ThemeSettings Theme { get; set; } = new();

        /// <summary>
        /// 背景材质设置
        /// </summary>
        public MaterialSettings Material { get; set; } = new();

        /// <summary>
        /// 时间显示设置
        /// </summary>
        public TimeDisplaySettings TimeDisplay { get; set; } = new();
    }

    /// <summary>
    /// 主题设置
    /// </summary>
    public class ThemeSettings
    {
        /// <summary>
        /// 主题模式
        /// </summary>
        public ThemeMode Mode { get; set; } = ThemeMode.FollowSystem;
    }

    /// <summary>
    /// 背景材质设置
    /// </summary>
    public class MaterialSettings
    {
        /// <summary>
        /// 背景材质类型
        /// </summary>
        public BackgroundMaterial Material { get; set; } = BackgroundMaterial.Mica;
    }

    /// <summary>
    /// 时间显示设置
    /// </summary>
    public class TimeDisplaySettings
    {
        /// <summary>
        /// 时间格式
        /// </summary>
        public string TimeFormat { get; set; } = "HH:mm:ss";

        /// <summary>
        /// 日期格式
        /// </summary>
        public string DateFormat { get; set; } = "yyyy年MM月dd日 dddd";

        /// <summary>
        /// 时间字体设置
        /// </summary>
        public FontSettings TimeFont { get; set; } = new()
        {
            FontFamily = "Microsoft YaHei UI",
            FontSize = 96,
            FontWeight = FontWeights.Bold
        };

        /// <summary>
        /// 日期字体设置
        /// </summary>
        public FontSettings DateFont { get; set; } = new()
        {
            FontFamily = "Microsoft YaHei UI",
            FontSize = 36,
            FontWeight = FontWeights.Normal
        };
    }

    /// <summary>
    /// 字体设置
    /// </summary>
    public class FontSettings
    {
        /// <summary>
        /// 字体族
        /// </summary>
        public string FontFamily { get; set; } = "Microsoft YaHei UI";

        /// <summary>
        /// 字体大小
        /// </summary>
        public double FontSize { get; set; } = 16;

        /// <summary>
        /// 字体粗细
        /// </summary>
        public Windows.UI.Text.FontWeight FontWeight { get; set; } = FontWeights.Normal;
    }
}