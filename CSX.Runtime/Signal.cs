using System;
using System.Collections.Generic;

namespace CSX.Runtime
{
    public interface ISignal
    {
        void Subscribe(Action<object> subscriber);
    }

    public class Signal<T> : ISignal
    {
        private T _value;
        private readonly List<Action<T>> _subscribers = new List<Action<T>>();

        public Signal(T initialValue)
        {
            _value = initialValue;
        }

        public T Value
        {
            get => _value;
            set
            {
                if (!EqualityComparer<T>.Default.Equals(_value, value))
                {
                    _value = value;
                    Notify();
                }
            }
        }

        public void Subscribe(Action<T> subscriber)
        {
            _subscribers.Add(subscriber);
            subscriber(_value); // Immediate callback with current value
        }

        // Type-erased subscription for the Binder
        void ISignal.Subscribe(Action<object> subscriber)
        {
            Subscribe(val => subscriber(val!));
        }

        private void Notify()
        {
            foreach (var sub in _subscribers)
            {
                sub(_value);
            }
        }

        public override string ToString() => _value?.ToString() ?? "";
    }
}
