using System;
using design_patterns.Utils;
using design_patterns.Messaging;
using design_patterns.Messaging.MessageArgs;
using design_patterns.ProblemManagement;
using design_patterns.TaskRanges;
using design_patterns.Encryption;
using design_patterns.Tasking;

public class PeerApp {
    public static void Main(string[] args) {

        var app = new PeerApp();
        bool isProblemCreator = app.ParseArguments(args);
        Console.WriteLine("Is problem creator: " + isProblemCreator);

        TaskRangeCollection taskRangeCollection = new TaskRangeCollection();
        RandomTaskRangeIterator iterator = new RandomTaskRangeIterator(taskRangeCollection);
        Problem problem = new Problem(iterator);
        TaskRangeCollectionManager taskRangeCollectionManager = new TaskRangeCollectionManager(problem, taskRangeCollection);

        TaskManager taskManager = new TaskManager(problem);

        var peer = new LocalPeer();
        var messageHandler = new MessageHandler(peer, problem);

        if (isProblemCreator) {
            var encrypter = EncrypterFactory.CreateEncrypter(EncryptionType.SHA1); 
            string problemHash = encrypter.Encrypt(ProblemParameters.PASSWORD);
            NewProblemArgs newProblemArgs = new NewProblemArgs(problemHash, EncryptionType.SHA1, null);
            problem.GotNewProblem(null, newProblemArgs);
        }

        
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