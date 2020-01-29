using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSF.WPF.Framework.ViewModel
{
    internal class MainWindowVM
    {
        private MainWindow Window;


        internal object ActiveContent { get; set; }

        internal MainWindowVM(MainWindow mainWindow)
        {
            Window = mainWindow;
            PipeServer();
            
        }


        private async void PipeServer()
        {
            await Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        using (var pipeStream = new NamedPipeServerStream("CSF.WPF.Framework"))
                        using (StreamReader rdr = new StreamReader(pipeStream))
                        {
                            pipeStream.WaitForConnection();
                            string temp;
                            while ((temp = rdr.ReadLine()) != PipeConst.End)
                            {
                                Console.WriteLine("{0}:{1}", DateTime.Now, temp);
                                switch (temp)
                                {
                                    case PipeConst.Activate:
                                        await Window.Dispatcher.InvokeAsync(() => { Window.WindowState = System.Windows.WindowState.Normal; });
                                        await Window.Dispatcher.InvokeAsync(Window.Activate);
                                        break;
                                    case PipeConst.Close:
                                        await Window.Dispatcher.InvokeAsync(Window.Close);
                                        break;
                                    case PipeConst.ShowConsole:
                                        MainClass.ShowConsole();
                                        break;
                                    case PipeConst.HideConsole:
                                        MainClass.HideConsole();
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                    catch (ObjectDisposedException)
                    {
                        continue;
                    }
                }
            });
        }

    }
}
