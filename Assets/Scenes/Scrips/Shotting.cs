using UnityEngine;

public class Shotting : MonoBehaviour
{
    private Camera _cameraMain; // Camera chính.
    private Vector3 mousePos; // Vị trí chuột trong thế giới.
    private Transform _playerFlip; // Transform của player để flip.
    private PlayerMovement _playerMovement; // Tham chiếu đến script PlayerMovement.

    // ========== Animation ==========
    [SerializeField]
    private Animator _muzzleFlashAnimator; // Animator hiệu ứng bắn.

    // [SerializeField]
    // private GameObject _muzzleFlashPrefab; // Prefab hiệu ứng bắn.

    [SerializeField]
    private Transform _gunPoint; // Điểm bắn đạn.

    // ========= Bullet ==========
    [SerializeField]
    private GameObject _bulletPrefab; // Prefab đạn.

    [SerializeField]
    private float _fireInterval = 0.2f; // Thời gian ra đạn.

    [SerializeField]
    private float _bulletSpeed = 10f; // Tốc độ đạn.

    [SerializeField]
    private float _fireCooldown; // Thời gian chờ giữa các lần bắn.

    [SerializeField]
    private float _slowPlayerSpeed = 2f; // Tốc độ giảm của player khi bắn.

    // ========= Auto Aim ==========
    [SerializeField]
    private float _autoAimRange = 8f; // Phạm vi auto aim.

    private bool _autoAimEnabled = false; // Trạng thái auto aim.
    private Transform _currentTarget; // Enemy đang được target.

    private bool _isShooting = false; // Kiểm tra trạng thái bắn.

    void Start()
    {
        _cameraMain = GameObject.FindGameObjectsWithTag("MainCamera")[0].GetComponent<Camera>();
        _playerFlip = transform.parent;
        _playerMovement = transform.parent.GetComponent<PlayerMovement>();
    }

    void Update()
    {
        // Tìm target nếu auto aim được bật
        if (_autoAimEnabled)
        {
            FindNearestEnemy();
        }

        RotateGun();
        if (_fireCooldown > 0f)
            _fireCooldown -= Time.deltaTime;

        OnShoot();
    }

    // Hàm xoay súng theo con trỏ chuột hoặc auto aim vào enemy.
    void RotateGun()
    {
        Vector3 targetPosition;

        if (_autoAimEnabled && _currentTarget != null)
        {
            // Auto aim - xoay vào enemy, KHÔNG theo chuột
            targetPosition = _currentTarget.position;
        }
        else
        {
            // Manual aim - theo chuột
            mousePos = _cameraMain.ScreenToWorldPoint(Input.mousePosition);
            targetPosition = mousePos;
        }

        // Tính toán rotation
        Vector3 directionToTarget = targetPosition - transform.position;
        float rotZ = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotZ);

        // flip the gun và player.
        if (transform.eulerAngles.z > 90 && transform.eulerAngles.z < 270)
        {
            transform.localScale = new Vector3(-0.5f, -0.5f, 0);
            _playerFlip.localScale = new Vector3(-1, 1, 0);
        }
        else
        {
            transform.localScale = new Vector3(0.5f, 0.5f, 0);
            _playerFlip.localScale = new Vector3(1, 1, 0);
        }
    }

    // Hàm tìm enemy gần nhất
    void FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemies.Length == 0)
        {
            _currentTarget = null;
            return;
        }

        Transform nearestEnemy = null;
        float nearestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            if (enemy != null)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);

                if (distance < _autoAimRange && distance < nearestDistance)
                {
                    nearestEnemy = enemy.transform;
                    nearestDistance = distance;
                }
            }
        }

        _currentTarget = nearestEnemy;
    }

    // Hàm xử lý bắn.
    private void OnShoot()
    {
        if (Input.GetMouseButton(0) && _fireCooldown <= 0)
        {
            if (!_isShooting)
            {
                _isShooting = true;
                if (_playerMovement)
                    StartCoroutine(_playerMovement.PlayerSlowSpeed(_slowPlayerSpeed));
            }

            _muzzleFlashAnimator.SetTrigger("Shoot"); // Trigger hiệu ứng bắn.
            FireBullet();
        }
        else if (Input.GetMouseButtonUp(0) && _isShooting)
        {
            _isShooting = false;
            if (_playerMovement)
                _playerMovement.ResetSpeed();
        }
    }

    // Hàm bắn đạn.
    private void FireBullet()
    {
        // Reset đạn.
        _fireCooldown = _fireInterval;

        Vector2 dir;

        if (_autoAimEnabled && _currentTarget != null)
        {
            // Auto aim - bắn vào enemy
            dir = (_currentTarget.position - _gunPoint.position).normalized;
        }
        else
        {
            // Manual aim - bắn vào chuột
            Vector3 mouse = _cameraMain.ScreenToWorldPoint(Input.mousePosition);
            mouse.z = _gunPoint.position.z;
            dir = (mouse - _gunPoint.position).normalized;
        }

        // Gọi đạn.
        GameObject bullet = Instantiate(_bulletPrefab, _gunPoint.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb)
        {
            rb.velocity = dir * _bulletSpeed; // Tốc độ đạn.
        }
        // Giảm tốc độ player khi bắn.
        PlayerMovement playerMovement = transform.parent.GetComponent<PlayerMovement>();
        if (playerMovement)
            playerMovement.PlayerSlowSpeed(_slowPlayerSpeed);
    }

    // Public method để toggle auto aim từ button UI
    public void ToggleAutoAim()
    {
        _autoAimEnabled = !_autoAimEnabled;
        if (!_autoAimEnabled)
            _currentTarget = null;
    }

    // Public property để UI truy cập
    public bool IsAutoAimEnabled => _autoAimEnabled;
    public Transform CurrentTarget => _currentTarget;
}
