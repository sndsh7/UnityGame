using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//get send from server to client
//client has to listen server packets
public enum ServerPackets
{
    SConnectionOk = 1,
}
public enum ClientPackets
{
    CThankYou = 1,
}