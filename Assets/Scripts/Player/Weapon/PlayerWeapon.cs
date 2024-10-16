using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Materials")]
    
    [Tooltip("The material to be applied to the line renderer for purple targeting.")]
    [SerializeField] private Material _purpleBeamMaterial;

    [Tooltip("The material to be applied to the line renderer for teal targeting.")]
    [SerializeField] private Material _tealBeamMaterial;

    private LineRenderer _lineRenderer;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    public void SetPurpleTargetingBeam()
    {
        _lineRenderer.material = _purpleBeamMaterial;
    }

    public void SetTealTargetingBeam()
    {
        _lineRenderer.material = _tealBeamMaterial;
    }
}
