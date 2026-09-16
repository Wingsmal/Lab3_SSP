using System;

namespace ReflectionLab
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Начало выполнения лабораторной работы: Рефлексия в C#\n");
            Tasks123.Run();
            Tasks456.Run();
            Task7.Run();

            Console.WriteLine("\nЛабораторная работа завершена. Нажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}