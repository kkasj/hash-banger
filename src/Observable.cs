public abstract class Observable {
    private List<ISubscriber> subscribers = new();
    
    public void Subscribe(ISubscriber subscriber) {
        subscribers.Add(subscriber);
    }
    
    public void PokeAllSubscribers() {
        foreach (var subscriber in subscribers) {
            subscriber.Poke();
        }
    }
}