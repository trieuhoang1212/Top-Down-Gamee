using System;
using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector3 _startPosition; // Vị trí ban đầu của viên đạn.

    [SerializeField]
    private float _lifetime = 2f;

    [SerializeField]
    private CapsuleCollider2D capsuleCollider2D;

    [SerializeField]
    private float _bulletRange = 10f;

    private void Start()
    {
        Destroy(gameObject, _lifetime);
        _startPosition = transform.position;
    }

    private void Update()
    {
        FireRange();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Chỉ xử lý va chạm với Enemy
        if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }

    private void FireRange()
    {
        // tính hướng bắn từ vị trí ban đầu
        Vector3 dir = (transform.position - _startPosition).normalized;

        // Vượt quá thì hủy
        if (Vector3.Distance(_startPosition, transform.position) >= _bulletRange)
        {
            Destroy(gameObject);
        }
    }
}
