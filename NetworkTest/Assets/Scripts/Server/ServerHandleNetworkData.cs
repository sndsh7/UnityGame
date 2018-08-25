using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerHandleNetworkData : MonoBehaviour {

    private delegate void Packet_(int index, byte[] data);
    private static Dictionary<int, Packet_> Packets;

    public static void InititalizeNetworkPackages()
    {
        Debug.Log("Initialize Network Packages");
        Packets = new Dictionary<int, Packet_>
            {
                {(int)ClientPackets.CThankYou,HandleThankYou}
            };
    }

    public static void HandleNetworkInfromation(int index, byte[] data)
    {
        int Packetnum; PacketBuffer buffer = new PacketBuffer();
        Packet_ Packet;
        buffer.WriteByte(data);
        Packetnum = buffer.ReadInteger();
        buffer.Dispose();
        if (Packets.TryGetValue(Packetnum, out Packet))
        {
            Packet.Invoke(index, data);
        }
    }

    private static void HandleThankYou(int index, byte[] data)
    {
        PacketBuffer buffer = new PacketBuffer();
        buffer.WriteByte(data);
        buffer.ReadInteger();
        string msg = buffer.ReadString();
        buffer.Dispose();

        //add your code you want to execute here:
        Debug.Log(msg);
    }

}

