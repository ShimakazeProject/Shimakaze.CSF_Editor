using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace RA2CsfEditor
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public static ViewModel.MainVM VM { get; private set; }
        public static Shimakaze.LogMgr.Logger Logger { get; private set; }
        public App()
        {
            Logger = new Shimakaze.LogMgr.Logger("RA2StrEditor 开始初始化进程 日志等级:All");
            VM = null;
            StartWithCommand(Environment.GetCommandLineArgs());


            Task.Run(MsgBoxInit);

            DispatcherUnhandledException += (o, e) =>
            {
                Logger.Write(Shimakaze.LogMgr.LogRank.FATAL, "程序出现未经处理的异常!", e.Exception);
                throw e.Exception;
            };
        }
        private void MsgBoxInit()
        {
            MessageBoxX.MessageBoxXConfigurations.Add("InfoTheme", new Panuon.UI.Silver.Core.MessageBoxXConfigurations()
            {
                MessageBoxStyle = MessageBoxStyle.Modern,
                MessageBoxIcon = MessageBoxIcon.Info,
            });
            MessageBoxX.MessageBoxXConfigurations.Add("WarnTheme", new Panuon.UI.Silver.Core.MessageBoxXConfigurations()
            {
                MessageBoxStyle = MessageBoxStyle.Modern,
                MessageBoxIcon = MessageBoxIcon.Warning,
            });
            MessageBoxX.MessageBoxXConfigurations.Add("ErrorTheme", new Panuon.UI.Silver.Core.MessageBoxXConfigurations()
            {
                MessageBoxStyle = MessageBoxStyle.Modern,
                MessageBoxIcon = MessageBoxIcon.Error,
            });
            MessageBoxX.MessageBoxXConfigurations.Add("SuccessTheme", new Panuon.UI.Silver.Core.MessageBoxXConfigurations()
            {
                MessageBoxStyle = MessageBoxStyle.Modern,
                MessageBoxIcon = MessageBoxIcon.Success,
            });
            MessageBoxX.MessageBoxXConfigurations.Add("NoneTheme", new Panuon.UI.Silver.Core.MessageBoxXConfigurations()
            {
                MessageBoxStyle = MessageBoxStyle.Modern,
                MessageBoxIcon = MessageBoxIcon.None,
            });
        }
    }
}
