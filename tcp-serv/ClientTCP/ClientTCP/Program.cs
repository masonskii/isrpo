using System;
using System.Text;
using System.Net.Sockets;
using System.IO;

namespace ClientTCP
{
    class Program
    {
        const int ECHO_PORT = 8080;
      
        static void Main(string[] args)
        {
            
            Console.Write("  Введите ваше имя: ");
            string name = Console.ReadLine();
            Console.WriteLine("---Успешный вход!---\n Напишите | exit | чтобы покинуть сервер");
            try
            {
                TcpClient eClient = new TcpClient("127.0.0.1", ECHO_PORT);
                StreamReader readerStream = new StreamReader(eClient.GetStream());
                NetworkStream writerStream = eClient.GetStream();
                string dataToSend;
                dataToSend = name;
                dataToSend += "\r\n";
                byte[] data = Encoding.UTF8.GetBytes(dataToSend);
                writerStream.Write(data, 0, data.Length);
                while (true)
                {
                    Console.Write(name + ": ");
                    dataToSend = Console.ReadLine();
                    dataToSend += "\r\n";
                    data = Encoding.UTF8.GetBytes(dataToSend);
                    writerStream.Write(data, 0, data.Length);
                    if (dataToSend.IndexOf("exit") > -1)
                        break;
                    string returnData;
                    returnData = readerStream.ReadLine();
                    Console.WriteLine(" $Сервер: " + returnData);

                }
                eClient.Close();
            }
            catch (Exception e) { Console.WriteLine("Исключение: " + e.Message); }
        }
    }
}
