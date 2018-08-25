using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientHandleNetworkData : MonoBehaviour {

    private delegate void Packet_(byte[] data);
    private static Dictionary<int, Packet_> Packets;

    public static void InititalizeNetworkPackages()
    {
        Debug.Log("Initialize Network Packages");
        Packets = new Dictionary<int, Packet_>
            {
                {(int)ServerPackets.SConnectionOk,HandleConnectonOk}
            };
    }

    public void Awake()
    {
        InititalizeNetworkPackages();
    }

    public static void HandleNetworkInfromation(byte[] data)
    {
        int Packetnum; PacketBuffer buffer = new PacketBuffer();
        Packet_ Packet;
        buffer.WriteByte(data);
        Packetnum = buffer.ReadInteger();
        buffer.Dispose();
        if (Packets.TryGetValue(Packetnum, out Packet))
        {
            Packet.Invoke(data);
        }
    }

    private static void HandleConnectonOk(byte[] data)
    {
        PacketBuffer buffer = new PacketBuffer();
        buffer.WriteByte(data);
        buffer.ReadInteger();
        string msg = buffer.ReadString();
        buffer.Dispose();

        //add your code you want to execute here:
        Debug.Log(msg);

        //ClientTCP.ThankYouServer();
        ClientTCP.SendImage();
    }
}
