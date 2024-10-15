using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTerminus : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        ILaserReceiver receiver = collision.GetComponent<ILaserReceiver>();
        if (receiver != null)
        {
            receiver.Activate();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        ILaserReceiver receiver = collision.GetComponent<ILaserReceiver>();
        if (receiver != null)
        {
            receiver.Deactivate();
        }
    }
}
