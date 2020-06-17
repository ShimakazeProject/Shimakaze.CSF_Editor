namespace Shimakaze.LogMgr
{
    using System;
    using System.IO;
    public class Logger
    {
        private readonly string FILE_NAME;
        private readonly string LOGGER_NAME;
        private readonly string LOG_DIR;
        private readonly LogRank LOG_RANK;
        private readonly FileStream fileStream;
        private readonly StreamWriter streamWriter;

        public Logger(
            string loggerName,
            string fileName=null,
            string fileDir=null,
            LogRank rank = LogRank.ALL)
        {
            LOGGER_NAME = loggerName;
            FILE_NAME = fileName ?? $"{DateTime.Now.ToShortDateString().Replace('/', '_')}.log";
            LOG_DIR = fileDir ?? $"{AppDomain.CurrentDomain.BaseDirectory}Debug";
            if (!Directory.Exists(LOG_DIR))
                Directory.CreateDirectory(LOG_DIR);
            LOG_RANK = rank;
            fileStream = new FileStream($@"{LOG_DIR}\{FILE_NAME}", FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            streamWriter = new StreamWriter(fileStream);
        }
        public void Write(LogRank rank, string msg, Exception e = null)
        {
            // 判断日志等级
            if ((int)rank <= (int)LOG_RANK) return;
            switch ((int)rank)
            {
                case 1:
                    System.Diagnostics.Trace.WriteLine($"TRACE\t:{msg}");
                    break;
                case 2:
                    System.Diagnostics.Debug.WriteLine($"DEBUG\t:{msg}");
                    break;
                case 3:
                    System.Diagnostics.Debug.WriteLine($"INFO\t:{msg}");
                    streamWriter.WriteLine($"[INFO]  : {DateTime.Now.ToShortTimeString()} | {msg}");
                    break;
                case 4:
                    System.Diagnostics.Debug.WriteLine($"WARN\t:{msg}");
                    streamWriter.WriteLine($"[WARN]  : {DateTime.Now.ToShortTimeString()} | {msg}" +
                         $"\r\n\t{e?.Message}\r\n\t> {e?.StackTrace}\r\n\t> {e?.Source}\r\n\t> {e?.TargetSite}");
                    break;
                case 5:
                    System.Diagnostics.Debug.WriteLine($"ERROR\t:{msg}");
                    streamWriter.WriteLine($"[ERROR] : {DateTime.Now.ToShortTimeString()} | {msg}" +
                         $"\r\n\t{e?.Message}\r\n\t> {e?.StackTrace}\r\n\t> {e?.Source}\r\n\t> {e?.TargetSite}");
                    break;
                case 6:
                    System.Diagnostics.Debug.Fail($"FATAL\t:{msg}");
                    streamWriter.WriteLine($"[FATAL] : {DateTime.Now.ToShortTimeString()} | {msg}" +
                         $"\r\n\t{e?.Message}\r\n\t> {e?.StackTrace}\r\n\t> {e?.Source}\r\n\t> {e?.TargetSite}");
                    Panuon.UI.Silver.MessageBoxX.Show($"{msg}" +
                         $"\r\n\t{e?.Message}\r\n\t> {e?.StackTrace}\r\n\t> {e?.Source}\r\n\t> {e?.TargetSite}",
                         "严重错误");
                    break;
            }
            streamWriter.Flush();
        }
        public void WriteLine(LogRank rank = LogRank.OFF)
        {
            System.Diagnostics.Debug.WriteLine("");
            if ((int)rank >= 3) streamWriter.WriteLine();
        }
        public void WriteLine(LogRank rank, string msg, Exception e = null)
        {
            Write(rank, msg, e);
            WriteLine(rank);
        }
    }
     public enum LogRank {
        ALL,// 全部开启

        TRACE,// 该级别日志，默认情况下，既不打印到终端也不输出到文件。
        DEBUG,// 该级别日志，默认情况下会打印到终端输出，但是不会归档到日志文件。
        INFO,// 一般这种信息都是一过性的，不会大量反复输出。
        WARN,// 程序处理中遇到非法数据或者某种可能的错误。
        ERROR,// 该错误发生后程序仍然可以运行，但是极有可能运行在某种非正常的状态下，导致无法完成全部既定的功能。
        FATAL,// 表明程序遇到了致命的错误，必须马上终止运行。

        OFF,// 全部关闭
    } 
}

