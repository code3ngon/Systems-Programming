using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace tutorial_4
{
    internal class Question_1
    {
        // ────────────────────────────────────────────────
        //  Shared counters
        // ────────────────────────────────────────────────
        private static int unsafeCounter = 0;
        private static int lockedCounter = 0;
        private static int interlockedCounter = 0;

        private const int INCREMENTS_PER_TASK = 1_000_000;
        private const int TASK_COUNT = 5;


        public static void DemonstrateRaceConditionAndFixes()
        {
            Console.WriteLine($"Each task will do {INCREMENTS_PER_TASK:N0} increments");
            Console.WriteLine($"Total expected = {TASK_COUNT * INCREMENTS_PER_TASK:N0}\n");

            // ────────────────────────────────────────────────
            // 1. UNSAFE version – Race condition
            // ────────────────────────────────────────────────
            unsafeCounter = 0;
            RunTest("No synchronization (race condition)",
                    () => unsafeCounter++,
                    () => unsafeCounter);   // ← reader

            // ────────────────────────────────────────────────
            // 2. Using lock
            // ────────────────────────────────────────────────
            lockedCounter = 0;
            object lockObject = new object();

            RunTest("Using lock",
                    () =>
                    {
                        lock (lockObject)
                        {
                            lockedCounter++;
                        }
                    },
                    () => lockedCounter);   // ← reader

            // ────────────────────────────────────────────────
            // 3. Using Interlocked
            // ────────────────────────────────────────────────
            interlockedCounter = 0;

            RunTest("Using Interlocked.Increment",
                    () => Interlocked.Increment(ref interlockedCounter),
                    () => interlockedCounter);   // ← reader
        }


        // Changed signature: added Func<int> to read the final value
        private static void RunTest(string title, Action incrementAction, Func<int> valueReader)
        {
            Console.WriteLine($"┌─ {title,-38} ───────────────────────────────┐");

            var sw = Stopwatch.StartNew();

            Task[] tasks = new Task[TASK_COUNT];

            for (int i = 0; i < TASK_COUNT; i++)
            {
                tasks[i] = Task.Run(() =>
                {
                    for (int j = 0; j < INCREMENTS_PER_TASK; j++)
                    {
                        incrementAction();
                    }
                });
            }

            Task.WaitAll(tasks);

            sw.Stop();

            int finalValue = valueReader();   // ← clean & safe way to get value

            long expected = (long)TASK_COUNT * INCREMENTS_PER_TASK;
            long lost = expected - finalValue;

            Console.WriteLine($"│ Final value     : {finalValue,15:N0}          │");
            Console.WriteLine($"│ Expected        : {expected,15:N0}          │");
            Console.WriteLine($"│ Lost increments : {lost,15:N0}          │");
            Console.WriteLine($"│ Time            : {sw.ElapsedMilliseconds,6} ms               │");
            Console.WriteLine("└───────────────────────────────────────────────────────┘");
            Console.WriteLine();
        }
    }
}
