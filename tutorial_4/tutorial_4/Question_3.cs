using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace tutorial_4
{
    internal class Question_3
    {
        private const string LogFilePath = "app-log.txt";
        private const int MessagesPerTask = 200;
        private const int TaskCount = 5;

        public static void DemonstrateThreadSafeLogging()
        {
            Console.WriteLine("=== Thread-Safe File Logging Demo ===\n");
            Console.WriteLine($"Each of {TaskCount} tasks writes {MessagesPerTask} lines\n");

            // Clean previous file
            if (File.Exists(LogFilePath)) File.Delete(LogFilePath);

            // 1. UNSAFE version
            Console.WriteLine("1. No synchronization (race condition expected)");
            RunUnsafeVersion();
            Console.WriteLine(" → Check file size and content for corruption / missing lines\n");

            File.Delete(LogFilePath);

            // 2. Using lock
            Console.WriteLine("2. Using lock around File.AppendAllText");
            RunWithLock();
            Console.WriteLine(" → File should be correct\n");

            File.Delete(LogFilePath);

            // 3. Dedicated logging task + queue
            Console.WriteLine("3. Using dedicated logging task + BlockingCollection queue");
            RunWithLoggingQueue();
            Console.WriteLine(" → File should be correct, cleanest separation\n");
        }

        private static void RunUnsafeVersion()
        {
            var sw = Stopwatch.StartNew();
            Task[] tasks = new Task[TaskCount];
            for (int i = 0; i < TaskCount; i++)
            {
                int taskId = i + 1;
                tasks[i] = Task.Run(() =>
                {
                    for (int j = 1; j <= MessagesPerTask; j++)
                    {
                        string msg = $"[{DateTime.Now:HH:mm:ss.fff}] Task-{taskId,-2} line {j,4}";
                        try
                        {
                            File.AppendAllText(LogFilePath, msg + Environment.NewLine);
                        }
                        catch { /* ignore for demo */ }
                    }
                });
            }
            Task.WaitAll(tasks);
            sw.Stop();
            ReportResult(sw.ElapsedMilliseconds);
        }

        private static readonly object _fileLock = new object();

        private static void RunWithLock()
        {
            var sw = Stopwatch.StartNew();
            Task[] tasks = new Task[TaskCount];
            for (int i = 0; i < TaskCount; i++)
            {
                int taskId = i + 1;
                tasks[i] = Task.Run(() =>
                {
                    for (int j = 1; j <= MessagesPerTask; j++)
                    {
                        string msg = $"[{DateTime.Now:HH:mm:ss.fff}] Task-{taskId,-2} line {j,4}";
                        lock (_fileLock)
                        {
                            File.AppendAllText(LogFilePath, msg + Environment.NewLine);
                        }
                    }
                });
            }
            Task.WaitAll(tasks);
            sw.Stop();
            ReportResult(sw.ElapsedMilliseconds);
        }

        private static void RunWithLoggingQueue()
        {
            var queue = new BlockingCollection<string>(boundedCapacity: 500);
            var cts = new CancellationTokenSource();

            // Dedicated background logger – sửa using declaration thành using block
            var loggerTask = Task.Run(() =>
            {
                using (var writer = new StreamWriter(LogFilePath, append: true))
                {
                    writer.AutoFlush = true;

                    foreach (var message in queue.GetConsumingEnumerable(cts.Token))
                    {
                        writer.WriteLine(message);
                    }
                }
            }, cts.Token);

            var sw = Stopwatch.StartNew();

            Task[] workers = new Task[TaskCount];
            for (int i = 0; i < TaskCount; i++)
            {
                int taskId = i + 1;
                workers[i] = Task.Run(() =>
                {
                    for (int j = 1; j <= MessagesPerTask; j++)
                    {
                        string msg = $"[{DateTime.Now:HH:mm:ss.fff}] Task-{taskId,-2} line {j,4}";
                        queue.Add(msg);
                    }
                });
            }

            Task.WaitAll(workers);
            queue.CompleteAdding();           // báo hiệu không còn message nào nữa
            loggerTask.Wait();                // đợi logger ghi hết
            sw.Stop();

            cts.Dispose();

            ReportResult(sw.ElapsedMilliseconds);
        }

        private static void ReportResult(long ms)
        {
            if (!File.Exists(LogFilePath))
            {
                Console.WriteLine(" → File was not created!");
                return;
            }

            int lineCount = File.ReadAllLines(LogFilePath).Length;
            int expected = TaskCount * MessagesPerTask;

            Console.WriteLine($" → Wrote {lineCount} lines (expected {expected})");
            Console.WriteLine($" → Time: {ms} ms");
            Console.WriteLine($" → File size: {new FileInfo(LogFilePath).Length / 1024.0:F1} KB\n");
        }
    }
}