using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    private Animator _anim;
    private PlayerMovement _playerMovement;

    private PlayerInputActions _inputs;

    private int _isGroundedHash;
    private int _isMoveInput;
    private int _triggerJump;
    private int _isJumpFalling;

    private void Awake()
    {
        _inputs = new PlayerInputActions();
    }

    private void Start()
    {
        _anim = GetComponent<Animator>();
        _playerMovement = GetComponent<PlayerMovement>();

        _isGroundedHash = Animator.StringToHash("IsGrounded");
        _isMoveInput = Animator.StringToHash("IsMoveInput");
        _triggerJump = Animator.StringToHash("Jump");
        _isJumpFalling = Animator.StringToHash("IsJumpFalling");
    }

    private void OnEnable()
    {
        _inputs.Player.Movement.Enable();
    }

    private void OnDisable()
    {
        _inputs.Player.Movement.Disable();
    }

    private void Update()
    {
        Vector2 moveInput = _inputs.Player.Movement.ReadValue<Vector2>();

        _anim.SetBool(_isGroundedHash, _playerMovement.LastOnGroundTime > 0);
        _anim.SetBool(_isMoveInput, moveInput != Vector2.zero);
        _anim.SetBool(_isJumpFalling, _playerMovement.IsJumpFalling);
    }

    public void TriggerJump()
    {
        _anim.SetTrigger(_triggerJump);
    }
}
