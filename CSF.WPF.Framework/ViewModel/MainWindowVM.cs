using CSF.WPF.Framework.CommandBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.AvalonDock.Layout;

namespace CSF.WPF.Framework.ViewModel
{
    internal class MainWindowVM
    {
        private MainWindow Window;


        internal object ActiveContent { get; set; }

        public RelayCommand OpenCsfDocument => new RelayCommand(OpenCsf);

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

        private void OpenCsf()
        {
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
                    var doc = new Controls.Document();
                    var docVM = new DocumentVM(doc);
                    doc.DataContext = docVM;

                    docVM.OpenCsf(ofd.FileName);

                    LayoutDocument anchorable = new LayoutDocument
                    {
                        Title = ofd.SafeFileName,
                        Content = doc
                    };

                    var child = Window.layoutRoot.RootPanel.Children;
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
                        Window.layoutRoot.RootPanel.Children.Add(new LayoutDocumentPane(anchorable));
                        success = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, "[MainWindow][NewTab_MenuItemClick]");
            }
        }

    }
}
