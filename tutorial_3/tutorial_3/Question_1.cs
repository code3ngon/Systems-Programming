using System;

namespace tutorial_3
{
    struct MyStruct
    {
        public int X;
    }

    class MyClass
    {
        public int Y;
    }

    internal class Question_1
    {
        // Reference nằm trên STACK, object nằm trên HEAP
        private MyClass classField = new MyClass();

        public void Run()
        {
            // Value type -> STACK
            int localInt = 10;

            // Struct (value type) -> STACK
            MyStruct localStruct;
            localStruct.X = 20;

            // Reference -> STACK, object -> HEAP
            MyClass localClass = new MyClass();
            localClass.Y = 30;

            // Khi Run() kết thúc:
            // - localInt, localStruct bị loại khỏi STACK
            // - localClass mất reference → object eligible for GC
            // - classField vẫn tồn tại vì Question_1 còn sống

            Console.WriteLine("Question 1 demo completed.\n");
        }
    }
}
