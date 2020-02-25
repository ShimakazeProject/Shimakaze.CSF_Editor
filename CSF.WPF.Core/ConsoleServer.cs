using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;

namespace CSF.WPF.Core
{
    internal static class ConsoleServer
    {
        #region Field
        internal static NamedPipeServerStream pipeStream = new NamedPipeServerStream(Program.GUID);
        internal static NamedPipeClientStream OutStream = new NamedPipeClientStream("CSF.WPF.Core.OutStream");
        #endregion

        internal static void Serverd()
        {
            while (true)
            {
                try
                {
                    using var readStream = new StreamReader(pipeStream);
                    pipeStream.WaitForConnection();

                }
                catch (ObjectDisposedException)
                {
                    pipeStream = new NamedPipeServerStream(Program.GUID);
                }
            }
        }

        internal static void Write()
        {
            try
            {

            }
            catch (ObjectDisposedException)
            {
                OutStream = new NamedPipeClientStream("CSF.WPF.Core.OutStream");
            }
        }

        //private static async void PipeServer()
        //{
        //    await Task.Run(async () =>
        //    {
        //        while (true)
        //        {
        //            try
        //            {
        //                using (var pipeStream = new NamedPipeServerStream("CSF.WPF.Framework"))
        //                using (StreamReader rdr = new StreamReader(pipeStream))
        //                {
        //                    pipeStream.WaitForConnection();
        //                    string temp;
        //                    while ((temp = rdr.ReadLine()) != PipeConst.End)
        //                    {
        //                        Console.WriteLine("{0}:{1}", DateTime.Now, temp);
        //                        switch (temp)
        //                        {
        //                            case PipeConst.Activate:
        //                                await Window.Dispatcher.InvokeAsync(() => { Window.WindowState = System.Windows.WindowState.Normal; });
        //                                await Window.Dispatcher.InvokeAsync(Window.Activate);
        //                                break;
        //                            case PipeConst.Close:
        //                                await Window.Dispatcher.InvokeAsync(Window.Close);
        //                                break;
        //                            case PipeConst.ShowConsole:
        //                                MainClass.ShowConsole();
        //                                break;
        //                            case PipeConst.HideConsole:
        //                                MainClass.HideConsole();
        //                                break;
        //                            default:
        //                                break;
        //                        }
        //                    }
        //                }
        //            }
        //            catch (ObjectDisposedException)
        //            {
        //                continue;
        //            }
        //        }
        //    });
        //}
    }
}
