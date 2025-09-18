using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float _speed;

    private Rigidbody2D _rigidbody;

    //==================== Movement ====================//
    private Vector2 _movementInput;
    private Vector2 _smoothVelocity;
    private Vector2 _movementInputSmooth;

    [SerializeField]
    private float _smoothTime = 0.1f;
    private Quaternion targetRotation;

    // Start is called before the first frame update
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        PlayerVelocity();
        FlipPlayer();
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
    }

    private void FlipPlayer()
    {
        if (_movementInputSmooth.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (_movementInputSmooth.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void OnMove(InputValue value)
    {
        _movementInput = value.Get<Vector2>();
    }
}
