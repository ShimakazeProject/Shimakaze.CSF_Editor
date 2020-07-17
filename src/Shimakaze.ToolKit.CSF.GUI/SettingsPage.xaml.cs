using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Shimakaze.ToolKit.CSF.GUI
{
    /// <summary>
    /// SettingsPage.xaml 的交互逻辑
    /// </summary>
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
        }
        // 浅色
        private void LightTheme_RadioButton_Checked(object sender, RoutedEventArgs e) => Theme.ThemeManager.AppBaseTheme = Theme.ThemeManager.BaseTheme.Light;
        // 深色
        private void DarkTheme_RadioButton_Checked(object sender, RoutedEventArgs e) => Theme.ThemeManager.AppBaseTheme = Theme.ThemeManager.BaseTheme.Dark;
        // 随系统
        private void SystemTheme_RadioButton_Checked(object sender, RoutedEventArgs e) => Theme.ThemeManager.AppBaseTheme = Theme.ThemeManager.SystemBaseTheme;

        private void FollowSystemAccent_CheckBox_Check(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).IsChecked ?? false)
            {
                AccentColorPicker.IsEnabled = false;
                Theme.ThemeManager.AppAccentColor = AccentColorPicker.Color = Theme.ThemeManager.SystemAccentColor;
            }
            else
            {
                AccentColorPicker.IsEnabled = true;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Theme.ThemeManager.AppAccentColor = AccentColorPicker.Color = (Color)ColorConverter.ConvertFromString((sender as TextBox).Text);
                System.Diagnostics.Debug.WriteLine(AccentColorPicker.Color);
            }
            catch (FormatException)
            {
            }
        }
    }
}
