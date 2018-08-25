using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class ServerTCP : MonoBehaviour
{

    class ServerTCP1
    {
        private static Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static byte[] _buffer = new byte[1023];

        public static Client[] _clients = new Client[Constants.MAX_PLAYERS];

        public static void SetupServer()
        {
            for (int i = 0; i < Constants.MAX_PLAYERS; i++)
            {
                _clients[i] = new Client();
            }
            _serverSocket.Bind(new IPEndPoint(IPAddress.Any, 5155));
            _serverSocket.Listen(10);
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        private static void AcceptCallback(IAsyncResult ar)
        {
            Socket socket = _serverSocket.EndAccept(ar);
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);

            for (int i = 0; i < Constants.MAX_PLAYERS; i++)
            {
                if (_clients[i].socket == null)
                {
                    _clients[i].socket = socket;
                    _clients[i].ip = socket.RemoteEndPoint.ToString();
                    _clients[i].index = i;
                    _clients[i].StartClient();
                    Console.WriteLine("Connection from '{0}'recived", _clients[i].ip);
                    SendConnectionOk(i);
                    return;
                }
            }
        }
        private static void SendDataTo(int index, byte[] data)
        {
            byte[] sizeinfo = new byte[4];
            sizeinfo[0] = (byte)data.Length;
            sizeinfo[1] = (byte)(data.Length >> 8);
            sizeinfo[2] = (byte)(data.Length >> 16);
            sizeinfo[3] = (byte)(data.Length >> 24);

            _clients[index].socket.Send(sizeinfo);
            _clients[index].socket.Send(data);
        }
        public static void SendConnectionOk(int index)
        {
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteInteger((int)ServerPackets.SConnectionOk);
            buffer.WriteString("You are successfully connected to server");
            SendDataTo(index, buffer.ToArray());
            buffer.Dispose();
        }

    }

    class Client
    {
        public int index;
        public string ip;
        public Socket socket;
        private bool closing = false;
        private byte[] _buffer = new byte[1024];

        public void StartClient()
        {
            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
        }
        public void ReceiveCallback(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;

            try
            {
                int receive = socket.EndReceive(ar);
                if (receive <= 0)
                {
                    CloseClient(index);
                }
                else
                {
                    byte[] databuffer = new byte[receive];
                    Array.Copy(_buffer, databuffer, receive);
                    ServerHandleNetworkData.HandleNetworkInfromation(index, databuffer);
                    socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
                }
            }
            catch
            {
                CloseClient(index);
            }
        }

        private void CloseClient(int index)
        {
            closing = true;
            Console.WriteLine("Connection from {0} has been terminated", ip);

            socket.Close();
        }
    }
}

	

