using System;
using System.Runtime.InteropServices;

namespace EasyStrategems
{
    internal class Program
    {

        static async Task Main(string[] args)
        {
            var transformer = new StratagemTransformer();
            transformer.StartTransform();
            while (Console.ReadKey(true).Key != ConsoleKey.F2)
            {
                continue;
            }
            transformer.EndTransform();
        }
    }
}