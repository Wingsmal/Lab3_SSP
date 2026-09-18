using System;
using System.Collections.Generic;
using System.Reflection;

namespace ReflectionLab
{
    // 1. Атрибут для плагинов (Требование из методички)
    [AttributeUsage(AttributeTargets.Class)]
    public class PluginAttribute : Attribute
    {
        public string Name { get; }
        public PluginAttribute(string name) => Name = name;
    }

    // 2. Интерфейс плагина
    public interface IPlugin
    {
        void Execute();
    }

    // 3. Реализации плагинов с использованием Атрибута
    [Plugin("ExportPlugin")]
    public class DataExporterPlugin : IPlugin
    {
        public void Execute() => Console.WriteLine("[Плагин] Экспорт данных завершен.");
    }

    [Plugin("TelemetryPlugin")]
    public class TelemetryPlugin : IPlugin
    {
        public void Execute() => Console.WriteLine("[Плагин] Телеметрия собрана.");
    }

    public class FakePlugin : IPlugin
    {
        // У этого плагина нет атрибута, он не должен быть загружен!
        public void Execute() => Console.WriteLine("Я фейк.");
    }

    // --- Задание 7. Менеджер загрузки ---
    public static class Task7
    {
        public static void Run()
        {
            Console.WriteLine("\n--- ЗАДАНИЕ 7: Динамическая загрузка плагинов по конфигурации ---");

            // Имитация файла конфигурации (в реальном проекте это читалось бы из config.json)
            List<string> allowedPluginsConfig = new List<string> { "ExportPlugin", "TelemetryPlugin" };

            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] types = assembly.GetTypes();

            List<IPlugin> loadedPlugins = new List<IPlugin>();

            foreach (Type type in types)
            {
                // Проверяем, реализует ли класс интерфейс IPlugin
                if (typeof(IPlugin).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                {
                    // Ищем наш кастомный атрибут (Требование методички)
                    PluginAttribute attr = type.GetCustomAttribute<PluginAttribute>();

                    if (attr != null)
                    {
                        // Проверяем, разрешен ли плагин в нашем "файле конфигурации"
                        if (allowedPluginsConfig.Contains(attr.Name))
                        {
                            IPlugin plugin = (IPlugin)Activator.CreateInstance(type);
                            loadedPlugins.Add(plugin);
                            Console.WriteLine($"Обнаружен и разрешен конфигурацией плагин: {attr.Name} (Класс: {type.Name})");
                        }
                    }
                }
            }

            Console.WriteLine("\nЗапуск загруженных плагинов:");
            foreach (var plugin in loadedPlugins)
            {
                plugin.Execute();
            }
        }
    }
}