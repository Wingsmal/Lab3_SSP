using System;
using System.Linq;
using System.Reflection;

namespace ReflectionLab
{
    public class UserProfile
    {
        private int _age;
        private string _username;
        private double _balance;

        public int Age
        {
            get => _age;
            set { if (value >= 0 && value <= 120) _age = value; else throw new ArgumentException("Неверный возраст"); }
        }

        public string Username
        {
            get => _username;
            set { if (!string.IsNullOrWhiteSpace(value)) _username = value; else throw new ArgumentException("Имя не может быть пустым"); }
        }

        public double Balance
        {
            get => _balance;
            set { if (value >= 0) _balance = value; else throw new ArgumentException("Баланс не может быть отрицательным"); }
        }

        public UserProfile() { }

        public UserProfile(int age, string username, double balance)
        {
            Age = age;
            Username = username;
            Balance = balance;
        }

        public void DisplayInfo()
        {
            Console.WriteLine($"[UserProfile Info] Имя: {Username}, Возраст: {Age}, Баланс: {Balance}");
        }
    }

    public static class Tasks123
    {
        public static void Run()
        {
            Console.WriteLine("======= ЗАДАНИЯ 1, 2, 3: БАЗОВАЯ РЕФЛЕКСИЯ =======");

            Type type = typeof(UserProfile);

            Console.WriteLine("\n--- Задание 2: Инспекция методов (без get_ и set_) ---");

            MethodInfo[] allMethods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

            foreach (var method in allMethods)
            {
                if (method.Name.StartsWith("get_") || method.Name.StartsWith("set_"))
                    continue;

                string access = method.IsPublic ? "public" : (method.IsPrivate ? "private" : "protected");
                string isStatic = method.IsStatic ? "static" : "instance";
                var parameters = string.Join(", ", method.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));

                Console.WriteLine($"{access} {isStatic} {method.ReturnType.Name} {method.Name}({parameters})");
            }

 
            Console.WriteLine("\n--- Задание 3: Создание объектов через Activator ---");

            object obj1 = Activator.CreateInstance(type);

            type.GetProperty("Username").SetValue(obj1, "Студент");
            type.GetProperty("Age").SetValue(obj1, 20);

            Console.WriteLine($"Объект 1 создан. Читаем свойства: Имя = {type.GetProperty("Username").GetValue(obj1)}, Возраст = {type.GetProperty("Age").GetValue(obj1)}");

            object obj2 = Activator.CreateInstance(type, new object[] { 35, "Преподаватель", 10000.0 });
            Console.WriteLine("Объект 2 создан с параметрами.");

            type.GetMethod("DisplayInfo").Invoke(obj2, null);

            Console.WriteLine("==================================================\n");
        }
    }
}