using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FloorSwitchTrigger : MonoBehaviour
{
    public UnityAction<Collider2D> OnSwitchActivated { get; set; }
    public UnityAction<Collider2D> OnSwitchDeactivated { get; set; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (OnSwitchActivated != null)
        {
            OnSwitchActivated(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (OnSwitchDeactivated != null)
        {
            OnSwitchDeactivated(collision);
        }
    }
}
