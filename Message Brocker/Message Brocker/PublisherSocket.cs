using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Publisher
{
    class PublisherSocket
    {
        private Socket _socket;
        public bool isConnected;
        public PublisherSocket()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Connect(string ipAddress, int port)
        {
            _socket.BeginConnect(new IPEndPoint(IPAddress.Parse(ipAddress), port), ConnectedCallback, null);
            Thread.Sleep(2000);
        }

        public void Send(byte[] data)
        {
            try
            {
                _socket.Send(data);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error ocurred {e.Message}");
            }
        }

        private void ConnectedCallback(IAsyncResult asyncResult)
        {
            if(_socket.Connected)
            {
                Console.WriteLine("Sender connected to Broker");
            }
            else
            {
                Console.WriteLine("Error: Not connected");
            }

            isConnected = _socket.Connected;
        }
    }
}