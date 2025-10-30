using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnemySpawn : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab; // Prefab kẻ địch.

    [SerializeField]
    private float _minimumSpawnTime; // Thời gian tối thiểu để spawn kẻ địch.

    [SerializeField]
    private float _maximumSpawnTime; // Thời gian tối đa để spawn kẻ địch.

    [SerializeField]
    private float _spawnUntilSpawn; // Thời gian đếm ngược đến lần spawn tiếp theo.

    void Awake()
    {
        SetTimeSpawn();
    }

    // Update is called once per frame
    void Update()
    {
        _spawnUntilSpawn -= Time.deltaTime;
        if (_spawnUntilSpawn <= 0f)
        {
            Instantiate(_enemyPrefab, transform.position, Quaternion.identity);
            SetTimeSpawn();
        }
    }

    private void SetTimeSpawn()
    {
        _spawnUntilSpawn = Random.Range(_minimumSpawnTime, _maximumSpawnTime);
    }
}
