using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float _speed; // Tốc độ di chuyển của player.
    private float _originalSpeed; // Tốc độ gốc của player.

    private Rigidbody2D _rigidbody; // Rigidbody2D của player.

    //================= Movement =================
    private Vector2 _movementInput; // Input di chuyển từ Input System.
    private Vector2 _smoothVelocity; // Vận tốc mượt.
    private Vector2 _movementInputSmooth; // Input di chuyển mượt.

    [SerializeField]
    private float _smoothTime = 0.1f; // Thời gian mượt.

    [SerializeField]
    private Animator _animator; // Animator của player.

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _originalSpeed = _speed;
    }

    private void FixedUpdate()
    {
        PlayerVelocity();
    }

    // Hàm di chuyển player với hiệu ứng mượt.
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

    // Hàm nhận input di chuyển từ Input System.
    private void OnMove(InputValue value)
    {
        _movementInput = value.Get<Vector2>();
    }

    // Hàm giảm tốc độ di chuyển của player khi bắn.
    public IEnumerator PlayerSlowSpeed(float speed)
    {
        _speed = speed;
        yield return new WaitForSeconds(3f);
    }

    public void ResetSpeed()
    {
        _speed = _originalSpeed;
    }
}
