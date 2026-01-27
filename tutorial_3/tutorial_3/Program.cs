using System;
using System.Threading.Tasks;

namespace tutorial_3
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Question 1===");
            // Run Question 1 demo
            Question_1 q1 = new Question_1();
            q1.Run();

            Console.WriteLine("=== Question 2===");
            // Run Question 2 demo
            Question_2 q2 = new Question_2();
            q2.Run();

            Console.WriteLine("=== Question 3===");
            // Run Question 3 demo
            Question_3 q3 = new Question_3();
            q3.Run_Before();
            q3.Run_After();

            // Run Question 4 demo
            Console.WriteLine("=== Question 4===");
            Question_4 q4 = new Question_4();

            q4.Run_WithThread();
            q4.Run_WithThreadPool();
            q4.Run_WithTask();

            // Run Question 5 demo
            Question_5 q5 = new Question_5();

            await q5.RunAsync();
            Console.WriteLine("All demos completed.");
            Console.ReadLine();
        }
    }
}
