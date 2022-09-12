using System;
using System.Net;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet packet)
    {
        string msg = packet.ReadString();
        int myId = packet.ReadInt();

        Debug.Log($"Message from server: {msg}");
        Client.Instance.MyId = myId;
        ClientSend.WelcomeReceived();

        Client.Instance.Udp.Connect(((IPEndPoint)Client.Instance.Tcp.Socket.Client.LocalEndPoint).Port);
    }
    public static void HandleMessage(Packet packet)
    {
        string msg = packet.ReadString();
        int myId = packet.ReadInt();

        if(myId == Client.Instance.MyId)
        {
            Debug.Log(msg);
        }
    }

    public static void UDPTest(Packet packet)
    {
        string msg = packet.ReadString();
        Debug.Log($"Received packet via UDP. Contains message: {msg}");
        ClientSend.UDPTestReceived();
    }
}
