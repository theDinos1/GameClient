using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    private static void SendTCPData(Packet packet)
    {
        packet.WriteLength();
        Client.Instance.Tcp.SendData(packet);
    }
    private static void SendUDPData(Packet packet)
    {
        packet.WriteLength();
        Client.Instance.Udp.SendData(packet);
    }

    #region Packets

    public static void WelcomeReceived()
    {
        using(Packet packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            packet.Write(Client.Instance.MyId);
            packet.Write(UIManager.Instance.UsernameField.text);

            SendTCPData(packet);
        }
    }

    public static void PlayerMovement(bool[] inputs)
    {
        
    }

    #endregion
}
