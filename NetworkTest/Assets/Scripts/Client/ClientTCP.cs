using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using UnityEngine;

public class ClientTCP : MonoBehaviour {

    public string IP_ADDRESS;
    public int PORT;

    public static Socket _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private byte[] _asynchbuffer = new byte[1204];

	// Use this for initialization
	void Start () 
    {
        Debug.Log("Connecting to server ....");
        _clientSocket.BeginConnect(IP_ADDRESS, PORT, new AsyncCallback(ConnectCallback), _clientSocket);
	}
	
    public  void ConnectCallback(IAsyncResult ar)
    {
        _clientSocket.EndConnect(ar);
        while (true)
        {
            OnReceive();
        }
    }

    private  void OnReceive()
    {
        byte[] _sizeinfo = new byte[4];
        byte[] _receivedbuffer = new byte[1024];

        int totalread, currentread;

        try
        {
            currentread = totalread = _clientSocket.Receive(_sizeinfo);
            if (totalread <= 0)
            {
                Console.WriteLine("You are not connected to the server");
            }
            else
            {
                while (totalread < _sizeinfo.Length && currentread > 0)
                {
                    currentread = _clientSocket.Receive(_sizeinfo, totalread, _sizeinfo.Length - totalread, SocketFlags.None);
                    totalread += currentread;
                }
                int messagesize = 0;
                messagesize |= _sizeinfo[0];
                messagesize |= _sizeinfo[1] << 8;
                messagesize |= _sizeinfo[2] << 16;
                messagesize |= _sizeinfo[3] << 24;

                byte[] data = new byte[messagesize];

                totalread = 0;
                currentread = totalread = _clientSocket.Receive(data, totalread, data.Length - totalread, SocketFlags.None);
                while (totalread < messagesize && currentread > 0)
                {
                    currentread = _clientSocket.Receive(data, totalread, data.Length - totalread, SocketFlags.None);
                    totalread += currentread;
                }
                ClientHandleNetworkData.HandleNetworkInfromation(data);

            }
        }
        catch
        {
            Console.WriteLine("You are not connected to server.");
        }
    }

    public static void SendData(byte[] data)
    {
        _clientSocket.Send(data);
    }

    public static void ThankYouServer()
    {
        PacketBuffer buffer = new PacketBuffer();
        buffer.WriteInteger((int)ClientPackets.CThankYou);
        buffer.WriteString("Hello, thank you for letting me connect to server");
        SendData(buffer.ToArray());
        buffer.Dispose();
    }

    public static void SendImage()
    {
        string imgPath = Application.dataPath + "/Initial/test.png";
        //Debug.Log(imgPath);
        byte[] imageData = File.ReadAllBytes(imgPath);
        PacketBuffer buffer = new PacketBuffer();
        buffer.WriteInteger((int)ClientPackets.CThankYou);
        buffer.WriteByte(imageData);
        SendData(buffer.ToArray());
        buffer.Dispose();
                                    
    }


}
