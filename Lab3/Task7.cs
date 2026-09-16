using System;
using System.Collections.Generic;
using System.Reflection;

namespace ReflectionLab
{
    public interface IPlugin
    {
        string Name { get; }
        string Version { get; }
        void Execute();
    }

    public class CalculatorPlugin : IPlugin
    {
        public string Name => "Калькулятор";
        public string Version => "1.0.0";
        public void Execute() => Console.WriteLine($"[Плагин '{Name}' v{Version}] Успешно запущен!");
    }

    public class LoggerPlugin : IPlugin
    {
        public string Name => "Системный Логгер";
        public string Version => "2.1.0";
        public void Execute() => Console.WriteLine($"[Плагин '{Name}' v{Version}] Логирование активировано.");
    }

    public static class Task7
    {
        public static void Run()
        {
            Console.WriteLine("======= ЗАДАНИЕ 7: ДИНАМИЧЕСКАЯ ЗАГРУЗКА ПЛАГИНОВ =======");

            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            Type[] allTypes = currentAssembly.GetTypes();

            List<IPlugin> loadedPlugins = new List<IPlugin>();

            Console.WriteLine("Поиск типов, реализующих IPlugin, в текущей сборке...\n");

            foreach (Type type in allTypes)
            {
                if (typeof(IPlugin).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                {
                    IPlugin plugin = (IPlugin)Activator.CreateInstance(type);
                    loadedPlugins.Add(plugin);
                    Console.WriteLine($"[+] Найден и загружен класс: {type.Name}");
                }
            }

            Console.WriteLine("\nВыполнение загруженных плагинов (Позднее связывание):");
            foreach (var plugin in loadedPlugins)
            {
                Type pluginType = plugin.GetType();
                MethodInfo executeMethod = pluginType.GetMethod("Execute");

                if (executeMethod != null)
                {
                    executeMethod.Invoke(plugin, null);
                }
            }

            Console.WriteLine("=========================================================\n");
        }
    }
}