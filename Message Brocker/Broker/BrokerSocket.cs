using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Broker
{
    internal class BrokerSocket
    {
        private Socket _socket;
        private const int CONNECTIONS_LIMIT = 8;
        public BrokerSocket()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Start(string ip, int port)
        {
            _socket.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
            _socket.Listen(CONNECTIONS_LIMIT);
            Accept();
        }

        private void Accept()
        {
            _socket.BeginAccept(AcceptedCallback, null);
        }

        private void AcceptedCallback(IAsyncResult asyncResult)
        {
            ConnectionInfo connectionInfo = new ConnectionInfo();

            try
            {
                connectionInfo.Socket = _socket.EndAccept(asyncResult);
                connectionInfo.Address = connectionInfo.Socket.RemoteEndPoint.ToString();
                connectionInfo.Socket.BeginReceive(
                    connectionInfo.Data, 0, connectionInfo.Data.Length, 
                    SocketFlags.None, ReceiveCallBack, connectionInfo);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Can't accept {e.Message}");
            }
            finally
            {
                Accept();
            }
        }

        private void ReceiveCallBack(IAsyncResult asyncResult)
        {
            ConnectionInfo connection = asyncResult.AsyncState as ConnectionInfo;

            try
            {
                Socket senderSocket = connection.Socket;
                SocketError response;
                int buffSize = senderSocket.EndReceive(asyncResult, out response);

                if(response == SocketError.Success)
                {
                    byte[] payload = new byte[buffSize];
                    Array.Copy(connection.Data, payload, payload.Length);

                    PayloadHandler.Handle(payload, connection);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine($"Can't receive data {e.Message}");
            }
            finally
            {
                try
                {
                    connection.Socket.BeginReceive(connection.Data, 0, connection.Data.Length,
                        SocketFlags.None, ReceiveCallBack, connection);
                }
                catch(Exception e)
                {
                    Console.WriteLine($"{e.Message}");
                    var address = connection.Socket.RemoteEndPoint.ToString();

                    connection.Socket.Close();
                    //stergem din storage
                }
            }
        }
    }
}
