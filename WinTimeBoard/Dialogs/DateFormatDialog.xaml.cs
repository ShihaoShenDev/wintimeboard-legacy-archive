using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WinTimeBoard.Dialogs
{
    /// <summary>
    /// 日期格式设置对话框
    /// </summary>
    public sealed partial class DateFormatDialog : ContentDialog
    {
        public DateFormatDialog()
        {
            this.InitializeComponent();
            InitializeDialog();
        }

        /// <summary>
        /// 当前日期格式
        /// </summary>
        public string DateFormat { get; set; } = "yyyy年MM月dd日 dddd";

        /// <summary>
        /// 初始化对话框
        /// </summary>
        private void InitializeDialog()
        {
            SetCurrentFormat(DateFormat);
            UpdatePreview();
        }

        /// <summary>
        /// 设置当前格式
        /// </summary>
        /// <param name="format">日期格式</param>
        private void SetCurrentFormat(string format)
        {
            // 查找匹配的预设格式
            foreach (RadioButton radio in DateFormatRadioButtons.Items)
            {
                if (radio.Tag?.ToString() == format)
                {
                    radio.IsChecked = true;
                    return;
                }
            }

            // 如果没有匹配的预设格式，选择自定义格式
            var customRadio = DateFormatRadioButtons.Items[^1] as RadioButton; // 最后一个是自定义格式
            if (customRadio != null)
            {
                customRadio.IsChecked = true;
                CustomFormatPanel.Visibility = Visibility.Visible;
                CustomFormatTextBox.Text = format;
            }
        }

        /// <summary>
        /// 获取当前选择的日期格式
        /// </summary>
        /// <returns>日期格式字符串</returns>
        public string GetSelectedDateFormat()
        {
            if (DateFormatRadioButtons.SelectedItem is RadioButton selectedRadio)
            {
                if (selectedRadio.Tag?.ToString() == "Custom")
                {
                    return CustomFormatTextBox.Text ?? "yyyy年MM月dd日 dddd";
                }
                return selectedRadio.Tag?.ToString() ?? "yyyy年MM月dd日 dddd";
            }
            return "yyyy年MM月dd日 dddd";
        }

        /// <summary>
        /// 更新预览
        /// </summary>
        private void UpdatePreview()
        {
            try
            {
                var format = GetSelectedDateFormat();
                var preview = DateTime.Now.ToString(format);
                PreviewText.Text = preview;
            }
            catch
            {
                PreviewText.Text = "格式错误";
            }
        }

        /// <summary>
        /// 日期格式单选按钮选择变化事件
        /// </summary>
        private void DateFormatRadioButtons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DateFormatRadioButtons.SelectedItem is RadioButton selectedRadio)
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