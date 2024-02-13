using System;
using design_patterns.Messaging;
using design_patterns.Utils;

public class PeerApp {
    public static void Main(string[] args) {

        var app = new PeerApp();
        bool isProblemCreator = app.ParseArguments(args);
        Console.WriteLine("Is problem creator: " + isProblemCreator);

        TaskRangeIterator iterator = new TaskRangeIterator();
        IteratorProxy iteratorProxy = new IteratorProxy(iterator);
        var encrypter = EncrypterFactory.CreateEncrypter(EncryptionType.SHA1); 
        string problemHash = encrypter.Encrypt(ProblemParameters.PASSWORD);
        Problem problem = new Problem(iteratorProxy, null);
        TaskManager taskManager = new TaskManager(problem);

        if (isProblemCreator) {
            NewProblemArgs newProblemArgs = new NewProblemArgs(problemHash, EncryptionType.SHA1, null);
            problem.GotNewProblem(newProblemArgs);
        }

        var peer = new LocalPeer();
        var messageHandler = new MessageHandler(peer, problem);
        
        // peer.Register();

        // listen for new problem args
        // while (true) {
        //     Thread.Sleep(1000);
        // }
    }

    private bool ParseArguments(string [] args) {
        if (args.Length < 2)
        {
            Console.WriteLine("Usage: PeerApp [port] [isProblemCreator]");
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

        // second argument specifies if the peer should be the problem creator
        if (args[1] == "1") {
            return true;
        }
        else {
            return false;
        }
    }
}