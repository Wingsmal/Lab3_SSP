using System;

namespace ReflectionLab
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== ЛАБОРАТОРНАЯ РАБОТА: РЕФЛЕКСИЯ ===");

            Task1.Run();
            Task2.Run();
            Task3.Run();
            Task4.Run();
            Task5.Run();
            Task6.Run();
            Task7.Run();

            Console.WriteLine("\nРабота завершена. Нажмите любую клавишу...");
            Console.ReadKey();
        }
    }
}