using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class Client : MonoBehaviour {

    private bool socketReady;
    private TcpClient socket;
    private NetworkStream stream;
    private StreamWriter writer;
    private StreamReader reader;

    string host;
    int port;

    public Text x,y,z;

    public void ConnectToServer()
    {
        //If already connected, Ignore this function
        if (socketReady)
            return;
        
        //Default host value
        host = "127.0.0.1";
        port = 6321;

        //Overwrite default host/port value, if there is something in those boxes
        string h;
        int p;
        h = GameObject.Find("HostInput").GetComponent<InputField>().text;
        if (h != "")
            host = h;
        int.TryParse(GameObject.Find("PortInput").GetComponent<InputField>().text, out p);
        if (p != 0)
            port = p;

        //Create the socket
        try
        {
            socket = new TcpClient(host, port);
            stream = socket.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);
            socketReady = true;
        }
        catch(Exception e)
        {
            Debug.Log("Socket Connection Error :"+e.Message);
        }
    }

	private void Update ()
    {
        Screen.orientation = ScreenOrientation.Portrait;

        if(socketReady)
        {
            if(stream.DataAvailable)
            {
                string data = reader.ReadLine();
                if (data != null)
                    OnIncommingData(data);
               
            }
        }
        //for mouse pointer position
        //string message = Mathf.RoundToInt(Input.mousePosition.x).ToString()+" "+ Mathf.RoundToInt(Input.mousePosition.y);

        //take input from device sensor

        string message = Mathf.RoundToInt((Input.acceleration.x)*10).ToString()+" "+Mathf.RoundToInt((Input.acceleration.y)*10).ToString();

        int zAxis = Mathf.RoundToInt((Input.acceleration.z) * 10);



        x.text = Mathf.RoundToInt((Input.acceleration.x)*10).ToString();
        y.text = Mathf.RoundToInt((Input.acceleration.y)*10).ToString();
        z.text = Mathf.RoundToInt((Input.acceleration.z) * 10).ToString();

        int Z = Convert.ToInt32(z);
        //int xpos = Mathf.RoundToInt(Input.mousePosition.x);
        //int ypos = Mathf.RoundToInt(Input.mousePosition.y);
        //Send(message);
        if(Z >= 10)
        {
            OnSendButton();
        }

        //Send(xpos, ypos);

	}

    private void OnIncommingData(string data)
    {
        Debug.Log("Server : " + data);
    }

    private void Send(string data)
    {
        if (!socketReady)
            return;
        writer.WriteLine(data);
        writer.Flush();
    }
    public void OnSendButton()
    {
        byte[] imageArray = File.ReadAllBytes(Application.persistentDataPath+"Initial/test.png");
        string base64ImageRepresentation = Convert.ToBase64String(imageArray);
        Send(base64ImageRepresentation);
    }
  
}
