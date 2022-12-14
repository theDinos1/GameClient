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
    public static void SpawnPlayer(Packet packet)
    {
        int playerId = packet.ReadInt();
        string username = packet.ReadString();
        Vector3 position = packet.ReadVector3();
        Quaternion rotatiom = packet.ReadQuaternion();

        GameManager.Instance.SpawnPlayer(playerId, username, position, rotatiom);
    }

}
