using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CSF.Logmgr
{
    public static class Logger
    {
        private static string logger_NAME;
        private static string logger_DIR;
        private static bool toFile;
        private static bool toConsole;
        private static Rank Rank = Rank.INFO;
        private const string TIME_FORMAT = "MM.dd-HH:mm:ss.fff";
        private const string RANK_FORMAT = " {0}\t";
        public static string Logger_File => logger_DIR + logger_NAME;
        public static void Init(string fileDir, string fileName, bool writeToFile = true, bool writeToConsole = false)
        {
            logger_DIR = fileDir;
            logger_NAME = fileName;
            toFile = writeToFile;
            toConsole = writeToConsole;
            if (File.Exists(Logger_File))
            {
                File.Delete(Logger_File);
            }
        }
        public static void Init(string fileDir, string fileName, Rank rank, bool writeToFile = true, bool writeToConsole = false)
        {
            Init(fileDir, fileName, writeToFile, writeToConsole);
            Rank = rank;
        }
        public static void Init(string fileDir, string fileName, string rank, bool writeToFile = true, bool writeToConsole = false)
            => Init(fileDir, fileName, (Rank)Enum.Parse(typeof(Rank), rank.ToUpper()), writeToFile, writeToConsole);
        public static void Log(string str) => Info(str);
        public static void Trace(string str)
        {
            System.Diagnostics.Trace.WriteLine(string.Format(RANK_FORMAT, "Trace"), str);
#if DEBUG
            if (Rank <= Rank.TRACE)
            {
                var rank = string.Format(RANK_FORMAT, "Trace");
                if (toFile) WriteFile(rank, str);
                if (toConsole) WriteConsole(rank, str);
            }
#endif
        }
        public static void Trace(string str, params object[] args) => Trace(string.Format(str, args));

        public static void Debug(string str)
        {
            if (Rank <= Rank.DEBUG)
            {
                var rank = string.Format(RANK_FORMAT,"Debug");
                System.Diagnostics.Trace.WriteLine(rank, str);
#if DEBUG
                if (toFile) WriteFile(rank, str);
#endif
                if (toConsole) WriteConsole(rank, str);
            }
        }
        public static void Debug(string str, params object[] args) => Debug(string.Format(str, args));

        public static void Info(string str)
        {
            if (Rank <= Rank.INFO)
            {
                var rank = string.Format(RANK_FORMAT,"Info");
                System.Diagnostics.Trace.WriteLine(rank, str);
                if (toFile) WriteFile(rank, str);
                if (toConsole) WriteConsole(rank, str, ConsoleColor.White);
            }
        }
        public static void Info(string str, params object[] args) => Info(string.Format(str, args));

        public static void Warn(string str)
        {
            if (Rank <= Rank.WARN)
            {
                var rank = string.Format(RANK_FORMAT,"Warn");
                System.Diagnostics.Trace.WriteLine(rank, str);
                if (toFile) WriteFile(rank, str);
                if (toConsole) WriteConsole(rank, str, ConsoleColor.Yellow);
            }
        }
        public static void Warn(string str, params object[] args) => Warn(string.Format(str, args));

        public static void Error(string str)
        {
            if (Rank <= Rank.ERROR)
            {
                var rank = string.Format(RANK_FORMAT,"Error");
                System.Diagnostics.Trace.WriteLine(rank, str);
                if (toFile) WriteFile(rank, str);
                if (toConsole) WriteConsole(rank, str, ConsoleColor.Red);
            }
        }
        public static void Error(string str, params object[] args) => Error(string.Format(str, args));

        public static void Fatal(string str)
        {
            if (Rank <= Rank.FATAL)
            {
                var rank = string.Format(RANK_FORMAT,"Fatal");
                System.Diagnostics.Trace.WriteLine(rank, str);
                if (toFile) WriteFile(rank, str);
                if (toConsole) WriteConsole(rank, str, ConsoleColor.DarkRed);
            }
        }
        public static void Fatal(string str, params object[] args) => Fatal(string.Format(str, args));

        private static void WriteFile(string rank, string str)
        {
            try
            {
                using var fs = File.Exists(Logger_File)
                    ? new FileStream(Logger_File, FileMode.Append, FileAccess.Write, FileShare.Read)
                    : new FileStream(Logger_File, FileMode.Create, FileAccess.Write, FileShare.Read);
                using var sw = new StreamWriter(fs);
                sw.Write("[{0}] {1}", DateTime.Now.ToString(TIME_FORMAT), rank);
                sw.WriteLine(str);
            }
            catch { }
        }
        private static void WriteConsole(string rank, string str)
        {
            Console.ResetColor();
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write(DateTime.Now.ToString(TIME_FORMAT));
            Console.ResetColor();
            Console.Write("] ");

            Console.Write(rank);
            Console.WriteLine(str);
        }
        private static void WriteConsole(string rank, string str, ConsoleColor color)
        {
            Console.ResetColor();
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write(DateTime.Now.ToString(TIME_FORMAT));
            Console.ResetColor();
            Console.Write("] ");

            Console.ForegroundColor = color;
            Console.Write(rank);

            Console.ResetColor();
            Console.WriteLine(str);
        }
    }
    public enum Rank
    {
        ALL,// 全部开启

        /// <summary>
        ///  该级别日志，默认情况下，既不打印到终端也不输出到文件。
        /// </summary>
        TRACE,
        /// <summary>
        ///  该级别日志，默认情况下会打印到终端输出，但是不会归档到日志文件。
        /// </summary>
        DEBUG,
        /// <summary>
        ///  一般这种信息都是一过性的，不会大量反复输出。
        /// </summary>
        INFO,
        /// <summary>
        ///  程序处理中遇到非法数据或者某种可能的错误。
        /// </summary>
        WARN,
        /// <summary>
        ///  该错误发生后程序仍然可以运行，但是极有可能运行在某种非正常的状态下，导致无法完成全部既定的功能。
        /// </summary>
        ERROR,
        /// <summary>
        ///  表明程序遇到了致命的错误，必须马上终止运行。
        /// </summary>
        FATAL,

        OFF,// 全部关闭
    }
}
