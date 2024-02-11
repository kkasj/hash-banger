public class Problem : Observable {
    public IteratorProxy IteratorProxy { get; set; }
    public ProblemArgs Args { get; set; }

    public Problem(IteratorProxy iteratorProxy, ProblemArgs args) {
        IteratorProxy = iteratorProxy;
        Args = args;
    }
    
    public void GotNewProblem(string problemHash, EncryptionType encryptionType, TaskRangeIterator? iterator) {
        Args = new ProblemArgs(problemHash, encryptionType);
        // iterator is not null when the peer connects to the network while the problem is already being solved
        if (iterator != null) {
            IteratorProxy = new IteratorProxy(iterator);
        }
        IteratorProxy.Reset();
    }

    public void TaskDone(RangeUpdater updater, Range range) {
        var rangeToUpdate = new RangeUpdate(UpdateType.RangeDone, updater.Source, range);
        IteratorProxy.UpdateRange(rangeToUpdate);
        PokeAllSubscribers();
    }
    
    public void ReserveTask(RangeUpdater updater, Range range) {
        var rangeToUpdate = new RangeUpdate(UpdateType.RangeReservation, updater.Source, range);
        IteratorProxy.UpdateRange(rangeToUpdate);
        PokeAllSubscribers();
    }
}