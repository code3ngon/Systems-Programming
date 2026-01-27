using System;
using System.Collections.Generic;

namespace tutorial_3
{
    class LargeObject
    {
        public byte[] Data = new byte[1024 * 100]; // ~100KB
    }

    internal class Question_2
    {
        public void Run()
        {
            long before = GC.GetTotalMemory(false);
            Console.WriteLine($"Memory before allocation: {before / 1024} KB");

            List<LargeObject> objects = new List<LargeObject>();

            for (int i = 0; i < 1000; i++)
            {
                objects.Add(new LargeObject());
            }

            long afterAlloc = GC.GetTotalMemory(false);
            Console.WriteLine($"Memory after allocation: {afterAlloc / 1024} KB");

            // Remove references
            objects = null;

            // Force GC (for observation only)
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            long afterGC = GC.GetTotalMemory(true);
            Console.WriteLine($"Memory after GC: {afterGC / 1024} KB");

            Console.WriteLine("Question 2 demo completed.\n");
        }
    }
}
