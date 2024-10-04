using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StabilizeFollowHUD : MonoBehaviour
{
    private Transform _parent;

    private void Awake()
    {
        _parent = transform.parent;
    }

    private void LateUpdate()
    {
        if (_parent != null)
        {
            transform.rotation = Quaternion.identity;
        }
    }
}
