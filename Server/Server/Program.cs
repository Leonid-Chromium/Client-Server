using System;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Console.WriteLine("1\tServer\n2\tClient");
            int i = Convert.ToInt32(Console.ReadLine());
            switch(i)
            {
                case 1:
                    while(true)
                        NetClass.Server();

                case 2:
                    while (true)
                        NetClass.Client();

                default:
                    Console.WriteLine("Неправильный вариант");
                    break;
            }
        }
    }
}
