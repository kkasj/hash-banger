
using System;
using design_patterns.Messaging;
using design_patterns.Utils;

public class PeerApp {
    public static void MainChuj(string[] args) {

        var app = new PeerApp();
        app.ParseArguments(args);

        TaskRangeIterator iterator = new TaskRangeIterator();
        IteratorProxy iteratorProxy = new IteratorProxy(iterator);
        var encrypter = EncrypterFactory.CreateEncrypter(EncryptionType.SHA1); 
        string problemHash = encrypter.Encrypt(ProblemParameters.PASSWORD);
        Problem problem = new Problem(iteratorProxy, new ProblemArgs(problemHash, EncryptionType.SHA1));
        TaskManager taskManager = new TaskManager(problem);

        var peer = new LocalPeer();
        var messageHandler = new MessageHandler(peer, problem);
        
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