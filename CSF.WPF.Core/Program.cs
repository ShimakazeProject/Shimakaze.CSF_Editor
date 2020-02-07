using System;
using System.Collections.Generic;
using System.Text;

namespace CSF.WPF.Core
{
    internal class Program
    {

        private const string GUID = "{FD25BA98-74C3-4793-976A-17A09CC7A2B6}";

        /// <summary>
        /// 程序的入口 主方法(主函数)
        /// </summary>
        /// <param name="args">命令行参数 不包括程序本身</param>
        [STAThread] // 单线程模型 UI必备
        public static void Main(string[] args)
        {
#if DEBUG
            foreach (var arg in args)
            {
                Console.WriteLine(arg);
            }
#endif

            var app = new App();
            app.InitializeComponent();
            app.StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
            app.Run();
        }
    }
}
