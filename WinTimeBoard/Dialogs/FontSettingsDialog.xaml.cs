using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.UI.Text;
using WinTimeBoard.Models;

namespace WinTimeBoard.Dialogs
{
    /// <summary>
    /// 字体设置对话框
    /// </summary>
    public sealed partial class FontSettingsDialog : ContentDialog
    {
        private readonly List<string> _availableFontFamilies;
        private readonly List<FontWeightItem> _fontWeights;

        public FontSettingsDialog()
        {
            this.InitializeComponent();
            
            // 初始化字体族列表
            _availableFontFamilies = new List<string>
            {
                "Microsoft YaHei UI",
                "Microsoft YaHei",
                "Segoe UI",
                "Arial",
                "Calibri",
                "Consolas",
                "Times New Roman",
                "SimSun",
                "SimHei",
                "KaiTi",
                "FangSong"
            };

            // 初始化字体粗细列表
            _fontWeights = new List<FontWeightItem>
            {
                new FontWeightItem("Thin", FontWeights.Thin),
                new FontWeightItem("ExtraLight", FontWeights.ExtraLight),
                new FontWeightItem("Light", FontWeights.Light),
                new FontWeightItem("Normal", FontWeights.Normal),
                new FontWeightItem("Medium", FontWeights.Medium),
                new FontWeightItem("SemiBold", FontWeights.SemiBold),
                new FontWeightItem("Bold", FontWeights.Bold),
                new FontWeightItem("ExtraBold", FontWeights.ExtraBold),
                new FontWeightItem("Black", FontWeights.Black)
            };

            InitializeControls();
            UpdatePreview();
        }

        /// <summary>
        /// 时间字体设置
        /// </summary>
        public FontSettings TimeFont { get; set; } = new FontSettings
        {
            FontFamily = "Microsoft YaHei UI",
            FontSize = 96,
            FontWeight = FontWeights.Bold
        };

        /// <summary>
        /// 日期字体设置
        /// </summary>
        public FontSettings DateFont { get; set; } = new FontSettings
        {
            FontFamily = "Microsoft YaHei UI",
            FontSize = 36,
            FontWeight = FontWeights.Normal
        };

        /// <summary>
        /// 初始化控件
        /// </summary>
        private void InitializeControls()
        {
            // 设置字体族
            TimeFontFamilyComboBox.ItemsSource = _availableFontFamilies;
            DateFontFamilyComboBox.ItemsSource = _availableFontFamilies;

            // 设置字体粗细
            TimeFontWeightComboBox.ItemsSource = _fontWeights;
            TimeFontWeightComboBox.DisplayMemberPath = "DisplayName";
            TimeFontWeightComboBox.SelectedValuePath = "FontWeight";

            DateFontWeightComboBox.ItemsSource = _fontWeights;
            DateFontWeightComboBox.DisplayMemberPath = "DisplayName";
            DateFontWeightComboBox.SelectedValuePath = "FontWeight";

            // 设置当前值
            LoadCurrentSettings();

            // 绑定事件
            TimeFontFamilyComboBox.SelectionChanged += OnFontSettingChanged;
            TimeFontWeightComboBox.SelectionChanged += OnFontSettingChanged;
            DateFontFamilyComboBox.SelectionChanged += OnFontSettingChanged;
            DateFontWeightComboBox.SelectionChanged += OnFontSettingChanged;
        }

        /// <summary>
        /// 加载当前设置
        /// </summary>
        private void LoadCurrentSettings()
        {
            // 时间字体设置
            TimeFontFamilyComboBox.SelectedItem = TimeFont.FontFamily;
            TimeFontSizeSlider.Value = TimeFont.FontSize;
            TimeFontSizeText.Text = TimeFont.FontSize.ToString("F0");
            
            var timeWeightItem = _fontWeights.FirstOrDefault(w => w.FontWeight.Weight == TimeFont.FontWeight.Weight);
            TimeFontWeightComboBox.SelectedItem = timeWeightItem ?? _fontWeights.First(w => w.FontWeight.Weight == FontWeights.Bold.Weight);

            // 日期字体设置
            DateFontFamilyComboBox.SelectedItem = DateFont.FontFamily;
            DateFontSizeSlider.Value = DateFont.FontSize;
            DateFontSizeText.Text = DateFont.FontSize.ToString("F0");
            
            var dateWeightItem = _fontWeights.FirstOrDefault(w => w.FontWeight.Weight == DateFont.FontWeight.Weight);
            DateFontWeightComboBox.SelectedItem = dateWeightItem ?? _fontWeights.First(w => w.FontWeight.Weight == FontWeights.Normal.Weight);
        }

        /// <summary>
        /// 更新预览
        /// </summary>
        private void UpdatePreview()
        {
            try
            {
                // 更新时间预览
                PreviewTimeText.FontFamily = new FontFamily(TimeFontFamilyComboBox.SelectedItem?.ToString() ?? "Microsoft YaHei UI");
                PreviewTimeText.FontSize = TimeFontSizeSlider.Value * 0.5; // 缩放预览大小
                if (TimeFontWeightComboBox.SelectedItem is FontWeightItem timeWeightItem)
                {
                    PreviewTimeText.FontWeight = timeWeightItem.FontWeight;
                }

                // 更新日期预览
                PreviewDateText.FontFamily = new FontFamily(DateFontFamilyComboBox.SelectedItem?.ToString() ?? "Microsoft YaHei UI");
                PreviewDateText.FontSize = DateFontSizeSlider.Value * 0.5; // 缩放预览大小
                if (DateFontWeightComboBox.SelectedItem is FontWeightItem dateWeightItem)
                {
                    PreviewDateText.FontWeight = dateWeightItem.FontWeight;
                }
            }
            catch
            {
                // 忽略预览更新错误
            }
        }

        /// <summary>
        /// 获取当前字体设置
        /// </summary>
        /// <returns></returns>
        public (FontSettings TimeFont, FontSettings DateFont) GetFontSettings()
        {
            var timeFont = new FontSettings
            {
                FontFamily = TimeFontFamilyComboBox.SelectedItem?.ToString() ?? "Microsoft YaHei UI",
                FontSize = TimeFontSizeSlider.Value,
                FontWeight = (TimeFontWeightComboBox.SelectedItem as FontWeightItem)?.FontWeight ?? FontWeights.Bold
            };

            var dateFont = new FontSettings
            {
                FontFamily = DateFontFamilyComboBox.SelectedItem?.ToString() ?? "Microsoft YaHei UI",
                FontSize = DateFontSizeSlider.Value,
                FontWeight = (DateFontWeightComboBox.SelectedItem as FontWeightItem)?.FontWeight ?? FontWeights.Normal
            };

            return (timeFont, dateFont);
        }

        private void TimeFontSizeSlider_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            if (TimeFontSizeText != null)
            {
                TimeFontSizeText.Text = e.NewValue.ToString("F0");
                UpdatePreview();
            }
        }

        private void DateFontSizeSlider_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            if (DateFontSizeText != null)
            {
                DateFontSizeText.Text = e.NewValue.ToString("F0");
                UpdatePreview();
            }
        }

        private void OnFontSettingChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdatePreview();
        }
    }

    /// <summary>
    /// 字体粗细项
    /// </summary>
    public class FontWeightItem
    {
        public string DisplayName { get; set; }
        public Windows.UI.Text.FontWeight FontWeight { get; set; }

        public FontWeightItem(string displayName, Windows.UI.Text.FontWeight fontWeight)
        {
            DisplayName = displayName;
            FontWeight = fontWeight;
        }
    }
}