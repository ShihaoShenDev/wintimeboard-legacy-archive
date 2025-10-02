using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WinTimeBoard.Dialogs
{
    /// <summary>
    /// 时间格式设置对话框
    /// </summary>
    public sealed partial class TimeFormatDialog : ContentDialog
    {
        public TimeFormatDialog()
        {
            this.InitializeComponent();
            InitializeDialog();
        }

        /// <summary>
        /// 当前时间格式
        /// </summary>
        public string TimeFormat { get; set; } = "HH:mm:ss";

        /// <summary>
        /// 初始化对话框
        /// </summary>
        private void InitializeDialog()
        {
            SetCurrentFormat(TimeFormat);
            UpdatePreview();
        }

        /// <summary>
        /// 设置当前格式
        /// </summary>
        /// <param name="format">时间格式</param>
        private void SetCurrentFormat(string format)
        {
            // 查找匹配的预设格式
            foreach (RadioButton radio in TimeFormatRadioButtons.Items)
            {
                if (radio.Tag?.ToString() == format)
                {
                    radio.IsChecked = true;
                    return;
                }
            }

            // 如果没有匹配的预设格式，选择自定义格式
            var customRadio = TimeFormatRadioButtons.Items[4] as RadioButton;
            if (customRadio != null)
            {
                customRadio.IsChecked = true;
                CustomFormatPanel.Visibility = Visibility.Visible;
                CustomFormatTextBox.Text = format;
            }
        }

        /// <summary>
        /// 获取当前选择的时间格式
        /// </summary>
        /// <returns>时间格式字符串</returns>
        public string GetSelectedTimeFormat()
        {
            if (TimeFormatRadioButtons.SelectedItem is RadioButton selectedRadio)
            {
                if (selectedRadio.Tag?.ToString() == "Custom")
                {
                    return CustomFormatTextBox.Text ?? "HH:mm:ss";
                }
                return selectedRadio.Tag?.ToString() ?? "HH:mm:ss";
            }
            return "HH:mm:ss";
        }

        /// <summary>
        /// 更新预览
        /// </summary>
        private void UpdatePreview()
        {
            try
            {
                var format = GetSelectedTimeFormat();
                var preview = DateTime.Now.ToString(format);
                PreviewText.Text = preview;
            }
            catch
            {
                PreviewText.Text = "格式错误";
            }
        }

        /// <summary>
        /// 时间格式单选按钮选择变化事件
        /// </summary>
        private void TimeFormatRadioButtons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TimeFormatRadioButtons.SelectedItem is RadioButton selectedRadio)
            {
                // 显示或隐藏自定义格式面板
                if (selectedRadio.Tag?.ToString() == "Custom")
                {
                    CustomFormatPanel.Visibility = Visibility.Visible;
                }
                else
                {
                    CustomFormatPanel.Visibility = Visibility.Collapsed;
                }

                UpdatePreview();
            }
        }

        /// <summary>
        /// 自定义格式文本变化事件
        /// </summary>
        private void CustomFormatTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdatePreview();
        }
    }
}