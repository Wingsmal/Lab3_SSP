using System;
using System.Reflection;

namespace ReflectionLab
{
    // --- ЦЕЛЕВОЙ КЛАСС (Относится к Заданию 4) ---
    public class SecureComponent
    {
        private string _internalToken = "DEFAULT_TOKEN";
        private const string _readOnlyKey = "CONST_KEY"; // Для демонстрации ошибки доступа

        // Публичный метод для чтения приватного значения
        public string GetTokenValue() => _internalToken;

        // Метод без параметров
        private void RebootSystem()
        {
            Console.WriteLine("[System] Выполнена перезагрузка системы без параметров.");
        }

        // Метод с параметрами
        private double ProcessData(int coefficient, double baseMetric)
        {
            return baseMetric * coefficient + 10.0;
        }
    }

    // --- ЗАДАНИЕ 4: Подготовка "Чёрного ящика" ---
    public static class Task4
    {
        public static void Run()
        {
            Console.WriteLine("\n--- ЗАДАНИЕ 4: Создание 'Чёрного ящика' ---");
            SecureComponent component = new SecureComponent();
            Console.WriteLine("Объект SecureComponent создан.");

            // Демонстрируем чтение через публичный метод
            Console.WriteLine($"Начальное значение скрытого поля (через публичный метод): {component.GetTokenValue()}");
            Console.WriteLine("[Инфо] Прямой доступ к полю _internalToken извне закрыт модификатором private.");
        }
    }

    // --- ЗАДАНИЕ 5: Доступ к приватному полю + Исключения ---
    public static class Task5
    {
        public static void Run()
        {
            Console.WriteLine("\n--- ЗАДАНИЕ 5: Изменение приватного поля ---");
            SecureComponent component = new SecureComponent();
            Type type = typeof(SecureComponent);

            try
            {
                // Успешное изменение поля
                FieldInfo tokenField = type.GetField("_internalToken", BindingFlags.NonPublic | BindingFlags.Instance);
                if (tokenField != null)
                {
                    tokenField.SetValue(component, "HACKED_TOKEN");
                    Console.WriteLine($"После взлома значение поля равно: {component.GetTokenValue()}");
                }

                // Демонстрация исключения "Поле не найдено"
                Console.WriteLine("\nПопытка обратиться к несуществующему полю '_notFound'...");
                FieldInfo missingField = type.GetField("_notFound", BindingFlags.NonPublic | BindingFlags.Instance);
                if (missingField == null) throw new MissingFieldException("Поле '_notFound' отсутствует в классе!");

            }
            catch (MissingFieldException ex)
            {
                Console.WriteLine($"[Перехвачено исключение]: {ex.Message}");
            }

            try
            {
                // Демонстрация сценария "Недостаточно прав" (попытка изменить константу)
                Console.WriteLine("\nПопытка изменить константу '_readOnlyKey'...");
                FieldInfo constField = type.GetField("_readOnlyKey", BindingFlags.NonPublic | BindingFlags.Static); // Константы статические
                if (constField != null)
                {
                    constField.SetValue(null, "NEW_CONST"); // Вызовет FieldAccessException
                }
            }
            catch (FieldAccessException)
            {
                Console.WriteLine("[Перехвачено исключение FieldAccessException]: Недостаточно прав для изменения поля (оно является константой).");
            }
        }
    }

    // --- ЗАДАНИЕ 6: Вызов приватных методов ---
    public static class Task6
    {
        public static void Run()
        {
            Console.WriteLine("\n--- ЗАДАНИЕ 6: Вызов приватных методов ---");
            SecureComponent component = new SecureComponent();
            Type type = typeof(SecureComponent);

            // 1. Метод БЕЗ параметров через Invoke
            MethodInfo rebootMethod = type.GetMethod("RebootSystem", BindingFlags.NonPublic | BindingFlags.Instance);
            Console.WriteLine("Вызов метода RebootSystem (0 параметров) через Invoke:");
            rebootMethod.Invoke(component, null);

            // 2. Метод С параметрами через Invoke
            MethodInfo processMethod = type.GetMethod("ProcessData", BindingFlags.NonPublic | BindingFlags.Instance);
            Console.WriteLine("\nВызов метода ProcessData (2 параметра) через Invoke:");
            object result = processMethod.Invoke(component, new object[] { 2, 50.0 });
            Console.WriteLine($"Результат Invoke: {result}");

            // 3. Вызов через создание делегата
            Console.WriteLine("\nВызов метода ProcessData через скомпилированный Делегат:");
            Func<int, double, double> processDelegate = (Func<int, double, double>)Delegate.CreateDelegate(
                typeof(Func<int, double, double>), component, processMethod);

            double delegateResult = processDelegate(3, 50.0);
            Console.WriteLine($"Результат Делегата: {delegateResult}");
        }
    }
}