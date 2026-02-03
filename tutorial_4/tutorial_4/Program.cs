using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tutorial_4
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Race Condition & Thread Safety Demo ===\n");

            Question_1.DemonstrateRaceConditionAndFixes();

            Question_2.DemonstrateTaskCoordination();

            Question_3.DemonstrateThreadSafeLogging();

            Question_4.DemonstrateFileMonitoringAndProcessing();

            string input = "secret.txt";
            string encrypted = "secret.enc.gz";
            string restored = "restored.txt";

            Question_5.DemonstrateSecureFileStorage(input, encrypted, restored);

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey(true);
        }
    }
}
