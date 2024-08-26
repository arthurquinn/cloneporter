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
        _purplePortal.SetPosition(position);
        _purplePortal.SetRotation(rotation);
    }

    public void SetTealPortal()
    {

    }
}
