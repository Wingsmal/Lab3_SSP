using System;
using System.Linq;
using System.Reflection;

namespace ReflectionLab
{
    public class UserProfile
    {
        private int _identifier;
        private string _accountName;
        private double _accountBalance;

        public int Identifier
        {
            get => _identifier;
            set { if (value >= 0) _identifier = value; else throw new ArgumentException("Идентификатор не может быть отрицательным."); }
        }

        public string AccountName
        {
            get => _accountName;
            set { if (!string.IsNullOrWhiteSpace(value)) _accountName = value; else throw new ArgumentException("Имя аккаунта не может быть пустым."); }
        }

        public double AccountBalance
        {
            get => _accountBalance;
            set { if (value >= 0) _accountBalance = value; else throw new ArgumentException("Баланс не может быть отрицательным."); }
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
            Console.WriteLine($"[UserProfile] Состояние объекта: Identifier={Identifier}, AccountName='{AccountName}', AccountBalance={AccountBalance}");
        }
    }

    public static class Tasks123
    {
        public static void Run()
        {
            Console.WriteLine("======= РАЗДЕЛ 1: Извлечение метаданных и динамическая инициализация =======");

            Type targetType = typeof(UserProfile);

            Console.WriteLine("\n--- 1.1 Анализ методов целевого типа (исключая аксессоры свойств) ---");

            MethodInfo[] methods = targetType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

            foreach (var method in methods)
            {
                if (method.Name.StartsWith("get_") || method.Name.StartsWith("set_"))
                    continue;

                string accessModifier = method.IsPublic ? "public" : (method.IsPrivate ? "private" : "protected");
                string instanceModifier = method.IsStatic ? "static" : "instance";
                var parameters = string.Join(", ", method.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));

                Console.WriteLine($"Сигнатура: {accessModifier} {instanceModifier} {method.ReturnType.Name} {method.Name}({parameters})");
            }

            Console.WriteLine("\n--- 1.2 Инстанцирование объектов с использованием класса Activator ---");
            object defaultInstance = Activator.CreateInstance(targetType);

            targetType.GetProperty("AccountName").SetValue(defaultInstance, "TestAccount_01");
            targetType.GetProperty("Identifier").SetValue(defaultInstance, 101);

            Console.WriteLine($"Экземпляр №1 инстанцирован (конструктор по умолчанию). Валидация свойств: AccountName='{targetType.GetProperty("AccountName").GetValue(defaultInstance)}', Identifier={targetType.GetProperty("Identifier").GetValue(defaultInstance)}");
            object parameterizedInstance = Activator.CreateInstance(targetType, new object[] { 202, "SystemAdmin", 50000.0 });
            Console.WriteLine("Экземпляр №2 инстанцирован (параметризованный конструктор).");

            targetType.GetMethod("DisplayInfo").Invoke(parameterizedInstance, null);

            Console.WriteLine("============================================================================\n");
        }
    }
}