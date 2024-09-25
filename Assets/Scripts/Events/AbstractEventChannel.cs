using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public abstract class AbstractEventChannel<TEvent>
{
    public UnityAction<TEvent> Event { get; private set; }

    public void Raise(TEvent @event)
    {
        if (Event != null)
        {
            Event(@event);
        }
    }

    public void Subscribe(UnityAction<TEvent> action)
    {
        Event += action;
    }

    public void Unsubscribe(UnityAction<TEvent> action)
    {
        Event -= action;
    }
}
