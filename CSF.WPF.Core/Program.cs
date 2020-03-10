using CSF.Logmgr;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSF.WPF.Core
{
    internal class Program
    {
        public const string GUID = "{FD25BA98-74C3-4793-976A-17A09CC7A2B6}";

        public const ushort BUILD = 20038;

        public static App App { get; private set; }= new App();
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
            Logger.Info("General: ");
            Logger.Info("Operating system: {0}", Environment.OSVersion.VersionString);
            Logger.Info("Is x86_64 OS: {0}", Environment.Is64BitOperatingSystem);
            Logger.Info("Is x86_64 Process: {0}", Environment.Is64BitProcess);
            Logger.Info("======================");
            Logger.Trace("程序初始化");
            App.InitializeComponent();
            //MahApps.Metro.ThemeManager.ChangeTheme(app, "Green", "Dark");
            App.DispatcherUnhandledException += App_DispatcherUnhandledException;
            App.Exit += App_Exit;
            App.StartupUri = new Uri("View/MainWindow.xaml", UriKind.Relative);
            Logger.Trace("程序运行");
            App.Run();
            Logger.Trace("程序正常结束");
        }

        private static void App_Exit(object sender, System.Windows.ExitEventArgs e)
        {
            Logger.Info("Exit :{0}", Environment.ExitCode);
        }

        private static void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            var LogFile = string.Format("{0}Error{1}.log", AppContext.BaseDirectory, DateTime.Now.ToString("_MM.dd_HH.mm.ss.fff"));
            const string url = "https://github.com/frg2089/CNC-Strings-File-Editor/issues";
            Logger.Fatal("Dispatcher Unhandled Exception:{0}", e.Exception);
            System.IO.File.Copy(AppContext.BaseDirectory + "Debug.log", LogFile);
            var msgboxret = System.Windows.MessageBox.Show(
                string.Format("程序遭遇未经处理的异常{0}您可以将错误日志{2}提交到{0}{3}{0}您是否需要跳转到问题反馈?{0}异常信息:{1}",
                Environment.NewLine,
                e.Exception.Message,
                LogFile,
                url),
                "Fatal Error", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Error);
            if (msgboxret == System.Windows.MessageBoxResult.Yes)
            {
                System.Diagnostics.Process.Start(url);
            }
            throw e.Exception;

        }
    }
}
