namespace Messaging {
    public delegate void BusEvent<T>(T value);

    public class EventBus<T> {
        event BusEvent<T> busEvent;

        public void Subscribe(BusEvent<T> action) { busEvent += action; }
        public void Unsubscribe(BusEvent<T> action) { busEvent -= action; }

        public void Raise(T value) { busEvent?.Invoke(value); }
    }
}
