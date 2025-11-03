using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnemySpawn : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab; // Prefab của kẻ thù để spawn

    [SerializeField]
    private Transform _playerTransform; // Thêm tham chiếu đến player

    [SerializeField]
    private float _sizeX = 1f; // vùng spawn theo trục X

    [SerializeField]
    private float _sizeY = 1f;

    [SerializeField]
    private int _numberOfEnemies = 5;

    [SerializeField]
    private float _spawnCooldown = 3f;

    [SerializeField]
    private float _timeSpawn;

    void Start()
    {
        _timeSpawn = _spawnCooldown;

        // Nếu chưa gán player trong Inspector, tự động tìm
        if (_playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                _playerTransform = player.transform;
            }
        }
    }

    void Update()
    {
        if (_spawnCooldown > 0)
            _spawnCooldown -= Time.deltaTime;
        if (_spawnCooldown <= 0)
        {
            SpawnEnemies();
            _spawnCooldown = _timeSpawn;
        }

        NumberOfEnemies();
    }

    private void SpawnEnemies()
    {
        Vector3 spawnCenter = _playerTransform.position; // lấy vị trí của player làm trung tâm spawn

        float xPos = (Random.value - 0.5f) * 2 * _sizeX + spawnCenter.x;
        float yPos = (Random.value - 0.5f) * 2 * _sizeY + spawnCenter.y;

        var spawn = Instantiate(_enemyPrefab);

        spawn.transform.position = new Vector3(xPos, yPos, 0);
    }

    private void NumberOfEnemies()
    {
        int EnemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        if (EnemyCount >= _numberOfEnemies)
        {
            _spawnCooldown = _timeSpawn;
        }
    }
}
