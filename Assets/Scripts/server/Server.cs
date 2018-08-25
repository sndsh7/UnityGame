using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class Server : MonoBehaviour
{
    private List<ServerClient> clients;
    private List<ServerClient> disconnectList;

    public int port = 6321;
    private TcpListener server;
    private bool serverStarted;
    private static bool bd = false;

    static int ax = 600;
    static int ay = 400;
    static int xx1 = 500;
    static int xx2 = 400;

    static int ax1 = 0;
    static int ay1 = 0;

    private void Start()
    {
        clients = new List<ServerClient>();
        disconnectList = new List<ServerClient>();

        try
        {
            server = new TcpListener(IPAddress.Any, port);
            server.Start();
            StartListning();
            serverStarted = true;
            Debug.Log("Server has been Started on Port:" + port.ToString());
        }
        catch(Exception e)
        {
            Debug.Log("Server Error:" + e.Message);
        }

    }

    private void Update()
    {
        if (!serverStarted)
            return;
        foreach(ServerClient c in clients)
        {
            // Is the client is still connected ?
            if(!IsConnected(c.tcp))
            {
                c.tcp.Close();
                disconnectList.Add(c);
                continue;
            }
            //Check for message from client
            else
            {
                NetworkStream s = c.tcp.GetStream();
                if(s.DataAvailable)
                {
                    StreamReader reader = new StreamReader(s, true);
                    string data = reader.ReadLine();



                    if (data != null)
                        OnIncommingData(c,data);
                }
            }
        }
    }

    private void StartListning()
    {
        server.BeginAcceptTcpClient(AcceptTcpClient, server);
    }

    private bool IsConnected(TcpClient c)
    {

        try
        {
            if(c != null && c.Client != null && c.Client.Connected)
            {
                if(c.Client.Poll(0,SelectMode.SelectRead))
                {
                    return !(c.Client.Receive(new byte[1], SocketFlags.Peek) == 0);
                }
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }


    }

    private void OnIncommingData(ServerClient c, string data)
    {
        Debug.Log("Data : " + data);


        string[] temp = data.Split(' ');
        string xpos = temp[0];
        string ypos = temp[1];
        int x = Int32.Parse(xpos);
        int y = Int32.Parse(ypos);

        //xx1 = (x * 4) + ax;
        //xx2 = (-y * 4) + ay;
        //Debug.Log("XX1 :" + xx1);
        //Debug.Log("xx2: " + xx2);
        //string ypos = temp[0].Substring(0, temp[0].Length - 4).TrimStart('+').TrimStart('0');
        //Debug.Log(" x value : " + xpos + " Y value : " + ypos);
        //Debug.Log(c.clientName + "Has sent the following message: " + data);
        if (xx1 > 0 && xx1 < 1280 && xx2 > 0 && xx2 < 800)
        {
            bd = false;
            xx1 = (x * 6) + ax;
            xx2 = (-y * 6) + ay;
            Debug.Log("XX1 :" + xx1);
            Debug.Log("xx2: " + xx2);
            transform.position = Camera.main.ScreenToWorldPoint(new Vector3(xx1, xx2, 10f));
        }
        //if ((xx1 == 0 && xx2 == 0) || (xx1 == 1280 && xx2 == 800))
        if (xx1 <= 0 || xx1 >= 1280 || xx2 <= 0 || xx2 >= 800)
        {
            ax1 = x * 6 + ax;
            ay1 = -y * 6 + ay;
            Debug.Log("Boundry :");
            bd = true;
        }
        if(bd != true)
        {
            ax = x * 6 + ax;
            ay = -y * 6 + ay;
        }
        if(bd == true)
        {
            xx1 = ax1 + (x * 6 );
            xx2 = ay1 + (-y * 6 );

            Debug.Log("xx1: " + xx1 + "xx2: " + xx2);
        }
       

    }

    private void Broadcast(string data,List<ServerClient>cl)
    {
        foreach(ServerClient c in cl)
        {
            try
            {
                StreamWriter writer = new StreamWriter(c.tcp.GetStream());
                writer.WriteLine(data);
                writer.Flush();
                
            }
            catch(Exception e)
            {
                Debug.Log("Write error" + e.Message + "to client" + c.clientName);
            }
        }
    }

    private void AcceptTcpClient(IAsyncResult ar)
    {
        
        TcpListener listener = (TcpListener)ar.AsyncState;

        clients.Add(new ServerClient(listener.EndAcceptTcpClient(ar)));
        StartListning();

        //Send message to everyone, Say someone has connected
        Broadcast(clients[clients.Count - 1].clientName + "has connected", clients);
    }
}

public class ServerClient
{
    public TcpClient tcp;
    public string clientName;

    public ServerClient(TcpClient ClientSocket){
        clientName = "Guest";
        tcp = ClientSocket;
    }
}