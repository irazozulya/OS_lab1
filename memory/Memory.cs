using System;

namespace memory
{
    public class Memory
    {
        protected char[] memArray;// memory array

        public Memory()
        {
            memArray = new char[100];
            memArray[0] = memArray[1] = memArray[2] = Convert.ToChar(0);
            memArray[3] = Convert.ToChar(96);

        }

        public unsafe char* MemAlloc(int size)//memory allocation
        {
            int curr = 0;
            bool end = false;

            while(!end)
            {
                if (memArray[curr] == Convert.ToChar(0))
                {
                    if (memArray[curr + 3] >= size)
                    {
                        memArray[curr] = Convert.ToChar(1);

                        if (memArray[curr + 3] >= size + 5)
                        {
                            int next = curr + 4 + size;
                            memArray[next] = memArray[next + 1] = memArray[next + 2] = memArray[next + 3] = Convert.ToChar(0);
                            int temp = (int)memArray[curr + 3] - size - 4;
                            memArray[next + 3] = (char)temp;
                        } else
                        {
                            size = memArray[curr + 3];
                        }

                        memArray[curr + 1] = memArray[curr + 2] = memArray[curr + 3] = Convert.ToChar(0);
                        memArray[curr + 3] = (char) size;
                        end = true;

                        fixed (char* ret = &memArray[curr + 4])
                        {
                            return ret;
                        }
                    } else
                    {
                        curr += memArray[curr + 3] + 4;
                    }
                } else if (memArray[curr] == Convert.ToChar(1))
                {
                    curr += memArray[curr + 3] + 4;
                }
            }

            return null;
        }

        public unsafe char* MemRealloc(char* addr, int size)// memory reallocation
        {
            char[] reserve = memArray;

            if(addr == null)
            {
                return MemAlloc(size);
            } else
            {
                fixed (char* a = &memArray[0])
                {
                    long add = Convert.ToInt64(Convert.ToString((uint)addr));
                    long firstadd = Convert.ToInt64(Convert.ToString((uint)a));
                    int curr = Convert.ToInt32(add - firstadd) / 2 - 4;
                    int curr_size = memArray[curr + 3];

                    if (curr_size != size)
                    {
                        char[] temp = new char[curr_size];
                        for(int i = 0; i < curr_size; i++)
                        {
                            temp[i] = memArray[curr + 4 + i];
                        }
                        MemFree(addr);
                        char* next_addr = MemAlloc(size);
                        if (next_addr != null)
                        {
                            long next_add = Convert.ToInt64(Convert.ToString((uint)next_addr));
                            int next = Convert.ToInt32(next_add - firstadd) / 2;
                            for (int i = 0; i < Math.Min(curr_size, size); i++)
                            {
                                memArray[next + i] = temp[i];
                            }
                            return next_addr;
                        } else
                        {
                            memArray = reserve;
                            return null;
                        }
                    }
                    else
                    {
                        return addr;
                    }
                }
            }
        }

        protected void CheckMerge()//checking if there is free cells next to each other and merging them
        {
            int curr = 0;

            while (curr < memArray.Length)
            {
                if (memArray[curr] == Convert.ToChar(0))
                {
                    int size = memArray[curr + 3];
                    int next = curr + 4 + size;
                    if (next < memArray.Length)
                    {
                        if (memArray[next] == Convert.ToChar(0))
                        {
                            int size_next = memArray[next + 3];
                            memArray[next] = memArray[next + 1] = memArray[next + 2] = memArray[next + 3] = Convert.ToChar(0);

                            memArray[curr + 3] = (char)(size + size_next + 4);
                        }
                        else
                        {
                            curr += memArray[curr + 3] + 4;
                        }
                    } else
                    {
                        curr += memArray[curr + 3] + 4;
                    }
                }
                else if (memArray[curr] == Convert.ToChar(1))
                {
                    curr += memArray[curr + 3] + 4;
                }
            }
        }
        

            public unsafe void MemFree(char* addr)//clearing the heading
        {
            fixed (char* a = &memArray[0])
            {
                long add = Convert.ToInt64(Convert.ToString((uint)addr));
                long firstadd = Convert.ToInt64(Convert.ToString((uint)a));
                int curr = Convert.ToInt32(add - firstadd) / 2 - 4;

                memArray[curr] = Convert.ToChar(0);

                CheckMerge();
            }
        }

        public void ShowMem()//outputting the memory array
        {
            foreach (int i in memArray)
            {
                Console.Write(i + " | ");
            }

            Console.Write("\n");
        }

    }
}
