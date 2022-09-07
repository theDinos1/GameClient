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
    }

    public void ConnectToServer()
    {
        Tcp.Connect();
    }

    public class TCP
    {
        public TcpClient Socket;

        private NetworkStream _Stream;
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
        }

        private void ConnectCallback(IAsyncResult _result)
        {
            Socket.EndConnect(_result);

            if (!Socket.Connected)
            {
                return;
            }

            _Stream = Socket.GetStream();

            _Stream.BeginRead(_ReceiveBuffer, 0, DataBufferSize, ReceiveCallback, null);
        }

        private void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                int _byteLength = _Stream.EndRead(_result);
                if (_byteLength <= 0)
                {
                    // TODO: disconnect
                    return;
                }

                byte[] _data = new byte[_byteLength];
                Array.Copy(_ReceiveBuffer, _data, _byteLength);

                // TODO: handle data
                _Stream.BeginRead(_ReceiveBuffer, 0, DataBufferSize, ReceiveCallback, null);
            }
            catch
            {
                // TODO: disconnect
            }
        }
    }
}