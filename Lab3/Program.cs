using System;

namespace ReflectionLab
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Запуск процесса: Анализ механизмов System.Reflection в среде .NET\n");

            Tasks123.Run();
            Tasks456.Run();
            Task7.Run();

            Console.WriteLine("\nВсе процедуры выполнены штатно. Нажмите любую клавишу для завершения работы...");
            Console.ReadKey();
        }
    }
}