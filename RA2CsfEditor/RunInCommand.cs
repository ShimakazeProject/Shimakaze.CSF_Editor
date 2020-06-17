namespace RA2CsfEditor
{
    using System;
    using System.Collections.Generic;
    public partial class App : System.Windows.Application
    {
        private void StartWithCommand(string[] args)
        {
            List<FileFormat> export = new List<FileFormat>();
            FileFormat import = null;
            bool IsImport = false;
            if (args.Length == 1) return;
            for (int i = 0; i < args.Length; i++)
            {
                ConsoleManager.Show();
                Console.WriteLine(args[i]);
                // 导出
                if (args[i].StartsWith("--export"))
                {
                    var arg = args[i].Remove(0, 8);
                    if (arg.StartsWith("-csf=")) export.Add(new FileFormat(arg.Remove(0, 5), Format.CSF));
                    else if (arg.StartsWith("-xml=")) export.Add(new FileFormat(arg.Remove(0, 6), Format.XML));
                    else if (arg.StartsWith("-json=")) export.Add(new FileFormat(arg.Remove(0, 5), Format.JSON));
                    else
                    {
                        Console.WriteLine($"错误的参数 :{args[i]}");
                        return;
                    }
                }
                // 导入
                else if (args[i].StartsWith("--import"))
                {
                    if (IsImport)
                    {
                        Console.WriteLine($"重复导入 :{args[i]}");
                        return;
                    }
                    IsImport = true;
                    var arg = args[i].Remove(0, 8);
                    if (arg.StartsWith("-csf=")) import = new FileFormat(arg.Remove(0, 5), Format.CSF);
                    else if (arg.StartsWith("-xml=")) import = new FileFormat(arg.Remove(0, 6), Format.XML);
                    else if (arg.StartsWith("-json=")) import = new FileFormat(arg.Remove(0, 5), Format.JSON);
                    else
                    {
                        Console.WriteLine($"错误的参数 :{args[i]}");
                        return;
                    }
                }

            }
            if (!IsImport)
            {
                Console.WriteLine("缺失导入参数");
                return;
            }
            else GetCommandArgsRun(import, export?.ToArray());
        }
        private async void GetCommandArgsRun(FileFormat i, FileFormat[] e)
        {
            var vm = new ViewModel.MainVM();
            Console.WriteLine("正在导入");
            switch (i.Format)
            {
                case Format.CSF:
                    Console.WriteLine($"正在从CSF导入\t:{i.Path}");
                    await vm.ImportFromCsf(i.Path);
                    break;
                case Format.JSON:
                    Console.WriteLine($"正在从JSON导入\t:{i.Path}");
                    //await vm.ImportFromJson(i.Path);
                    break;
                case Format.XML:
                    Console.WriteLine($"正在从XML导入\t:{i.Path}");
                    //await vm.ImportFromXml(i.Path);
                    break;
            }

            if (e.Length > 0)
            {
                Console.WriteLine("正在导出");
                foreach (var item in e)
                {
                    switch (item.Format)
                    {
                        case Format.CSF:
                            Console.WriteLine($"正在导出为CSF\t:{item.Path}");
                            //await vm.ExportToCsf(item.Path);
                            Console.WriteLine("完成");
                            break;
                        case Format.JSON:
                            Console.WriteLine($"正在导出为JSON\t:{item.Path}");
                            //await vm.ExportToJson(item.Path);
                            Console.WriteLine("完成");
                            break;
                        case Format.XML:
                            Console.WriteLine($"正在导出为XML\t:{item.Path}");
                            //await vm.ExportToXml(item.Path);
                            Console.WriteLine("完成");
                            break;
                    }
                }
                Console.WriteLine("导出完成 正在退出");
                System.Threading.Thread.Sleep(3000);
                ConsoleManager.Hide();
                Shutdown();
            }
            else
            {
                ConsoleManager.Hide();
                VM = vm;
            }
        }
        private class FileFormat
        {
            public string Path { get; private set; }
            public Format Format { get; private set; }
            public FileFormat(string p, Format f)
            {
                Path = p.Replace("\"", string.Empty);
                Format = f;
            }
        }
        private enum Format
        {
            CSF,
            JSON,
            XML
        }

    }
}
