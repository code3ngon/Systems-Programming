using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace tutorial_3
{
    internal class Question_4
    {

        private const int WorkItemCount = 100;

        // Công việc giả lập
        private void DoWork(string label)
        {
            Console.WriteLine($"{label} - Thread ID: {Thread.CurrentThread.ManagedThreadId}");
            Thread.Sleep(10); // Giả lập xử lý
        }

        // 1️⃣ Sử dụng Thread (tạo thread thủ công)
        public void Run_WithThread()
        {
            Console.WriteLine("=== Using Thread ===");
            Stopwatch sw = Stopwatch.StartNew();

            Thread[] threads = new Thread[WorkItemCount];

            for (int i = 0; i < WorkItemCount; i++)
            {
                threads[i] = new Thread(() => DoWork("Thread"));
                threads[i].Start();
            }

            // Chờ tất cả thread kết thúc
            foreach (var t in threads)
            {
                t.Join();
            }

            sw.Stop();
            Console.WriteLine($"Thread time: {sw.ElapsedMilliseconds} ms\n");
        }

        // 2️⃣ Sử dụng ThreadPool
        public void Run_WithThreadPool()
        {
            Console.WriteLine("=== Using ThreadPool ===");
            Stopwatch sw = Stopwatch.StartNew();

            using (CountdownEvent countdown = new CountdownEvent(WorkItemCount))
            {
                for (int i = 0; i < WorkItemCount; i++)
                {
                    ThreadPool.QueueUserWorkItem(_ =>
                    {
                        DoWork("ThreadPool");
                        countdown.Signal();
                    });
                }

                countdown.Wait();
            }

            sw.Stop();
            Console.WriteLine($"ThreadPool time: {sw.ElapsedMilliseconds} ms\n");
        }

        // 3️⃣ Sử dụng Task
        public void Run_WithTask()
        {
            Console.WriteLine("=== Using Task ===");
            Stopwatch sw = Stopwatch.StartNew();

            Task[] tasks = new Task[WorkItemCount];

            for (int i = 0; i < WorkItemCount; i++)
            {
                tasks[i] = Task.Run(() => DoWork("Task"));
            }

            Task.WaitAll(tasks);

            sw.Stop();
            Console.WriteLine($"Task time: {sw.ElapsedMilliseconds} ms\n");
        }
    }
}
