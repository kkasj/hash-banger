using System;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        TaskRangeIterator iterator = new TaskRangeIterator();
        IteratorProxy iteratorProxy = new IteratorProxy(iterator);
        var encrypter = EncrypterFactory.CreateEncrypter(EncryptionType.SHA1); 
        string problemHash = encrypter.Encrypt("hashha");
        Problem problem = new Problem(iteratorProxy, new ProblemArgs(problemHash, EncryptionType.SHA1));
        TaskManager taskManager = new TaskManager(problem);
        problem.GotNewProblem(problemHash, EncryptionType.SHA1, iterator);
    }
}