using design_patterns.Messaging.MessageArgs;
using design_patterns.Utils;

/// <summary>
/// Represents a problem to be solved by the network.
/// </summary>
public class Problem : Observable {
    public IteratorProxy IteratorProxy { get; set; }
    public ProblemArgs? Args { get; set; }

    public Problem(IteratorProxy iteratorProxy, ProblemArgs? args) {
        IteratorProxy = iteratorProxy;
        Args = args;
    }
    
    /// <summary>
    /// Updates problem arguments and notifies subscribers that a new problem has been received.
    /// </summary>
    /// <param name="newProblemArgs"></param>
    public void GotNewProblem(NewProblemArgs newProblemArgs) {
        string problemHash = newProblemArgs.ProblemHash;
        EncryptionType encryptionType = newProblemArgs.EncryptionType;
        TaskRangeIterator? iterator = newProblemArgs.Iterator;

        Args = new ProblemArgs(problemHash, encryptionType);
        // ProblemParameters.UpdatePassword(problemHash);
        // iterator is not null when the peer connects to the network while the problem is already being solved
        IteratorProxy.Reset(iterator);

        PokeAllSubscribers();
    }

    /// <summary>
    /// Notifies subscribers that a task has been completed.
    /// </summary>
    /// <param name="updater"></param>
    /// <param name="range"></param>
    public void TaskDone(RangeUpdater updater, Range range) {
        if (Args is null) {
            return;
        }

        var rangeToUpdate = new RangeUpdate(UpdateType.RangeDone, updater.Source, range);
        IteratorProxy.UpdateRange(rangeToUpdate);
        PokeAllSubscribers();
    }
    
    /// <summary>
    /// Notifies subscribers that a task has been reserved.
    /// </summary>
    public void ReserveTask(RangeUpdater updater, ReservedRange range) {
        if (Args is null) {
            return;
        }

        var rangeToUpdate = new RangeUpdate(UpdateType.RangeReservation, updater.Source, range);
        IteratorProxy.UpdateRange(rangeToUpdate);
        PokeAllSubscribers();
    }

    public void ProblemSolved(string result) {
        if (Args is null) {
            return;
        }

        Args.IsDone = true;
        Args.Result = result;
        Console.WriteLine("Found: " + result);
        PokeAllSubscribers();
    }
}