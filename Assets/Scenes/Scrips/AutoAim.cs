using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AutoAim : MonoBehaviour
{
    [SerializeField]
    private Button _toggleAutoAimButton; // Button để toggle auto aim.

    [SerializeField]
    private TMP_Text _statusText; // Text hiển thị trạng thái auto aim.

    private Shotting _shotting; // Reference đến Shotting script.

    void Start()
    {
        _shotting = FindAnyObjectByType<Shotting>();

        // Kết nối button với toggle function
        _toggleAutoAimButton.onClick.AddListener(ToggleAutoAim);
    }

    void Update()
    {
        // Cập nhật status text
        if (_statusText != null)
        {
            if (_shotting.IsAutoAimEnabled)
            {
                if (_shotting.CurrentTarget != null)
                    _statusText.text = "Auto Aim: ON ";
            }
            else
            {
                _statusText.text = "Auto Aim: OFF";
            }
        }
    }

    void ToggleAutoAim()
    {
        _shotting.ToggleAutoAim();
    }
}
