using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ReflectionLab
{

    // Общий интерфейс для всех плагинов
    public interface IPlugin
    {
        string Name { get; }
        string Version { get; }
        void Execute();
    }

    // Дополнительный интерфейс для плагинов с параметрами
    public interface IParameterizedPlugin : IPlugin
    {
        void ExecuteWithParams(Dictionary<string, object> parameters);
    }


    public class CalculatorPlugin : IPlugin
    {
        public string Name => "Calculator";
        public string Version => "1.0.0";

        public void Execute()
        {
            Console.WriteLine("Calculator plugin loaded");
        }

        public double Add(double a, double b) => a + b;
        public double Subtract(double a, double b) => a - b;
    }

    //  Логгер с параметрами
    public class LoggerPlugin : IParameterizedPlugin
    {
        public string Name => "Logger";
        public string Version => "1.0.0";

        public void Execute()
        {
            Console.WriteLine("Logger plugin executed");
        }

        public void ExecuteWithParams(Dictionary<string, object> parameters)
        {
            if (parameters.TryGetValue("message", out var msg))
            {
                Console.WriteLine($"[LOG] {msg}");
            }
        }
    }

 
    public class PluginManager
    {
        private readonly List<IPlugin> _plugins = new List<IPlugin>();
        private readonly string _pluginsPath;

        public PluginManager(string pluginsPath = null)
        {
            _pluginsPath = pluginsPath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");
            Directory.CreateDirectory(_pluginsPath);
        }

        public void LoadBuiltInPlugins()
        {
            Console.WriteLine("Loading built-in plugins...");
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            LoadPluginFromAssemblyObject(executingAssembly);
        }

        // Загрузка всех плагинов из директории
        public void LoadPlugins()
        {
            var dllFiles = Directory.GetFiles(_pluginsPath, "*.dll");

            foreach (var dllPath in dllFiles)
            {
                try
                {
                    Assembly assembly = Assembly.LoadFrom(dllPath);
                    LoadPluginFromAssemblyObject(assembly);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading plugin {dllPath}: {ex.Message}");
                }
            }
        }

        // Общий метод разбора сборки и поиска интерфейса (объединил логику из методички для чистоты кода)
        private void LoadPluginFromAssemblyObject(Assembly assembly)
        {
            Type[] types = assembly.GetTypes();

            foreach (var type in types)
            {
                if (typeof(IPlugin).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                {
                    IPlugin plugin = (IPlugin)Activator.CreateInstance(type);
                    _plugins.Add(plugin);
                    Console.WriteLine($"Loaded plugin: {plugin.Name} v{plugin.Version}");
                }
            }
        }

        public IEnumerable<IPlugin> GetPlugins() => _plugins;


        public IPlugin GetPlugin(string name)
        {
            return _plugins.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public void ExecutePluginMethod(IPlugin plugin, string methodName, object[] parameters)
        {
            Type pluginType = plugin.GetType();

            // Поиск метода
            MethodInfo method = pluginType.GetMethod(methodName);

            if (method != null)
            {
                // Вызов метода
                var result = method.Invoke(plugin, parameters);
                Console.WriteLine($"Method {methodName} executed. Result: {result ?? "null"}");
            }
            else
            {
                Console.WriteLine($"Method {methodName} not found in plugin {plugin.Name}");
            }
        }

        // Вызов метода с параметрами через рефлексию с проверкой типов
        public T ExecutePluginMethod<T>(IPlugin plugin, string methodName, object[] parameters)
        {
            Type pluginType = plugin.GetType();
            MethodInfo method = pluginType.GetMethod(methodName);

            if (method != null && method.ReturnType == typeof(T))
            {
                return (T)method.Invoke(plugin, parameters);
            }

            return default;
        }
    }


    public static class Task7
    {
        public static void Run()
        {
            Console.WriteLine("\n=== Plugin System Demo ===");

            PluginManager manager = new PluginManager();
            manager.LoadBuiltInPlugins(); // Загружаем плагины из текущего кода

            Console.WriteLine("\n=== Available Plugins ===");
            foreach (var p in manager.GetPlugins())
            {
                Console.WriteLine($"- {p.Name} (v{p.Version})");
                if (p is IParameterizedPlugin)
                {
                    Console.WriteLine("  Supports parameters");
                }
            }

            Console.WriteLine("\n=== Plugin Demonstrations ===");

            var calcPlugin = manager.GetPlugin("Calculator");
            if (calcPlugin != null)
            {
                Console.WriteLine("\nTesting Calculator plugin:");
                calcPlugin.Execute();
                Console.WriteLine("Using reflection to call methods:");

                // Вызов через Generic метод (возвращает double)
                double sum = manager.ExecutePluginMethod<double>(calcPlugin, "Add", new object[] { 15.5, 24.7 });
                Console.WriteLine($"15.5 + 24.7 = {sum}");

                double diff = manager.ExecutePluginMethod<double>(calcPlugin, "Subtract", new object[] { 50.0, 12.3 });
                Console.WriteLine($"50.0 - 12.3 = {diff}");
            }

            var loggerPlugin = manager.GetPlugin("Logger");
            if (loggerPlugin != null)
            {
                Console.WriteLine("\nTesting Logger plugin:");
                loggerPlugin.Execute();
                Console.WriteLine("Using parameterized execution:");

                if (loggerPlugin is IParameterizedPlugin paramLogger)
                {
                    var args = new Dictionary<string, object> { { "message", "Hello from Plugin System!" } };
                    paramLogger.ExecuteWithParams(args);
                }

                Console.WriteLine("Using reflection with parameters:");
                var argsForReflection = new Dictionary<string, object> { { "message", "Reflection call!" } };
                manager.ExecutePluginMethod(loggerPlugin, "ExecuteWithParams", new object[] { argsForReflection });
            }

            Console.WriteLine("\n=== Error Handling Demo ===");
            Console.WriteLine("Trying to call non-existent method:");
            manager.ExecutePluginMethod(calcPlugin, "NonExistentMethod", null);
        }
    }
}