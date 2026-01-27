using System;
using System.Threading;
using System.Threading.Tasks;

namespace tutorial_3
{
    internal class Question_5
    {
        // Async method mô phỏng tác vụ dài
        public async Task LongRunningOperationAsync()
        {
            Console.WriteLine($"Start async work - Thread ID: {Thread.CurrentThread.ManagedThreadId}");

            // Không block thread, chỉ đăng ký continuation
            await Task.Delay(2000);

            Console.WriteLine($"End async work - Thread ID: {Thread.CurrentThread.ManagedThreadId}");
        }

        // KHÔNG NÊN DÙNG
        public async void BadAsyncVoid()
        {
            await Task.Delay(1000);
            throw new Exception("Exception in async void");
        }

        // Gây block thread
        public void BlockingCall()
        {
            Console.WriteLine("Blocking call started");

            // Block thread đang chạy
            LongRunningOperationAsync().Wait();

            Console.WriteLine("Blocking call ended");
        }

        // Demo chuẩn
        public async Task RunAsync()
        {
            Console.WriteLine("=== Question 5: Async/Await ===");

            // Dùng await
            await LongRunningOperationAsync();

            // async void – exception không bắt được
            try
            {
                BadAsyncVoid();
            }
            catch
            {
                Console.WriteLine("This will NOT catch async void exception");
            }

            // ❌ Task.Wait / Result
            BlockingCall();
        }
    }
}
