using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NET_hw5_SwitchOffComp_MulticastGroup_Udp_CONS
{
    class Program
    {
        static IPAddress remoteAddress;
        const int remotePort = 3535;
        const int localPort = 3535;

        static void Main(string[] args)
        {
            try
            {
                remoteAddress = IPAddress.Parse("235.5.5.11");

                Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
                receiveThread.Start();

                SendMessage();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private static void SendMessage()
        {
            //отправляем данные
            UdpClient sender = new UdpClient();
            IPEndPoint endPoint = new IPEndPoint(remoteAddress, remotePort);

            try
            {
                while (true)
                {
                    Console.Write("Выключить компьютер (да/нет): ");
                    string message = Console.ReadLine();
                    
                    byte[] data = Encoding.Default.GetBytes(message);
                    sender.Send(data, data.Length, endPoint);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                sender.Close();
            }
        }
        private static void ReceiveMessage()
        {
            //получения данных
            UdpClient receiver = new UdpClient(localPort);
            receiver.JoinMulticastGroup(remoteAddress, 20);
            IPEndPoint remoteIp = null;
            string localAddress = LocalIPAddress();

            try
            {
                while (true)
                {
                    byte[] data = receiver.Receive(ref remoteIp);
                    //if (remoteIp.Address.ToString().Equals(localAddress))
                    //    continue;
                    string message = Encoding.Default.GetString(data);

                    if (message == "да")
                    {
                        System.Diagnostics.Process.Start("cmd", "/c shutdown -s -f -t 00");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                receiver.Close();
            }
        }
        private static string LocalIPAddress()
        {
            string localIP = "";
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }
    }
}
