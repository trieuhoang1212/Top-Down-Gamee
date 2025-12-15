using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnemyFollow : MonoBehaviour
{
    [SerializeField]
    private float speed = 1.5f;

    private GameObject _player;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            _player.transform.position,
            speed * Time.deltaTime
        );
    }
}
