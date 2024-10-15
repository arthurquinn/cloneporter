using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface ILaserReceiver
{
    void Activate();
    void Deactivate();
}

public class LaserActivator : MonoBehaviour, ILaserReceiver
{
    public UnityAction OnLaserEnter;
    public UnityAction OnLaserExit;

    public void Activate()
    {
        if (OnLaserEnter != null)
        {
            OnLaserEnter();
        }
    }

    public void Deactivate()
    {
        if (OnLaserExit != null)
        {
            OnLaserExit();
        }
    }
}
