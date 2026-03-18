using System.Collections;
using TMPro;
using UnityEngine;

public class ReloadGun : MonoBehaviour
{
    [SerializeField]
    private int _maxBullets = 10;

    private int _currentBullets;

    [SerializeField]
    private float _reloadTime = 1f;

    public int CurrentBullets => _currentBullets;

    [SerializeField]
    private Animator _animator;

    [SerializeField]
    private TMP_Text _ammoText;

    private bool _isReloading = false;
    public bool IsReloading => _isReloading;

    void Start()
    {
        _currentBullets = _maxBullets;
        UpdateAmmoText();
    }

    public bool CanShoot()
    {
        return !_isReloading && _currentBullets > 0;
    }

    public void ConsumeBullet()
    {
        if (_isReloading)
            return;

        if (_currentBullets > 0)
        {
            _currentBullets--;

            if (_currentBullets <= 0)
            {
                UpdateAmmoText();
                StartCoroutine(ReloadCoroutine());
                return;
            }

            UpdateAmmoText();
        }
    }

    public void Reload()
    {
        if (_isReloading)
            return;
        if (_currentBullets == _maxBullets)
            return;

        StartCoroutine(ReloadCoroutine());
    }

    private IEnumerator ReloadCoroutine()
    {
        _isReloading = true;
        UpdateAmmoText();

        if (_animator != null)
        {
            _animator.SetTrigger("Reload");
        }

        yield return new WaitForSeconds(_reloadTime);

        _currentBullets = _maxBullets;
        _isReloading = false;
        UpdateAmmoText();
    }

    private void UpdateAmmoText()
    {
        if (_ammoText == null)
            return;

        _ammoText.text = $"{_currentBullets}/{_maxBullets}";
    }
}
