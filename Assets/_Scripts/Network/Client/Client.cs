using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

public class Client : MonoBehaviour
{
    public static Client Instance;
    public static int DataBufferSize = 4096;

    public string Ip = "127.0.0.1";
    public int Port = 1214;
    public int MyId = 0;
    public TCP Tcp;
    public UDP Udp;

    private delegate void PacketHandler(Packet packet);
    private static Dictionary<int, PacketHandler> _PacketHandlers;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    private void Start()
    {
        Tcp = new TCP();
        Udp = new UDP();
    }

    public void ConnectToServer()
    {
        InitializeClientData();
        Tcp.Connect();
    }


    public class TCP
    {
        public TcpClient Socket;

        private NetworkStream _Stream;
        private Packet _ReceiveData;
        private byte[] _ReceiveBuffer;

        public void Connect()
        {
            Socket = new TcpClient
            {
                ReceiveBufferSize = DataBufferSize,
                SendBufferSize = DataBufferSize
            };

            _ReceiveBuffer = new byte[DataBufferSize];
            Socket.BeginConnect(Instance.Ip, Instance.Port, ConnectCallback, Socket);
            Debug.Log($"Connect TCP, IP: {Instance.Ip}:{Instance.Port}");
        }

        private void ConnectCallback(IAsyncResult _result)
        {
            Socket.EndConnect(_result);

            if (!Socket.Connected)
            {
                return;
            }

            _Stream = Socket.GetStream();

            _ReceiveData = new Packet();

            _Stream.BeginRead(_ReceiveBuffer, 0, DataBufferSize, ReceiveCallback, null);
        }

        private void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                int byteLength = _Stream.EndRead(_result);
                if (byteLength <= 0)
                {
                    // TODO: disconnect
                    return;
                }

                byte[] data = new byte[byteLength];
                Array.Copy(_ReceiveBuffer, data, byteLength);

                _ReceiveData.Reset(HandleData(data));
                _Stream.BeginRead(_ReceiveBuffer, 0, DataBufferSize, ReceiveCallback, null);
            }
            catch
            {
                // TODO: disconnect
            }
        }
        public void SendData(Packet packet)
        {
            try
            {
                if (Socket != null)
                {
                    _Stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error sending data to server via TCP: {ex}");
            }
        }
        private bool HandleData(byte[] _data)
        {
            int packetLength = 0;

            _ReceiveData.SetBytes(_data);

            if (_ReceiveData.UnreadLength() >= 4)
            {
                packetLength = _ReceiveData.ReadInt();
                if (packetLength <= 0)
                {
                    return true;
                }
            }

            while (packetLength > 0 && packetLength <= _ReceiveData.UnreadLength())
            {
                byte[] packetBytes = _ReceiveData.ReadBytes(packetLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet packet = new Packet(packetBytes))
                    {
                        int packetId = packet.ReadInt();
                        Debug.Log($"Received packet id: {packetId} via TCP");
                        _PacketHandlers[packetId](packet);
                    }
                });

                packetLength = 0;
                if (_ReceiveData.UnreadLength() >= 4)
                {
                    packetLength = _ReceiveData.ReadInt();
                    if (packetLength <= 0)
                    {
                        return true;
                    }
                }
            }
            if (packetLength <= 1)
            {
                return true;
            }

            return false;
        }
    }
    public class UDP
    {
        public UdpClient Socket;
        public IPEndPoint EndPoint;

        public UDP()
        {
            EndPoint = new IPEndPoint(IPAddress.Parse(Instance.Ip), Instance.Port);
        }

        public void Connect(int localPort)
        {
            Debug.Log($"Connect UDP, IP: {Instance.Ip}:{Instance.Port}");
            Socket = new UdpClient(localPort);

            Socket.Connect(EndPoint);
            Socket.BeginReceive(ReceiveCallback, null);

            using(Packet packet = new Packet())
            {
                SendData(packet);
            }
        }

        public void SendData(Packet packet)
        {
            try
            {
                packet.InsertInt(Instance.MyId);

                if(Socket != null)
                {
                    Socket.BeginSend(packet.ToArray(), packet.Length(), null, null);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error sending data to server via UDP: {ex}");
            }
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                byte[] data = Socket.EndReceive(result, ref EndPoint);
                Socket.BeginReceive(ReceiveCallback, null);

                if (data.Length < 4)
                {
                    // TODO: disconnect
                    Debug.Log("UDP receive callback return cause data length is less than 4");
                    return;
                }
                HandleData(data);
            }
            catch (Exception ex)
            {
                // TODO: disconnect
            }

        }

        private void HandleData(byte[] data)
        {
            using(Packet packet = new Packet(data))
            {
                int packetLength = packet.ReadInt();
                data = packet.ReadBytes(packetLength);
            }

            ThreadManager.ExecuteOnMainThread(() => 
            {
                using (Packet packet = new Packet(data))
                {
                    int packetId = packet.ReadInt();
                    Debug.Log($"Received packet id: {packetId} via UDP");
                    _PacketHandlers[packetId](packet);
                }

            });
        }
    }
    private void InitializeClientData()
    {
        _PacketHandlers = new Dictionary<int, PacketHandler>()
        {
            {(int)ServerPackets.welcome, ClientHandle.Welcome },
            {(int)ServerPackets.sendPackage, ClientHandle.HandleMessage },
            {(int)ServerPackets.spawnPlayer, ClientHandle.SpawnPlayer },

        };
        Debug.Log("Initialize packets.");
    }
}