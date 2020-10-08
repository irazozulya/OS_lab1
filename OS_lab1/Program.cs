using System;
using memory;

namespace OS_lab1
{
    class Program
    {
        static void Main(string[] args)
        {
            unsafe
            {
                Memory mem = new Memory();
                mem.ShowMem();

                char* first = mem.MemAlloc(7);
                Console.WriteLine((uint)first);
                mem.ShowMem();

                char* second = mem.MemAlloc(8);
                Console.WriteLine((uint)second);
                mem.ShowMem();

                char* third = mem.MemAlloc(10);
                Console.WriteLine((uint)third);
                mem.ShowMem();

                Console.Write("\n");
                mem.MemFree(second);
                mem.ShowMem();

                Console.Write("\n");
                mem.MemFree(third);
                mem.ShowMem();

                Console.Write("\n");
                second = mem.MemAlloc(13);
                mem.ShowMem();

                Console.Write("\n");
                mem.MemAlloc(6);
                mem.ShowMem();

                Console.Write("\n");
                mem.MemRealloc(second, 5);
                mem.ShowMem();
            }
        }
    }
}
