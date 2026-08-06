using System;
using System.Collections.Generic;

public static class EventBus
{
    private static readonly Dictionary<Type, Action<object>> _subscribers = new();

    /// <summary>
    /// Subscribe to specific event.
    /// </summary>
    /// <typeparam name="T">Type of event to subscribe to</typeparam>
    /// <param name="listener">Action to execute when event is published</param>
    public static void Subscribe<T>(Action<T> listener)
    {
        Type eventType = typeof(T);

        if (!_subscribers.ContainsKey(eventType))
        {
            _subscribers[eventType] = _ => { };
        }

        _subscribers[eventType] += obj => listener((T)obj);
    }

    /// <summary>
    /// Unsubscribe to specific event.
    /// </summary>
    /// <typeparam name="T">Type of event to subscribe to</typeparam>
    /// <param name="listener">Action to execute when event is published</param>
    public static void Unsubscribe<T>(Action<T> listener)
    {
        Type eventType = typeof(T);

        if (_subscribers.ContainsKey(eventType))
        {
            _subscribers[eventType] -= obj => listener((T)obj);
        }
    }

    /// <summary>
    /// Publish an event to all subscribers.
    /// </summary>
    /// <typeparam name="T">Type of event to publish</typeparam>
    /// <param name="eventMessage">Event message to publish</param>
    public static void Publish<T>(T eventMessage)
    {
        Type eventType = typeof(T);

        if (_subscribers.TryGetValue(eventType, out Action<object> action))
        {
            action?.Invoke(eventMessage);
        }
    }

    /// <summary>
    /// Unsubscribe from all events.
    /// </summary>
    public static void Clear()
    {
        _subscribers.Clear();
    }
}