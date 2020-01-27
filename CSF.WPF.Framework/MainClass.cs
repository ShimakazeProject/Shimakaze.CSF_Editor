using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSF.WPF.Framework
{
    /// <summary>
    /// 整个程序的入口
    /// </summary>
    public static class MainClass
    {
        private const string Kernel32_DllName = "kernel32.dll";
        //GUID
        private const string GUID = "{d1515467-d452-4880-b313-1613858fd188}";

        //避免被多次启动的Mutex
        private static Mutex mutex = new Mutex(true, GUID);

        /// <summary>
        /// 万物之源-主方法
        /// </summary>
        /// <param name="args">参数</param>
        [STAThread]
        public static void Main(string[] args)
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                if (args.Length > 1)// 存在命令行参数
                {
                    var IsImport = false;
                    for (int i = 1; i < args.Length; i++)
                    {
                        var arg = args[i];
                        if (arg.StartsWith("--import", StringComparison.InvariantCultureIgnoreCase)) // 导入
                        {
                            arg = args[i].Remove(0, 8);
                            if (IsImport)
                            {
                                Console.WriteLine($"重复导入 :{args[i]}");
                                return;
                            }
                            IsImport = true;
                            if (arg.StartsWith("-csf=")) return;
                            else if (arg.StartsWith("-xml=")) return;
                            else if (arg.StartsWith("-json=")) return;
                            else
                            {
                                Console.WriteLine($"错误的参数 :{args[i]}");
                                return;
                            }
                        }
                        else if (arg.StartsWith("--export", StringComparison.InvariantCultureIgnoreCase)) // 导出
                        {
                            arg = args[i].Remove(0, 8);
                            if (arg.StartsWith("-csf=")) return;
                            else if (arg.StartsWith("-xml=")) return;
                            else if (arg.StartsWith("-json=")) return;
                            else
                            {
                                Console.WriteLine($"错误的参数 :{args[i]}");
                                return;
                            }
                        }
                    }
                }
                else// args.Length == 1 args[0]就是程序本身 即没有参数
                {
                    var app = new App();
                    app.InitializeComponent();
                    app.StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
                    app.Run();
                }
            }
            else // 当前有实例正在运行
            {

            }
        }

        [DllImport(Kernel32_DllName)]
        private static extern bool AllocConsole();

        [DllImport(Kernel32_DllName)]
        private static extern bool FreeConsole();

        [DllImport(Kernel32_DllName)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport(Kernel32_DllName)]
        private static extern int GetConsoleOutputCP();

        public static bool HasConsole
        {
            get { return GetConsoleWindow() != IntPtr.Zero; }
        }

        /// <summary>
        /// Creates a new console instance if the process is not attached to a console already.
        /// </summary>
        public static void Show()
        {
            //#if DEBUG
            if (!HasConsole)
            {
                AllocConsole();
                InvalidateOutAndError();
            }
            //#endif
        }

        /// <summary>
        /// If the process has a console attached to it, it will be detached and no longer visible. Writing to the System.Console is still possible, but no output will be shown.
        /// </summary>
        public static void Hide()
        {
            //#if DEBUG
            if (HasConsole)
            {
                SetOutAndErrorNull();
                FreeConsole();
            }
            //#endif
        }

        public static void Toggle()
        {
            if (HasConsole)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        static void InvalidateOutAndError()
        {
            Type type = typeof(System.Console);

            System.Reflection.FieldInfo _out = type.GetField("_out",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            System.Reflection.FieldInfo _error = type.GetField("_error",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            System.Reflection.MethodInfo _InitializeStdOutError = type.GetMethod("InitializeStdOutError",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            Debug.Assert(_out != null);
            Debug.Assert(_error != null);

            Debug.Assert(_InitializeStdOutError != null);

            _out.SetValue(null, null);
            _error.SetValue(null, null);

            _InitializeStdOutError.Invoke(null, new object[] { true });
        }

        static void SetOutAndErrorNull()
        {
            Console.SetOut(TextWriter.Null);
            Console.SetError(TextWriter.Null);
        }

    }
}