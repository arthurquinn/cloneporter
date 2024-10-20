using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialLevel03 : MonoBehaviour
{
    [Header("Text")]
    [Tooltip("The propel hint text.")]
    [SerializeField] private GameObject _propelHintText;

    [Header("Triggers")]
    [Tooltip("The propel hint show trigger.")]
    [SerializeField] private GenericTrigger _propelHintShow;
    [SerializeField] private GenericTrigger _propelHintHide;

    private void Awake()
    {
        AssignCamera();
    }

    private void OnEnable()
    {
        _propelHintShow.OnTriggerEnter += ShowPropelHint;
        _propelHintHide.OnTriggerEnter += HidePropelHint;
    }

    private void OnDisable()
    {
        _propelHintShow.OnTriggerEnter -= ShowPropelHint;
        _propelHintHide.OnTriggerEnter -= HidePropelHint;
    }

    private void ShowPropelHint()
    {
        _propelHintText.SetActive(true);
    }

    private void HidePropelHint()
    {
        _propelHintText.SetActive(false);
    }

    private void AssignCamera()
    {
        Canvas canvas = GetComponentInChildren<Canvas>();
        if (canvas != null)
        {
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = Camera.main;
            canvas.sortingLayerName = "HUD";
        }
    }
}
