using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnemyFollow : MonoBehaviour
{
    [SerializeField]
    private float speed = 1.5f;

    [SerializeField]
    private Animator _animator;
    private GameObject _player;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindWithTag("Player");
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        Vector2 dirToPlayer = (_player.transform.position - transform.position).normalized;

        // Tạo hướng lệch 90 độ (ngang hông)
        Vector2 sideDir = new Vector2(-dirToPlayer.y, dirToPlayer.x);

        // Trộn giữa hướng thẳng và hướng ngang
        Vector2 finalDir = (dirToPlayer + sideDir * 0.5f).normalized;

        transform.position += (Vector3)(finalDir * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);
        }
    }

    void EnemyAnimation()
    {
        Vector2 dirToPlayer = (_player.transform.position - transform.position).normalized;
        _animator.SetFloat("isMoving", dirToPlayer.sqrMagnitude);
    }
}
