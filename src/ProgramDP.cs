
using System;
using design_patterns.Messaging;
using design_patterns.Utils;

public class PeerApp {
    public static void Main(string[] args) {

        var app = new PeerApp();
        app.ParseArguments(args);

        var peer = new LocalPeer();
        var messageHandler = new MessageHandler(peer);
        
        peer.Register();

        while (true) {
            Thread.Sleep(3000);
        }
    }

    private void ParseArguments(string [] args) {
        if (args.Length < 1)
        {
            Console.WriteLine("Usage: PeerApp [port]");
            Environment.Exit(1);
        }

        try {
            int port = int.Parse(args[0]);
            Settings.Port = port;
            Console.WriteLine("Port set to " + port);
        } catch (FormatException) {
            Console.WriteLine("Port must be an integer");
            Environment.Exit(1);
        }
    }
}