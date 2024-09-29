using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalPairController : MonoBehaviour
{
    [SerializeField] private PortalController _purplePortal;
    [SerializeField] private PortalController _tealPortal;

    private void Start()
    {
        _purplePortal.SetLinkedPortal(_tealPortal);
        _tealPortal.SetLinkedPortal(_purplePortal);
    }
}
