using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CubeDestructable : MonoBehaviour
{
    [Header("Event Channels")]
    [SerializeField] private InteractablesEventChannel _interactableEvents;

    [Header("Material")]
    [Tooltip("The material applied to the cube when it is being destroyed.")]
    [SerializeField] private Material _dissolveMaterial;

    [Header("Destroy Animation")]
    [Tooltip("The amount of time to fully dissolve the cube.")]
    [SerializeField] private float _dissolveTime;

    private Cube _cube;
    private Collider2D _collider;
    private Rigidbody2D _rb;
    private SpriteRenderer[] _renderers;
    private MaterialPropertyBlock[] _blocks;

    private Tween _dissolveTween;

    public UnityAction OnCubeDestroyed { get; set; }

    private void Awake()
    {
        _cube = GetComponent<Cube>();
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _renderers = GetComponentsInChildren<SpriteRenderer>();

        CreatePropertyBlocks();
    }

    private void DestroyCube()
    {
        // Player should drop the cube
        _interactableEvents.OnItemDropped.Raise(new HeldItemDroppedEvent(_cube));

        // Disable regular cube behavior
        _cube.enabled = false;
        _collider.enabled = false;
        _rb.bodyType = RigidbodyType2D.Kinematic;
        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = 0;

        // Start the destroy animations
        StartDestroyAnimation();
    }

    private void StartDestroyAnimation()
    {
        // Set the material
        ApplyDestroyMaterial();

        // Tween the dissolve amount
        float initialValue = 0f;
        float targetValue = 1f;
        _dissolveTween = DOTween.To(() => initialValue, SetDissolveAmount, targetValue, _dissolveTime)
            .OnComplete(FinishDestroy);
    }

    private void FinishDestroy()
    {
        // Kill the tween if it is still running
        _dissolveTween.Kill();

        // Raise the cube on destroy event 
        if (OnCubeDestroyed != null)
        {
            OnCubeDestroyed();
        }

        // Destroy this game object
        Destroy(gameObject);
    }

    private void SetDissolveAmount(float amount)
    {
        for (int i = 0; i < _renderers.Length; i++)
        {
            _renderers[i].GetPropertyBlock(_blocks[i]);
            _blocks[i].SetFloat("_Amount", amount);
            _renderers[i].SetPropertyBlock(_blocks[i]);
        }
    }

    private void CreatePropertyBlocks()
    {
        _blocks = new MaterialPropertyBlock[_renderers.Length];
        for (int i = 0; i < _renderers.Length; i++)
        {
            _blocks[i] = new MaterialPropertyBlock();
        }
    }

    private void ApplyDestroyMaterial()
    {
        for (int i = 0;  i < _renderers.Length; i++)
        {
            _renderers[i].material = _dissolveMaterial;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PortalForceField"))
        {
            DestroyCube();
        }
    }
}
