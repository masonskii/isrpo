using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace ServerTCP
{
    class ClientHandler
    {
        public TcpClient clientSocket;

        public void RunClient()
        {           
            StreamReader streamReader = new StreamReader(clientSocket.GetStream()); // реализует объект, который считывает символы из потока данных
            NetworkStream networkStream = clientSocket.GetStream(); // предоставление основного потока данных
            string returnData = streamReader.ReadLine();
            string name = returnData;
            Console.WriteLine("- Пользователь \"" + name + "\" присоединился на сервер!!");
            while (true)
            {
                returnData = streamReader.ReadLine();
                if (returnData.IndexOf("exit") > -1)
                {
                    Console.WriteLine("- Пользователь \"" + name + "\" покинул сервер! -");
                    break;
                }
                Console.WriteLine(name + ": " + returnData);
                returnData += "\r\n";

                byte[] dataWrite = Encoding.UTF8.GetBytes(returnData);
                networkStream.Write(dataWrite, 0, dataWrite.Length);

            }
            clientSocket.Close();
        }
    }
}
