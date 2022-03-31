using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

namespace Lab1_interforum
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        private bool done = true; // Флаг остановки слушающего потока
        private UdpClient client; // Сокет клиента
        private IPAddress groupAddress; // Групповой адрес рассылки
        private int localPort; // Локальный порт для приема сообщений
        private int remotePort; // Удаленный порт для отправки сообщений
        private int ttl;

        private IPEndPoint remoteEP;
        private UnicodeEncoding encoding = new UnicodeEncoding();

        private string name; // имя пользователя в разговоре
        private string message; // сообщение для отправки

        private readonly SynchronizationContext _syncContext;

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                //Считываем конфигурационный файл приложения
                NameValueCollection configuration = ConfigurationSettings.AppSettings;
                groupAddress = IPAddress.Parse(configuration["GroupAddress"]);
                localPort = int.Parse(configuration["LocalPort"]);
                remotePort = int.Parse(configuration["RemotePort"]);
                ttl = int.Parse(configuration["TTL"]);
            }
            catch
            {
                MessageBox.Show(this, "Ошибка в файле конфигурации приложения!",
                "Error Multicast Chart",
                MessageBoxButton.OK, MessageBoxImage.Error);
            }
            _syncContext = SynchronizationContext.Current;
        }

        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            name = textName.Text;
            textName.IsReadOnly = true;
            try
            {
                // присоединяемся к группе рассылки
                client = new UdpClient(localPort);
                client.JoinMulticastGroup(groupAddress, ttl);

                remoteEP = new IPEndPoint(groupAddress, remotePort);

                Thread receiver = new Thread(new ThreadStart(Listener));
                receiver.IsBackground = true;
                receiver.Start();

                byte[] data = encoding.GetBytes(name + "присоединился к чату");
                client.Send(data, data.Length, remoteEP);

                buttonStart.IsEnabled = false;
                buttonStop.IsEnabled = true;
                buttonSend.IsEnabled = true;
            }
            catch (SocketException ex)
            {
                MessageBox.Show(this, ex.Message, "Error Multicast Chart",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Listener()
        {
            done = false;
            try
            {
                while (!done)
                {
                    if (done)
                    {
                        byte[] data = encoding.GetBytes(name + " отключился");
                        return;
                    }
                        
                    IPEndPoint ep = null;
                    byte[] buffer = client.Receive(ref ep);
                    message = encoding.GetString(buffer);

                    _syncContext.Post(o => DisplayReceivedMessage(), null);
                }
            }
            catch (Exception ex)
            {
                if (done)
                    return;
                else
                    MessageBox.Show(this, ex.Message, "Error MulticastChat",
                        MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DisplayReceivedMessage()
        {
            string time = DateTime.Now.ToString("t");
            textMessages.Text = time + " " + message + "\r\n" + textMessages.Text;
        }

        private void buttonSend_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Отправляем сообщение группе
                byte[] data = encoding.GetBytes(name + ": " + textMessage.Text);
                client.Send(data, data.Length, remoteEP);
                textMessage.Clear();
                textMessage.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error MulticastChat",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void buttonStop_Click(object sender, RoutedEventArgs e)
        {
            StopListener();
        }

        private void StopListener()
        {
            // отправляем группе сообщение о выходе
            byte[] data = encoding.GetBytes(name + " has left the chart");
            client.Send(data, data.Length, remoteEP);
            //покидаем группу
            client.DropMulticastGroup(groupAddress);
            client.Close();
            //останавливаем поток, получающий сообщения
            done = true;
            buttonStart.IsEnabled = true;
            buttonStop.IsEnabled = false;
            buttonSend.IsEnabled = false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!done)
            {
                StopListener();
            }
        }
    }
}