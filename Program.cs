using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Tcp
{
    class Program
    {
        static Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static void Main(string[] args)
        {
            socket.Bind(new IPEndPoint(IPAddress.Any, 904));
            socket.Listen(5);
            Socket client = socket.Accept();
            byte[] buffer = new byte[1024];
            client.Receive(buffer);
            Console.WriteLine("Кол-во пикселей на второй картинке" + Encoding.ASCII.GetString(buffer));
            Console.ReadLine();
        }
    }
}
