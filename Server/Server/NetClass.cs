using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace Server
{
    class NetClass
    {
        const string ip = "192.168.1.117";
        const int port = 7373; // Порт для приёма

        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        public static void Server()
        {
            // получаем адреса для запуска сокета
           IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(ip), port);

            // создаем сокет
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                // связываем сокет с локальной точкой, по которой будем принимать данные
                listenSocket.Bind(ipPoint);

                // начинаем прослушивание
                listenSocket.Listen(10);

                Console.Clear();
                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    Socket server = listenSocket.Accept();                                    
                    var buffer = new byte[256]; // буфер для получаемых данных
                    var size = 0; // количество полученных байтов
                    var data = new StringBuilder();

                    do
                    {
                        if (server.Available != 0)
                        {
                            size = server.Receive(buffer);
                            data.Append(Encoding.UTF8.GetString(buffer, 0, size));
                        }
                        else
                        {
                            data.Append("Кто-то хочет сломать сервер");
                        }
                    }
                    while (server.Available > 0);

                    Console.WriteLine(DateTime.Now.ToShortTimeString() + ": " + data);
                    server.Send(Encoding.UTF8.GetBytes("Ваше сообщение доставлено")); // отправляем ответ

                    server.Shutdown(SocketShutdown.Both); // закрываем сокет
                    server.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void Client()
        {
            try
            {
                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(ip), port);

                Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                Console.Write("Введите сообщение:");
                string message = Console.ReadLine();
                var data = Encoding.UTF8.GetBytes(message);

                client.Connect(ipPoint); // подключаемся к удаленному хосту
                client.Send(data);

                // получаем ответ
                var buffer = new byte[256]; // буфер для ответа
                var size = 0; // количество полученных байт
                var answer = new StringBuilder(); 

                do
                {
                    
                    size = client.Receive(buffer);
                    answer.Append(Encoding.UTF8.GetString(buffer, 0, size));
                }
                while (client.Available > 0);

                Console.WriteLine("Ответ сервера: " + answer.ToString());

                // закрываем сокет
                client.Shutdown(SocketShutdown.Both);
                client.Close();
                Client();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.Read();
        }
    }
}
