using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    private Vector3 _startPosition; // Vị trí ban đầu của viên đạn.

    [SerializeField]
    private float _lifetime = 2f;

    [SerializeField]
    private float _bulletRange = 10f;

    [SerializeField]
    private float _bulletDamage = 1f;

    public void SetDamage(float damage)
    {
        _bulletDamage = damage;
    }

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
            Enemy enemy = other.GetComponentInParent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(_bulletDamage);
            }
            Destroy(gameObject);
        }
    }

    private void FireRange()
    {
        // Vượt quá thì hủy
        if (Vector3.Distance(_startPosition, transform.position) >= _bulletRange)
        {
            Destroy(gameObject);
        }
    }
}
