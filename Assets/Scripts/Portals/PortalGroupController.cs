using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalGroupController : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private PortalController _purplePortal;
    [SerializeField] private PortalController _tealPortal;

    private void Start()
    {
        
    }

    public void SetPurplePortal(Vector2 position, Quaternion rotation)
    {
        _purplePortal.SetPortal(position, rotation);
    }

    public void SetTealPortal(Vector2 position, Quaternion rotation)
    {
        _tealPortal.SetPortal(position, rotation);
    }
}
