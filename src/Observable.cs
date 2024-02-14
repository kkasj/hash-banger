/// <summary>
/// Represents an object that can be observed by subscribers.
/// </summary>
public abstract class Observable {
    public List<Subscriber> Subscribers = new();
    
    /// <summary>
    /// Subscribes a subscriber.
    /// </summary>
    /// <param name="subscriber"></param>
    public void Subscribe(Subscriber subscriber) {
        Subscribers.Add(subscriber);
    }
    
    /// <summary>
    /// Pokes all subscribers.
    /// </summary>
    public void PokeAllSubscribers() {
        foreach (Subscriber subscriber in Subscribers) {
            subscriber.Poke();
        }
    }
}