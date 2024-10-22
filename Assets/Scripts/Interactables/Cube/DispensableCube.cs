using DG.Tweening;
using System.Collections;
using UnityEngine;

public class DispensableCube : MonoBehaviour
{
    [Header("Launch Forces")]
    [Tooltip("The vertical launch force of the cube.")]
    [SerializeField] private float _launchForce;
    [Tooltip("The horizontal offset force while launching and in midair.")]
    [SerializeField] private float _launchOffsetForce;
    [Tooltip("The angular velocity applied when launched.")]
    [SerializeField] private float _launchAngularVelocity;

    private Rigidbody2D _rb;
    private Collider2D _collider;
    private Cube _cube;

    private Tween _currentTween;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _cube = GetComponent<Cube>();
    }

    public void MoveCube(Vector2 target, float duration, Ease ease, TweenCallback callback)
    {
        // Kill any running tweens
        if (_currentTween != null)
        {
            _currentTween.Kill();
            _currentTween = null;
        }

        // Start the animation
        _currentTween = _rb.DOMove(target, duration)
            .SetEase(ease)
            .OnComplete(callback);
    }

    public void Launch(Collider2D ignoreCollider)
    {
        // Ignore the launchers collider
        Physics2D.IgnoreCollision(_collider, ignoreCollider);

        // Setup cube to act like a normal cube again
        _rb.bodyType = RigidbodyType2D.Dynamic;
        _collider.enabled = true;
        _cube.enabled = true;

        // Calculate launch force sign (50% left or right)
        float launchDirection = Mathf.Round(Random.value) == 0 ? -1 : 1;

        // Add physics forces for the launch
        _rb.AddForce(_launchForce * Vector2.up, ForceMode2D.Impulse);
        _rb.angularVelocity = -launchDirection * _launchAngularVelocity;

        // Start coroutine to wait for launch end
        StartCoroutine(WaitForLaunchEnd(ignoreCollider, launchDirection));
    }

    private IEnumerator WaitForLaunchEnd(Collider2D ignoringCollider, float launchDirection)
    {
        while (_rb.velocity.y > 0)
        {
            // Add a small offset force
            _rb.AddForce(Vector2.right * launchDirection * _launchOffsetForce, ForceMode2D.Force);

            // Wait for next fixed update
            yield return new WaitForFixedUpdate();
        }

        // After we stopped launching reenable collision
        Physics2D.IgnoreCollision(_collider, ignoringCollider, false);
    }
}
