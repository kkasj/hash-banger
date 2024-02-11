using System;
using System.Collections.Generic;
using design_patterns.Messaging.MessageArgs;
using design_patterns.Utils;

class Program
{
    static void Main(string[] args)
    {
        TaskRangeIterator iterator = new TaskRangeIterator();
        IteratorProxy iteratorProxy = new IteratorProxy(iterator);
        var encrypter = EncrypterFactory.CreateEncrypter(EncryptionType.SHA1); 
        string problemHash = encrypter.Encrypt(ProblemParameters.PASSWORD);
        Problem problem = new Problem(iteratorProxy, new ProblemArgs(problemHash, EncryptionType.SHA1));
        TaskManager taskManager = new TaskManager(problem);
        NewProblemArgs newProblemArgs = new NewProblemArgs(problemHash, EncryptionType.SHA1, iterator);
        problem.GotNewProblem(newProblemArgs);
    }
}