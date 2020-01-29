﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.AvalonDock.Layout;

namespace CSF.WPF.Framework
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow 
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ViewModel.MainWindowVM(this);
        }

        private void NewTab_MenuItemClick(object sender, RoutedEventArgs e)
        {
            try
            {
                LayoutDocument anchorable = new LayoutDocument();
                anchorable.Title = "New Document";
                anchorable.Content = new Controls.Document();
                var child = layoutRoot.RootPanel.Children;
                bool success = false;
                for (int i = 0; i < child.Count; i++)
                {
                    if (child[i].GetType() == typeof(LayoutDocumentPane))
                    {
                        (child[i] as LayoutDocumentPane).Children.Add(anchorable);
                        success = true;
                        break;
                    }
                }
                if (!success)
                {
                    layoutRoot.RootPanel.Children.Add(new LayoutDocumentPane(anchorable));
                    success = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "[MainWindow][NewTab_MenuItemClick]");
            }
        }

        private async void Open_MenuItemClick(object sender, RoutedEventArgs e)
        {
            //Command="{Binding OpenCommand}"
            try
            {
                var ofd = new Microsoft.Win32.OpenFileDialog
                {
                    Filter = "红色警戒2字符串文件|*.csf",
                    FileName = "ra2md.csf",
                    Title = "打开文件"
                };
                if (ofd.ShowDialog() ?? false)
                {
                    var docVM = new ViewModel.DocumentVM();
                    LayoutDocument anchorable = new LayoutDocument
                    {
                        Title = ofd.SafeFileName,
                        Content = new Controls.Document(viewModel:docVM )
                    };
                    var child = layoutRoot.RootPanel.Children;
                    bool success = false;
                    for (int i = 0; i < child.Count; i++)
                    {
                        if (child[i].GetType() == typeof(LayoutDocumentPane))
                        {
                            (child[i] as LayoutDocumentPane).Children.Add(anchorable);
                            success = true;
                            break;
                        }
                    }
                    if (!success)
                    {
                        layoutRoot.RootPanel.Children.Add(new LayoutDocumentPane(anchorable));
                        success = true;
                    }
                    docVM.OpenCsf(ofd.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "[MainWindow][NewTab_MenuItemClick]");
            }
        }
    }
}
