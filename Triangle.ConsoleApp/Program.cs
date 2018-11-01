using System;

namespace Triangle.ConsoleApp
{        
    class Program
    {
        static void Main(string[] args)
        {
            var summFinder = new SummFinder(@"..\..\input.txt");
            var summ = summFinder.GetMaxSumm();

            Console.WriteLine();
            Console.WriteLine($"Max summ is {summ}.");

            Console.ReadKey();
        }
    }
}
