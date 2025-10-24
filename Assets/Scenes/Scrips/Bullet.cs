using System;
using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float _lifetime = 2f;

    private void Start()
    {
        Destroy(gameObject, _lifetime);
    }
}
