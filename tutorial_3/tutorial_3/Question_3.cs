using System;
using System.Collections.Generic;
using System.Linq;

namespace tutorial_3
{
    internal class Question_3
    {
        public void Run_Before()
        {
            Console.WriteLine("=== Before Optimization ===");

            List<int> numbers = new List<int>();

            // Cấp phát nhiều object List + resize liên tục
            for (int i = 0; i < 1_000_000; i++)
            {
                numbers.Add(i);
            }

            // LINQ tạo iterator + List mới
            List<int> evenSquares = numbers
                .Where(n => n % 2 == 0)
                .Select(n => n * n)
                .ToList();

            Console.WriteLine($"Result count: {evenSquares.Count}");
        }

        public void Run_After()
        {
            Console.WriteLine("=== After Optimization ===");

            // 1️⃣ Dùng array thay cho List<T> → không resize, ít allocation
            int[] numbers = new int[1_000_000];

            for (int i = 0; i < numbers.Length; i++)
            {
                numbers[i] = i;
            }

            // 2️⃣ Cấp phát mảng kết quả 1 lần
            int[] evenSquares = new int[numbers.Length / 2];
            int index = 0;

            // 3️⃣ Thay LINQ bằng vòng lặp → không tạo iterator / list tạm
            for (int i = 0; i < numbers.Length; i++)
            {
                int n = numbers[i];

                if (n % 2 == 0)
                {
                    evenSquares[index++] = n * n;
                }
            }

            Console.WriteLine($"Result count: {index}");
        }
    }
}
