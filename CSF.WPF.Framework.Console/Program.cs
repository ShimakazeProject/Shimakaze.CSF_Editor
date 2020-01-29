using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using console = System.Console;

namespace CSF.WPF.Framework.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            console.ForegroundColor = ConsoleColor.Blue;
            console.WriteLine("Welcome to CSF.WPF.Framework.Console!");
            using (var pipeStream = new NamedPipeClientStream("CSF.WPF.Framework"))
            using (StreamWriter writer = new StreamWriter(pipeStream))
            {
                while (true)
                {
                    console.ForegroundColor = ConsoleColor.Blue;
                    console.WriteLine("CSF.WPF.Framework Connecting!");
                    await pipeStream.ConnectAsync();
                    console.ForegroundColor = ConsoleColor.Green;
                    console.WriteLine("CSF.WPF.Framework Connected!");
                    writer.AutoFlush = true;
                    try
                    {
                        while (true)
                        {
                            writer.Flush();
                            console.ForegroundColor = ConsoleColor.White;
                            writer.WriteLine(console.ReadLine());
                        }
                    }
                    catch (System.IO.IOException)
                    {
                        console.ForegroundColor = ConsoleColor.Red;
                        console.WriteLine("CSF.WPF.Framework Disconnected!");
                    }
                }
            }

        }
    }
}
