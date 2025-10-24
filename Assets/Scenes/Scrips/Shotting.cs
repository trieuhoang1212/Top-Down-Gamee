using UnityEditor.Tilemaps;
using UnityEngine;

public class Shotting : MonoBehaviour
{
    private Camera _cameraMain; // Camera chính.
    private Vector3 mousePos; // Vị trí chuột trong thế giới.
    private Transform _playerFlip; // Transform của player để flip.

    // ========== Animation ==========
    [SerializeField]
    private Animator _muzzleFlashAnimator; // Animator hiệu ứng bắn.

    [SerializeField]
    private GameObject _muzzleFlashPrefab; // Prefab hiệu ứng bắn.

    [SerializeField]
    private Transform _gunPoint; // Điểm bắn đạn.

    // ========= Bullet ==========
    [SerializeField]
    private GameObject _bulletPrefab; // Prefab đạn.

    [SerializeField]
    private float _fireInterval = 0.2f; // Thời gian ra đạn.

    [SerializeField]
    private float _fireCooldown; // Thời gian chờ giữa các lần bắn.

    [SerializeField]
    private float _bulletForce; // Lực bắn đạn.

    [SerializeField]
    private float _slowPlayerSpeed = 1f; // Tốc độ giảm của player khi bắn.

    void Start()
    {
        _cameraMain = GameObject.FindGameObjectsWithTag("MainCamera")[0].GetComponent<Camera>();
        _playerFlip = transform.parent;
    }

    void Update()
    {
        RotateGun();
        if (_fireCooldown > 0f)
            _fireCooldown -= Time.deltaTime;

        OnShoot();
    }

    // Hàm xoay súng theo con trỏ chuột.
    void RotateGun()
    {
        // Rotate gun.
        mousePos = _cameraMain.ScreenToWorldPoint(Input.mousePosition);
        Vector3 rotation = mousePos - _cameraMain.transform.position;
        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
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

    // Hàm xử lý bắn.
    private void OnShoot()
    {
        if (Input.GetMouseButton(0) && _fireCooldown <= 0)
        {
            _muzzleFlashAnimator.SetTrigger("Shoot"); // Trigger hiệu ứng bắn.
            FireBullet();
        }
    }

    // Hàm bắn đạn.
    private void FireBullet()
    {
        // Reset đạn.
        _fireCooldown = _fireInterval;

        // Lấy vị trí chuột.
        Vector3 mouse = _cameraMain.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = _gunPoint.position.z;
        Vector2 dir = (mouse - _gunPoint.position).normalized;

        // Gọi đạn.
        GameObject bullet = Instantiate(_bulletPrefab, _gunPoint.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb)
            rb.AddForce(dir * _bulletForce, ForceMode2D.Impulse);

        // Giảm tốc độ player khi bắn.
        PlayerMovement playerMovement = transform.parent.GetComponent<PlayerMovement>();
        if (playerMovement)
            playerMovement.PlayerSlowSpeed(_slowPlayerSpeed);
    }
}
