using System;
using System.Reflection;

namespace ReflectionLab
{
    public class BlackBox
    {
        private string _secretData = "Секретный код: 0000";

        public string GetSecretData() => _secretData;

        private double CalculateSecretFormula(int multiplier, double baseValue)
        {
            return baseValue * multiplier + 50.0;
        }
    }

    public static class Tasks456
    {
        public static void Run()
        {
            Console.WriteLine("======= ЗАДАНИЯ 4, 5, 6: РАБОТА С ПРИВАТНЫМИ ЧЛЕНАМИ =======");

            BlackBox box = new BlackBox();
            Type type = typeof(BlackBox);

            Console.WriteLine("\n--- Задание 5: Изменение приватного поля ---");
            Console.WriteLine($"Значение ДО изменения: {box.GetSecretData()}");

            FieldInfo secretField = type.GetField("_secretData", BindingFlags.NonPublic | BindingFlags.Instance);
            if (secretField != null)
            {
                secretField.SetValue(box, "Секретный код взломан: 9999");
                Console.WriteLine($"Значение ПОСЛЕ изменения: {box.GetSecretData()}");
            }

            try
            {
                Console.WriteLine("\nПопытка доступа к несуществующему полю...");
                FieldInfo fakeField = type.GetField("_notFound", BindingFlags.NonPublic | BindingFlags.Instance);
                fakeField.SetValue(box, "test");
            }
            catch (NullReferenceException)
            {
                Console.WriteLine("Перехвачено исключение: Поле не найдено, невозможно изменить значение.");
            }

            Console.WriteLine("\n--- Задание 6: Вызов приватных методов ---");

            MethodInfo calcMethod = type.GetMethod("CalculateSecretFormula", BindingFlags.NonPublic | BindingFlags.Instance);

            object resultInvoke = calcMethod.Invoke(box, new object[] { 2, 100.0 });
            Console.WriteLine($"Результат вызова через Invoke(2, 100.0) = {resultInvoke}");
            Func<int, double, double> calcDelegate = (Func<int, double, double>)Delegate.CreateDelegate(
                typeof(Func<int, double, double>),
                box,
                calcMethod);

            double resultDelegate = calcDelegate(3, 100.0);
            Console.WriteLine($"Результат вызова через Delegate(3, 100.0) = {resultDelegate}");

            Console.WriteLine("============================================================\n");
        }
    }
}