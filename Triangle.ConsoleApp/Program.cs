using System;
using System.IO;
using Triangle.Core;

namespace Triangle.ConsoleApp
{        
    class Program
    {
        static void Main(string[] args)
        {
            var summFinder = new SummFinder(new SummFinderConsoleLogger());
            summFinder.LoadFromFile(new FileInfo("input.txt"));
            var summ = summFinder.GetMaxSumm(out int[] maxSummPath);

            Console.WriteLine("Path that provides max summ:");
            Console.WriteLine(string.Join(", ", maxSummPath));

            Console.WriteLine($"Max summ is {summ}.");

            Console.ReadKey();
        }
    }
}
