using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private Slider slider;

    [SerializeField]
    private new Camera camera;

    [SerializeField]
    private Transform target;

    [SerializeField]
    private Vector3 offset; // Offset mặc định từ enemy đến health bar

    private void Awake()
    {
        if (camera == null)
            camera = Camera.main;
    }

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (slider == null || maxHealth <= 0f)
            return;

        slider.value = Mathf.Clamp01(currentHealth / maxHealth);
    }

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update()
    {
        if (camera == null)
            camera = Camera.main;

        if (camera != null)
            transform.rotation = camera.transform.rotation; // Luôn hướng về camera

        if (target != null)
            transform.position = target.position + offset; // Bám theo enemy và giữ offset
    }
}
