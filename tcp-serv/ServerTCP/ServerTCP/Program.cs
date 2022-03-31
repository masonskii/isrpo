using System;
using System.Net.Sockets;
using System.Threading;

namespace ServerTCP
{
    class Program
    {
        const int ECHO_PORT = 8080;
        public static int nClients = 0;

        static void Main(string[] args)
        {
            try
            {
            #pragma warning disable CS0618 // Тип или член устарел
                TcpListener tcpListener = new TcpListener(ECHO_PORT); //ожидание подключения от TCP-клиентов
            #pragma warning restore CS0618 // Тип или член устарел
                tcpListener.Start();
                Console.WriteLine(" Ожидание присоединения пользователей...");
                while (nClients < 3)
                {
                    TcpClient client = tcpListener.AcceptTcpClient(); // предоставление клиентских подключений для TCP
                    ClientHandler clientHandler = new ClientHandler();
                    clientHandler.clientSocket = client;
                    Thread clientThread = new Thread(new ThreadStart(clientHandler.RunClient)); //создает и контролирует поток, задает приоритет и возвращает статус
                    clientThread.Start();
                    nClients++;
                }
                tcpListener.Stop();
            }
            catch (Exception e)
            {
                Console.WriteLine("Исключение: " + e);
            }
        }
    }
}
