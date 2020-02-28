using CSF.Logmgr;
using CSF.Plugin;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CSF.WPF.Core.Data.Plugin
{
    internal class PluginManager
    {
        public static Assembly LoadPlugin(string relativePath)
        {
            // Navigate up to the solution root
            string root = Path.GetFullPath(Path.Combine(
                Path.GetDirectoryName(
                    Path.GetDirectoryName(
                        Path.GetDirectoryName(
                            Path.GetDirectoryName(
                                Path.GetDirectoryName(typeof(Program).Assembly.Location)))))));

            string pluginLocation = Path.GetFullPath(Path.Combine(root, relativePath.Replace('\\', Path.DirectorySeparatorChar)));
            Console.WriteLine($"Loading commands from: {pluginLocation}");
            PluginLoadContext loadContext = new PluginLoadContext(pluginLocation);
            return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginLocation)));
        }

        public static IEnumerable<(Type,IPlugin)> CreateCommands(Assembly assembly)
        {
            int count = 0;
            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(IPlugin).IsAssignableFrom(type))
                {
                    IPlugin result = Activator.CreateInstance(type) as IPlugin;
                    if (result != null)
                    {
                        count++;
                        yield return (type, result);
                    }
                }
            }

            if (count == 0)
            {
                string availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
                throw new ApplicationException(
                    $"Can't find any type which implements ICommand in {assembly} from {assembly.Location}.\n" +
                    $"Available types: {availableTypes}");
            }
        }
        public static void SwitchPlugin(object sender, RoutedEventArgs e)
        {
            var sb = new StringBuilder();
            using (var sr = new StreamReader(new FileStream(AppContext.BaseDirectory + "\\Plugins\\Plugins", FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine().Split('|');
                    if (((sender as MenuItem).Header as string).Trim().Equals(line[1].Trim()))
                    {
                        sb.AppendLine(string.Format("{0}|{1}|{2}", (sender as MenuItem).IsChecked ? "0" : "1", line[1], line[2]));
                    }
                    else
                    {
                        sb.AppendLine(string.Format("{0}|{1}|{2}", line[0], line[1], line[2]));
                    }
                }
            }
            File.Delete(AppContext.BaseDirectory + "\\Plugins\\Plugins");
            File.WriteAllText(AppContext.BaseDirectory + "\\Plugins\\Plugins", sb.ToString());
            MessageBox.Show("重启应用生效", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
