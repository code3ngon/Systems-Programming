using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace tutorial_4
{
    internal class Question_2
    {
        private static readonly Random _random = new Random();

        public static void DemonstrateTaskCoordination()
        {
            Console.WriteLine("=== Task Coordination & Synchronization Demo ===\n");

            // Chạy 3 cách đồng bộ khác nhau
            RunWithWhenAll();
            RunWithCountdownEvent();
            RunWithManualResetEventSlim();

            Console.WriteLine("\nAll demonstrations completed.\n");
        }

        private static void RunWithWhenAll()
        {
            Console.WriteLine("1. Using Task.WhenAll (recommended for most cases)");
            var stopwatch = Stopwatch.StartNew();

            Task task1 = SimulateWorkAsync("Task A", 1200);
            Task task2 = SimulateWorkAsync("Task B", 800);
            Task task3 = SimulateWorkAsync("Task C", 1500);

            // Cách idiomatic nhất
            Task.WhenAll(task1, task2, task3)
                .ContinueWith(_ =>
                {
                    stopwatch.Stop();
                    Console.WriteLine($"   → All tasks completed in {stopwatch.ElapsedMilliseconds} ms");
                    Console.WriteLine("   → Now we can safely continue main logic\n");
                })
                .GetAwaiter().GetResult(); // Dùng cho Main sync
        }

        private static void RunWithCountdownEvent()
        {
            Console.WriteLine("2. Using CountdownEvent");
            var stopwatch = Stopwatch.StartNew();

            var cde = new CountdownEvent(3);

            Task.Run(() => { SimulateWork("Task X", 1100); cde.Signal(); });
            Task.Run(() => { SimulateWork("Task Y", 900); cde.Signal(); });
            Task.Run(() => { SimulateWork("Task Z", 1400); cde.Signal(); });

            cde.Wait();
            stopwatch.Stop();

            Console.WriteLine($"   → All tasks completed in {stopwatch.ElapsedMilliseconds} ms");
            Console.WriteLine("   → Now we can safely continue main logic\n");
        }

        private static void RunWithManualResetEventSlim()
        {
            Console.WriteLine("3. Using ManualResetEventSlim (low-level)");
            var stopwatch = Stopwatch.StartNew();

            var mres = new ManualResetEventSlim(initialState: false);
            int remaining = 3;

            Task.Run(() =>
            {
                SimulateWork("Task P", 1300);
                if (Interlocked.Decrement(ref remaining) == 0)
                    mres.Set();
            });

            Task.Run(() =>
            {
                SimulateWork("Task Q", 700);
                if (Interlocked.Decrement(ref remaining) == 0)
                    mres.Set();
            });

            Task.Run(() =>
            {
                SimulateWork("Task R", 1600);
                if (Interlocked.Decrement(ref remaining) == 0)
                    mres.Set();
            });

            mres.Wait();
            stopwatch.Stop();

            Console.WriteLine($"   → All tasks completed in {stopwatch.ElapsedMilliseconds} ms");
            Console.WriteLine("   → Now we can safely continue main logic\n");

            mres.Dispose();
        }

        // Helpers – đã sửa random
        private static async Task SimulateWorkAsync(string name, int baseMs)
        {
            int delay = baseMs + _random.Next(-300, 400);
            Console.WriteLine($"   {name} starting... (will take ~{delay} ms)");
            await Task.Delay(delay);
            Console.WriteLine($"   {name} finished.");
        }

        private static void SimulateWork(string name, int baseMs)
        {
            int delay = baseMs + _random.Next(-300, 400);
            Console.WriteLine($"   {name} starting... (will take ~{delay} ms)");
            Thread.Sleep(delay);
            Console.WriteLine($"   {name} finished.");
        }
    }
}
