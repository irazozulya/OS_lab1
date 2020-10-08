using System;

namespace memory
{
    public class Memory
    {
        protected char[] memArray;// memory array

        public Memory()
        {
            memArray = new char[100];
            memArray[0] = memArray[1] = Convert.ToChar(0);
            memArray[2] = Convert.ToChar(9);
            memArray[3] = Convert.ToChar(6);

        }

        protected int GetSize (int curr)// getting size from the heading
        {
            int size;
            if (memArray[curr + 1] == '\0')
            {
                if(memArray[curr + 2] == '\0')
                {
                    size = memArray[curr + 3];
                } else
                {
                    size = memArray[curr + 2] * 10 + memArray[curr + 3];
                }
            } else
            {
                size = memArray[curr + 1] * 100 + memArray[curr + 2] * 10 + memArray[curr + 3];
            }

            return size;
        }

        protected void SetSize(int curr, int size)// setting size into the heading
        {
            string s = Convert.ToString(size);
            int len = s.Length;
            for (int i = 0; i < len; i++)
            {
                memArray[curr + 3 - i] = Convert.ToChar(Convert.ToUInt32(char.GetNumericValue(s, len - 1 - i)));
            }
        }

        public unsafe char* MemAlloc(int size)//memory allocation
        {
            int curr = 0;
            bool end = false;

            while(!end)
            {
                if (memArray[curr] == Convert.ToChar(0))
                {
                    if (GetSize(curr) >= size)
                    {
                        memArray[curr] = Convert.ToChar(1);

                        if (GetSize(curr) >= size + 5)
                        {
                            int next = curr + 4 + size;
                            memArray[next] = memArray[next + 1] = memArray[next + 2] = memArray[next + 3] = Convert.ToChar(0);
                            SetSize(next, GetSize(curr) - size - 4);
                        }

                        memArray[curr + 1] = memArray[curr + 2] = memArray[curr + 3] = Convert.ToChar(0);
                        SetSize(curr, size);
                        end = true;

                        fixed (char* ret = &memArray[curr + 4])
                        {
                            return ret;
                        }
                    } else
                    {
                        curr += GetSize(curr) + 4;
                    }
                } else if (memArray[curr] == Convert.ToChar(1))
                {
                    curr += GetSize(curr) + 4;
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
                    int curr_size = GetSize(curr);

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
                    int size = GetSize(curr);
                    int next = curr + 4 + size;
                    if (next < memArray.Length)
                    {
                        if (memArray[next] == Convert.ToChar(0))
                        {
                            int size_next = GetSize(next);
                            memArray[next] = memArray[next + 1] = memArray[next + 2] = memArray[next + 3] = Convert.ToChar(0);

                            SetSize(curr, size + size_next + 4);
                        }
                        else
                        {
                            curr += GetSize(curr) + 4;
                        }
                    } else
                    {
                        curr += GetSize(curr) + 4;
                    }
                }
                else if (memArray[curr] == Convert.ToChar(1))
                {
                    curr += GetSize(curr) + 4;
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
