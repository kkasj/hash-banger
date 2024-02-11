using Lidgren.Network;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using design_patterns.Messaging.MessageArgs;
using design_patterns.Utils;

namespace design_patterns.Messaging;

public class LocalPeer {
    private readonly NetPeer _peer;
    
    // private List<NetConnection> _connections = new List<NetConnection>();

    public delegate void MessageReceivedEventHandler(string message);
    public event MessageReceivedEventHandler MessageReceived;

    public LocalPeer() {
        var config = new NetPeerConfiguration("super-hackers") {
            Port = Settings.Port,
            AcceptIncomingConnections = true,
            EnableUPnP = true
        };
        _peer = new NetPeer(config);
        _peer.Start();
        Console.WriteLine($"Peer started on port {_peer.Port}");
        var netThread = new Thread(new ThreadStart(ProcessNet));
        netThread.Start();
    }

    private void ProcessNet() {
        Console.WriteLine("NetWorker started, waiting for messages...");
        while (true) {
            while (_peer.ReadMessage() is { } msg) {
                switch (msg.MessageType) {
                    case NetIncomingMessageType.Data:
                        Console.WriteLine($"Received data from {msg.SenderConnection}.");
                        MessageReceived?.Invoke(msg.ReadString());
                        break;

                    case NetIncomingMessageType.StatusChanged:
                        Console.WriteLine($"Status {msg.SenderConnection} changed: {msg.SenderConnection.Status}");
                        break;
                    default:
                        Console.WriteLine("Unhandled type: " + msg.MessageType);
                        break;
                }

                _peer.Recycle(msg);
            }

            Thread.Sleep(1000);
        }
    }

    public void Register() {
        _peer.Connect("127.0.0.1", 42069);
    }

    public void BroadcastMessage(Message msg) {
        foreach (var connection in _peer.Connections) {
            if (connection == _peer.Connections[0]) {
                return;
            }
            NetOutgoingMessage outgoingMessage = _peer.CreateMessage();
            outgoingMessage.Write(JsonSerializer.Serialize(msg));
            _peer.SendMessage(outgoingMessage, connection, NetDeliveryMethod.ReliableOrdered);
        }
    }

    public void SendMessage(NetConnection receiver, Message msg) {
        NetOutgoingMessage outgoingMessage = _peer.CreateMessage();
        outgoingMessage.Write(JsonSerializer.Serialize(msg));
        _peer.SendMessage(outgoingMessage, receiver, NetDeliveryMethod.ReliableOrdered);
    }
    
    public Message CreateMessage(MessageType type, string content) {
        return new Message(type, content);
    }

    public void NodeConnected(NewNodeConnectedArgs args) {
        var ip = args.Socket.Split(":")[0];
        var port = int.Parse(args.Socket.Split(":")[1]);
        _peer.Connect(ip, port);
    }

    public void NodeDisconnected(NodeDisconnectedArgs args) {
        var ip = args.Socket.Split(":")[0];
        var port = int.Parse(args.Socket.Split(":")[1]);
    }

    public void NodesReceived(NodesReceivedArgs args) {
        Console.WriteLine($"Nodes received");
    }
}