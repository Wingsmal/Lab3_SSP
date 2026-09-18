using System;
using System.Collections.Generic;
using System.Reflection;

namespace ReflectionLab
{

    public interface IPlugin
    {
        string ComponentName { get; }
        string ComponentVersion { get; }
        void Initialize();
    }

    public class DataExporterPlugin : IPlugin
    {
        public string ComponentName => "Модуль экспорта данных";
        public string ComponentVersion => "1.0.0";
        public void Initialize() => Console.WriteLine($"[INFO] {ComponentName} (v{ComponentVersion}) успешно инициализирован.");
    }

    public class TelemetryPlugin : IPlugin
    {
        public string ComponentName => "Модуль сбора телеметрии";
        public string ComponentVersion => "2.1.3";
        public void Initialize() => Console.WriteLine($"[INFO] {ComponentName} (v{ComponentVersion}) успешно инициализирован.");
    }

    public static class Task7
    {
        public static void Run()
        {
            Console.WriteLine("======= БЛОК 3: Динамическая загрузка компонентов (Плагинов) =======");

            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            Type[] assemblyTypes = executingAssembly.GetTypes();

            List<IPlugin> registeredPlugins = new List<IPlugin>();

            Console.WriteLine("Выполнение сканирования типов сборки на наличие реализаций IPlugin...\n");

            // Поиск и регистрация компонентов
            foreach (Type type in assemblyTypes)
            {
                // Фильтрация: тип реализует IPlugin, является конкретным классом (не интерфейсом/абстракцией)
                if (typeof(IPlugin).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                {
                    IPlugin pluginInstance = (IPlugin)Activator.CreateInstance(type);
                    registeredPlugins.Add(pluginInstance);
                    Console.WriteLine($"[DISCOVERY] Обнаружен и загружен компонент: {type.FullName}");
                }
            }

            Console.WriteLine("\nИнициализация зарегистрированных компонентов (Позднее связывание):");
            foreach (var plugin in registeredPlugins)
            {
                // Динамическое получение метаданных и вызов метода
                Type pluginType = plugin.GetType();
                MethodInfo initMethod = pluginType.GetMethod("Initialize");

                if (initMethod != null)
                {
                    initMethod.Invoke(plugin, null);
                }
            }

            Console.WriteLine("===================================================================\n");
        }
    }
}