public abstract class Observable {
    public List<Subscriber> Subscribers = new();
    
    public void Subscribe(Subscriber subscriber) {
        Subscribers.Add(subscriber);
    }
    
    public void PokeAllSubscribers() {
        foreach (Subscriber subscriber in Subscribers) {
            subscriber.Poke();
        }
    }
}