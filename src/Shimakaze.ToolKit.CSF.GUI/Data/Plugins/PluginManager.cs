using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

using Shimakaze.ToolKit.CSF.Plugin.Interfaces;

namespace Shimakaze.ToolKit.CSF.GUI.Data.Plugins
{
    public static class PluginManager
    {
        public static void Init()
        {
            if (!Directory.Exists("Plugins")) Directory.CreateDirectory("Plugins");
            var pluginsPath = Directory.GetDirectories("Plugins");
            if (pluginsPath.Length < 1) return;

            foreach (var pluginRootDir in pluginsPath)
            {
                var pluginPath = Directory.GetFiles(pluginRootDir).Where(i => i.Split('.').Last().Equals("dll", StringComparison.OrdinalIgnoreCase)).ToArray();

                Assembly pluginAssembly = GetPluginAssembly(pluginPath[0]);
                IPluginInfo pluginInfo = GetPluginInfo(pluginAssembly);

                pluginInfo.Init();
                pluginInfo.Commands[0].Execute();
            }

            return;
            try
            {
                // Load commands from plugins.
                string[] pluginPaths = new string[]
                {
                    // Paths to plugins to load.
                };

                IEnumerable<IPluginCommandInfo> commands = pluginPaths.SelectMany(pluginPath =>
                {
                    Assembly pluginAssembly = LoadPlugin(pluginPath);
                    return CreateCommands(pluginAssembly);
                }).ToList();

                Console.WriteLine("Commands: ");
                // Output the loaded commands.
                foreach (IPluginCommandInfo command in commands)
                {
                    Console.WriteLine($"{command.Name}\t - {command.Description}");
                }
                //foreach (string commandName in args)
                //{
                //    Console.WriteLine($"-- {commandName} --");

                //    // Execute the command with the name passed as an argument.
                //    IPluginCommandInfo command = commands.FirstOrDefault(c => c.Name == commandName);
                //    if (command == null)
                //    {
                //        Console.WriteLine("No such command is known.");
                //        return;
                //    }

                //    command.Execute();

                //    Console.WriteLine();
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        static Assembly LoadPlugin(string relativePath)
        {
            // Navigate up to the solution root
            string root = Path.GetFullPath(Path.Combine(
                Path.GetDirectoryName(
                    Path.GetDirectoryName(
                        Path.GetDirectoryName(
                            Path.GetDirectoryName(
                                Path.GetDirectoryName(typeof(PluginManager).Assembly.Location)))))));

            string pluginLocation = Path.GetFullPath(Path.Combine(root, relativePath.Replace('\\', Path.DirectorySeparatorChar)));
            Console.WriteLine($"Loading commands from: {pluginLocation}");
            PluginLoadContext loadContext = new PluginLoadContext(pluginLocation);
            return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginLocation)));
        }

        static IEnumerable<IPluginCommandInfo> CreateCommands(Assembly assembly)
        {
            int count = 0;

            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(IPluginCommandInfo).IsAssignableFrom(type))
                {
                    if (Activator.CreateInstance(type) is IPluginCommandInfo result)
                    {
                        count++;
                        yield return result;
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
        static Assembly GetPluginAssembly(string relativePath)
        {
            // Navigate up to the solution root
            string root = Environment.CurrentDirectory;

            string pluginLocation = Path.GetFullPath(Path.Combine(root, relativePath.Replace('\\', Path.DirectorySeparatorChar)));
            Console.WriteLine($"Loading commands from: {pluginLocation}");
            PluginLoadContext loadContext = new PluginLoadContext(pluginLocation);
            return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginLocation)));
        }
        static IPluginInfo GetPluginInfo(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(IPluginInfo).IsAssignableFrom(type)
                    && Activator.CreateInstance(type) is IPluginInfo result)
                {
                    return result;
                }
            }
            return null;
        }
    }
    class PluginLoadContext : AssemblyLoadContext
    {
        private AssemblyDependencyResolver _resolver;

        public PluginLoadContext(string pluginPath)
        {
            _resolver = new AssemblyDependencyResolver(pluginPath);
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            string assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
            if (assemblyPath != null)
            {
                return LoadFromAssemblyPath(assemblyPath);
            }

            return null;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            string libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if (libraryPath != null)
            {
                return LoadUnmanagedDllFromPath(libraryPath);
            }

            return IntPtr.Zero;
        }
    }
}
