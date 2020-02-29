using CSF.Logmgr;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSF.WPF.Core
{
    internal class Program
    {

        internal const string GUID = "{FD25BA98-74C3-4793-976A-17A09CC7A2B6}";

        /// <summary>
        /// 程序的入口 主方法(主函数)
        /// </summary>
        /// <param name="args">命令行参数 不包括程序本身</param>
        [STAThread] // 单线程模型 UI必备
        public static void Main(string[] args)
        {
#if DEBUG
            var logRank = Rank.ALL;
#elif RELEASE
            var logRank = Rank.INFO;
#endif
            // Logger Initializing
            Logger.Init(AppContext.BaseDirectory, "Debug.log", logRank);
            Logger.Info("Operating system: " + Environment.OSVersion.VersionString);

            var app = new App();
            app.DispatcherUnhandledException += App_DispatcherUnhandledException;
            app.InitializeComponent();
            //app.StartupUri = new Uri("View/Window1.xaml", UriKind.Relative);
            app.StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
            app.Run();
        }

        private static void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.Fatal("遭遇未经处理的异常:{0}", e.Exception);
            System.IO.File.Copy(AppContext.BaseDirectory + "Debug.log", AppContext.BaseDirectory + string.Format("Error{0}.log", DateTime.Now.ToString("MM.dd_HH_mmss.fff")));
            var msgboxret = System.Windows.MessageBox.Show(string.Format("程序遭遇未经处理的异常{0}请将{2}目录下的错误日志提交给开发者{0}{3}{0}您是否需要跳转到问题反馈?{0}异常信息:{1}"
                , Environment.NewLine, e.Exception.Message, AppContext.BaseDirectory, "https://github.com/frg2089/CNC-Strings-File-Editor/issues"),
                "Fatal Exception", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Error);
            if (msgboxret == System.Windows.MessageBoxResult.Yes)
            {

            }
            throw new NotImplementedException();
        }
    }
}
