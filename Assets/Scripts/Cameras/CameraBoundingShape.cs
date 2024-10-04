using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraBoundingShape : MonoBehaviour
{
    public UnityAction OnZoneExit { get; set; }
    public UnityAction OnZoneEnter { get; set; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (OnZoneEnter != null)
        {
            OnZoneEnter();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (OnZoneExit != null)
        {
            OnZoneExit();
        }
    }
}
