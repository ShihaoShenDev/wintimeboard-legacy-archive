using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using WinTimeBoard.Models;

namespace WinTimeBoard.Services
{
    /// <summary>
    /// 背景材质服务，处理背景效果切换
    /// </summary>
    public class MaterialService
    {
        /// <summary>
        /// 应用背景材质
        /// </summary>
        /// <param name="window">要应用的窗口</param>
        /// <param name="material">背景材质类型</param>
        public void ApplyMaterial(Window window, BackgroundMaterial material)
        {
            if (window == null) return;

            // 清除现有的系统背景
            window.SystemBackdrop = null;

            // 应用新的背景材质
            window.SystemBackdrop = material switch
            {
                BackgroundMaterial.Mica => new Microsoft.UI.Xaml.Media.MicaBackdrop { Kind = Microsoft.UI.Composition.SystemBackdrops.MicaKind.Base },
                BackgroundMaterial.MicaAlt => new Microsoft.UI.Xaml.Media.MicaBackdrop { Kind = Microsoft.UI.Composition.SystemBackdrops.MicaKind.BaseAlt },
                BackgroundMaterial.Acrylic => new Microsoft.UI.Xaml.Media.DesktopAcrylicBackdrop(),
                BackgroundMaterial.None => null,
                _ => new Microsoft.UI.Xaml.Media.MicaBackdrop { Kind = Microsoft.UI.Composition.SystemBackdrops.MicaKind.Base }
            };
        }

        /// <summary>
        /// 检查背景材质是否支持
        /// </summary>
        /// <param name="material">背景材质类型</param>
        /// <returns>是否支持</returns>
        public bool IsMaterialSupported(BackgroundMaterial material)
        {
            return material switch
            {
                BackgroundMaterial.Mica => MicaController.IsSupported(),
                BackgroundMaterial.MicaAlt => MicaController.IsSupported(),
                BackgroundMaterial.Acrylic => DesktopAcrylicController.IsSupported(),
                BackgroundMaterial.None => true,
                _ => false
            };
        }

        /// <summary>
        /// 获取背景材质的显示名称
        /// </summary>
        /// <param name="material">背景材质</param>
        /// <returns>显示名称</returns>
        public string GetMaterialDisplayName(BackgroundMaterial material)
        {
            return material switch
            {
                BackgroundMaterial.None => "无背景",
                BackgroundMaterial.Mica => "Mica",
                BackgroundMaterial.MicaAlt => "MicaAlt",
                BackgroundMaterial.Acrylic => "Acrylic",
                _ => "未知"
            };
        }

        /// <summary>
        /// 从字符串解析背景材质
        /// </summary>
        /// <param name="materialString">背景材质字符串</param>
        /// <returns>背景材质</returns>
        public BackgroundMaterial ParseMaterial(string materialString)
        {
            return materialString switch
            {
                "None" => BackgroundMaterial.None,
                "Mica" => BackgroundMaterial.Mica,
                "MicaAlt" => BackgroundMaterial.MicaAlt,
                "Acrylic" => BackgroundMaterial.Acrylic,
                _ => BackgroundMaterial.Mica
            };
        }

        /// <summary>
        /// 获取推荐的背景材质（基于系统支持情况）
        /// </summary>
        /// <returns>推荐的背景材质</returns>
        public BackgroundMaterial GetRecommendedMaterial()
        {
            if (IsMaterialSupported(BackgroundMaterial.Mica))
                return BackgroundMaterial.Mica;
            
            if (IsMaterialSupported(BackgroundMaterial.Acrylic))
                return BackgroundMaterial.Acrylic;
            
            return BackgroundMaterial.None;
        }
    }
}