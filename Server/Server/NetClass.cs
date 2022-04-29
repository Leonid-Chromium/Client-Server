using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            IPEndPoint tcpEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); // создаем сокет

            try
            {                
                listenSocket.Bind(tcpEndPoint); // связываем сокет с локальной точкой, по которой будем принимать данные        
                listenSocket.Listen(10); // начинаем прослушивание

                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    Socket server = listenSocket.Accept();                                    
                    var buffer = new byte[256]; // буфер для получаемых данных
                    var size = 0; // количество полученных байтов
                    var data = new StringBuilder();

                    do
                    {
                        size = server.Receive(buffer);
                        data.Append(Encoding.UTF8.GetString(buffer, 0, size));
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

                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                Console.Write("Введите сообщение:");
                string message = Console.ReadLine();
                var data = Encoding.UTF8.GetBytes(message);

                socket.Connect(ipPoint); // подключаемся к удаленному хосту
                socket.Send(data);

                // получаем ответ
                var buffer = new byte[256]; // буфер для ответа
                var size = 0; // количество полученных байт
                var answer = new StringBuilder(); 

                do
                {
                    size = socket.Receive(buffer);
                    answer.Append(Encoding.UTF8.GetString(buffer, 0, size));
                }
                while (socket.Available > 0);

                Console.WriteLine("Ответ сервера: " + answer.ToString());

                // закрываем сокет
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
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
