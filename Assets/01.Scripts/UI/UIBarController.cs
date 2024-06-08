using UnityEngine;
using UnityEngine.UI;

public abstract class UIBarController : MonoBehaviour
{
    [SerializeField] protected Health _health;
    [SerializeField] private Slider _HpSlider;

    public void HandleHpChange()
    {
        float value = (float)_health.CurrentHealth / _health.MaxHealth;
        _HpSlider.value = value;
    }

    private void LateUpdate() // ī�޶� �ٶ󺸱�
    {
        Transform mainCamTrm = Camera.main.transform;
        Vector3 lookDirection = transform.position + mainCamTrm.rotation * Vector3.forward;
        transform.LookAt(lookDirection, mainCamTrm.rotation * Vector3.up);
    }
}
