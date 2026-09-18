using System;
using System.Reflection;

namespace ReflectionLab
{
    public class SecureComponent
    {
        private string _internalToken = "DEFAULT_TOKEN_V1";

        public string GetTokenValue() => _internalToken;

        private double ProcessRestrictedData(int coefficient, double baseMetric)
        {
            return baseMetric * coefficient + 25.5;
        }
    }

    public static class Tasks456
    {
        public static void Run()
        {
            Console.WriteLine("======= РАЗДЕЛ 2: Инспекция приватных членов и обход модификаторов доступа =======");

            SecureComponent component = new SecureComponent();
            Type componentType = typeof(SecureComponent);

            Console.WriteLine("\n--- 2.1 Модификация значения приватного поля ---");
            Console.WriteLine($"Исходное состояние поля _internalToken: {component.GetTokenValue()}");

            FieldInfo tokenField = componentType.GetField("_internalToken", BindingFlags.NonPublic | BindingFlags.Instance);
            if (tokenField != null)
            {
                tokenField.SetValue(component, "MUTATED_TOKEN_V2");
                Console.WriteLine($"Состояние поля _internalToken после рефлексивной записи: {component.GetTokenValue()}");
            }

            try
            {
                Console.WriteLine("\nТестирование поведения при обращении к отсутствующему полю (ожидается исключение)...");
                FieldInfo invalidField = componentType.GetField("_nullField", BindingFlags.NonPublic | BindingFlags.Instance);
                invalidField.SetValue(component, "test_value");
            }
            catch (NullReferenceException)
            {
                Console.WriteLine("Исключение NullReferenceException успешно перехвачено. Причина: целевое поле не найдено в метаданных типа.");
            }

            Console.WriteLine("\n--- 2.2 Динамический вызов приватного метода ---");

            MethodInfo processMethod = componentType.GetMethod("ProcessRestrictedData", BindingFlags.NonPublic | BindingFlags.Instance);

            // Метод 1: Использование MethodInfo.Invoke
            object resultFromInvoke = processMethod.Invoke(component, new object[] { 4, 150.0 });
            Console.WriteLine($"Выполнение через MethodInfo.Invoke (входные аргументы: 4, 150.0): возвращено значение {resultFromInvoke}");

            // Метод 2: Создание делегата
            Func<int, double, double> processDelegate = (Func<int, double, double>)Delegate.CreateDelegate(
                typeof(Func<int, double, double>),
                component,
                processMethod);

            double resultFromDelegate = processDelegate(5, 150.0);
            Console.WriteLine($"Выполнение через скомпилированный делегат (входные аргументы: 5, 150.0): возвращено значение {resultFromDelegate}");

            Console.WriteLine("==================================================================================\n");
        }
    }
}