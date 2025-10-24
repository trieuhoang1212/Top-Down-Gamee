using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float _speed;
    private float _originalSpeed;

    private Rigidbody2D _rigidbody;

    //================= Movement =================
    private Vector2 _movementInput;
    private Vector2 _smoothVelocity;
    private Vector2 _movementInputSmooth;

    [SerializeField]
    private float _smoothTime = 0.1f;

    [SerializeField]
    private Animator _animator;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _originalSpeed = _speed;
    }

    private void FixedUpdate()
    {
        PlayerVelocity();
        ResetSpeedAfterDelay(0.2f);
    }

    private void PlayerVelocity()
    {
        _movementInputSmooth = Vector2.SmoothDamp(
            _movementInputSmooth,
            _movementInput,
            ref _smoothVelocity,
            _smoothTime
        );
        _rigidbody.velocity = _movementInputSmooth * _speed;

        // Animation cho player đang di chuyển.
        _animator.SetFloat("isMoving", _movementInputSmooth.sqrMagnitude);
    }

    private void OnMove(InputValue value)
    {
        _movementInput = value.Get<Vector2>();
    }

    public void PlayerSlowSpeed(float speed)
    {
        _speed = speed;
        StartCoroutine(ResetSpeedAfterDelay(0.2f));
    }

    private IEnumerator ResetSpeedAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
    }
}
