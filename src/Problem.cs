namespace peer2peer; 

public class Problem : Observable {
    EncryptionType _type;
    public IteratorProxy _iterator;
    
    public void GotNewProblem(ISubscriber subscriber) { //TODO:
        
        PokeAllSubscribers();
    }

    public void TaskDone(RangeUpdater updater, Range range) {
        var rangeToUpdate = RangeUpdate(UpdateType.RangeDone, updater.Source, range);
        _iterator.UpdateRange(rangeToUpdate);
        PokeAllSubscribers();
    }
    
    public void ReserveTask(RangeUpdater updater, Range range) {
        var rangeToUpdate = RangeUpdate(UpdateType.RangeReservation, updater.Source, range);
        _iterator.UpdateRange(rangeToUpdate);
        PokeAllSubscribers();
    }
}