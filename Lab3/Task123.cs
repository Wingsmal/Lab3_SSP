using System;
using System.Linq;
using System.Reflection;

namespace ReflectionLab
{
    // инициализация класса по заданию 1 
    public class UserProfile
    {
        private int _identifier;
        private string _accountName;
        private double _accountBalance;

        public int Identifier
        {
            get => _identifier;
            set { if (value >= 0) _identifier = value; else throw new ArgumentException("Id не может быть < 0."); }
        }

        public string AccountName
        {
            get => _accountName;
            set { if (!string.IsNullOrWhiteSpace(value)) _accountName = value; else throw new ArgumentException("Имя пустое."); }
        }

        public double AccountBalance
        {
            get => _accountBalance;
            set { if (value >= 0) _accountBalance = value; else throw new ArgumentException("Баланс < 0."); }
        }

        public UserProfile() { }

        public UserProfile(int id, string name, double balance)
        {
            Identifier = id;
            AccountName = name;
            AccountBalance = balance;
        }

        public void DisplayInfo()
        {
            Console.WriteLine($"[User] ID={Identifier}, Name={AccountName}, Balance={AccountBalance}");
        }

        private void SecretMethod() { /* Приватный метод для 2 задания */ }
    }

    // --- ЗАДАНИЕ 1: Подготовка класса и демонстрация валидации ---
    public static class Task1
    {
        public static void Run()
        {
            Console.WriteLine("\n--- ЗАДАНИЕ 1: Создание целевого класса с валидацией ---");
            Console.WriteLine("Попытка создать объект UserProfile с некорректными данными (ID = -5)...");
            try
            {
                // Демонстрируем, что наша инкапсуляция и валидация работают
                UserProfile testUser = new UserProfile(-5, "Test", 100);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"[Успех] Валидация свойств отработала корректно. Перехвачена ошибка: {ex.Message}");
            }
        }
    }

    // --- ЗАДАНИЕ 2: Инспекция методов и BindingFlags ---
    public static class Task2
    {
        public static void Run()
        {
            Console.WriteLine("\n--- ЗАДАНИЕ 2: Инспекция методов (Разница BindingFlags) ---");
            Type type = typeof(UserProfile);

            Console.WriteLine("\n1. Только публичные методы (BindingFlags.Public | BindingFlags.Instance):");
            var publicMethods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            PrintMethods(publicMethods);

            Console.WriteLine("\n2. Только скрытые методы (BindingFlags.NonPublic | BindingFlags.Instance):");
            var privateMethods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
            PrintMethods(privateMethods);
        }

        private static void PrintMethods(MethodInfo[] methods)
        {
            foreach (var m in methods)
            {
                // Фильтрация свойств (get_ и set_)
                if (m.Name.StartsWith("get_") || m.Name.StartsWith("set_")) continue;

                string access = m.IsPublic ? "public" : "private";
                string parameters = string.Join(", ", m.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));
                Console.WriteLine($"  - {access} {m.ReturnType.Name} {m.Name}({parameters})");
            }
        }
    }

    // --- ЗАДАНИЕ 3: Позднее связывание (Activator) ---
    public static class Task3
    {
        public static void Run()
        {
            Console.WriteLine("\n--- ЗАДАНИЕ 3: Динамическое создание (Activator) ---");
            Type type = typeof(UserProfile);

            // Способ 1: Конструктор без параметров + установка свойств
            object user1 = Activator.CreateInstance(type);
            type.GetProperty("AccountName").SetValue(user1, "Admin");
            type.GetProperty("Identifier").SetValue(user1, 101);
            Console.WriteLine("Пользователь 1 создан (через конструктор без параметров).");
            type.GetMethod("DisplayInfo").Invoke(user1, null);

            // Способ 2: Конструктор с параметрами
            object user2 = Activator.CreateInstance(type, new object[] { 202, "Moderator", 500.5 });
            Console.WriteLine("Пользователь 2 создан (через конструктор с параметрами).");
            type.GetMethod("DisplayInfo").Invoke(user2, null);
        }
    }
}