using System.Collections;
using System.Collections.Generic;
using CodeMonkey;
using CodeMonkey.Utils;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Haptics;

public class PlayerAimWeapon : MonoBehaviour
{
    [SerializeField]
    Transform _aimTransform; // Child xoay súng (mặc định tên "Gun")

    [SerializeField]
    Animator _animator;

    [SerializeField]
    GameObject _bulletTrail; // dùng trong Animation Event nếu cần

    [SerializeField]
    Transform _gunPoint; // đầu nòng

    [SerializeField]
    private Transform _bodyPlayer;

    [SerializeField]
    SpriteRenderer _bodySpriteRenderer;

    // Thêm: chống giật khi aim gần tâm
    [SerializeField]
    float _aimDeadZone = 0.1f; // bán kính bỏ qua tính góc (world units)

    bool _facingLeft;

    void Awake()
    {
        if (_aimTransform == null)
            _aimTransform = transform.Find("Gun");
        if (_animator == null)
        {
            // Lấy Animator đúng chỗ (ưu tiên trên _aimTransform)
            if (_aimTransform)
                _animator = _aimTransform.GetComponentInChildren<Animator>(true);
            if (_animator == null)
                _animator = GetComponentInChildren<Animator>(true);
        }
        if (_gunPoint == null && _aimTransform)
            _gunPoint = _aimTransform.Find("GunPoint");

        // Tự tìm bodyPlayer và SpriteRenderer
        if (_bodyPlayer == null || _bodySpriteRenderer == null)
        {
            var srs = GetComponentsInChildren<SpriteRenderer>();
            foreach (var sr in srs)
            {
                if (sr.gameObject != _aimTransform.gameObject)
                {
                    if (_aimTransform != null && sr.transform.IsChildOf(_aimTransform))
                        continue; // bỏ sprite của Gun
                    _bodySpriteRenderer ??= sr;
                    _bodyPlayer ??= sr.transform;
                    if (_bodySpriteRenderer != null && _bodyPlayer != null)
                        break;
                }
            }
        }
    }

    void Update()
    {
        HandleAiming();
        HandleShooting();
        ReloadGunbullet();
    }

    void HandleAiming()
    {
        if (_aimTransform == null)
            return;

        // Lấy gốc tính góc tại nòng súng (ổn định hơn so với transform player)
        Vector3 origin = _gunPoint ? _gunPoint.position : _aimTransform.position;
        Vector3 mouse = UtilsClass.GetMouseWorldPosition();
        Vector3 dir = mouse - origin;

        // Dead-zone: khi lia sát tâm thì giữ nguyên hướng cũ, tránh nhảy
        if (dir.sqrMagnitude < _aimDeadZone * _aimDeadZone)
            return;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        _aimTransform.rotation = Quaternion.Euler(0f, 0f, angle);

        // Flip súng theo trục Y để sprite không bị lộn ngược
        _aimTransform.localScale = _facingLeft ? new Vector3(1f, -1f, 1f) : Vector3.one;

        // Flip thân theo trục X
        if (_bodySpriteRenderer != null)
        {
            _bodySpriteRenderer.flipX = _facingLeft;
        }
        else if (_bodyPlayer != null)
        {
            var s = _bodyPlayer.localScale;
            s.x = Mathf.Abs(s.x) * (_facingLeft ? -1f : 1f);
            _bodyPlayer.localScale = s;
        }
    }

    void HandleShooting()
    {
        if (_animator == null)
            return;
        if (Input.GetMouseButtonDown(0))
            _animator.SetTrigger("Shoot"); // sẽ phát clip Shoot
    }

    // Gọi bằng Animation Event trong clip Shoot tại frame nổ súng
    public void Fire()
    {
        if (_gunPoint == null || _bulletTrail == null)
            return;

        Vector3 mouse = UtilsClass.GetMouseWorldPosition();
        var go = Instantiate(_bulletTrail, _gunPoint.position, Quaternion.identity);

        var trail = go.GetComponent<TrailBullet>();
        if (trail != null)
            trail.SetTargetPosition(mouse);
    }

    public void ReloadGunbullet()
    {
        if (_animator == null)
            return;
        if (Input.GetKeyDown(KeyCode.R))
        {
            _animator.SetTrigger("Reload");
        }
    }
}
