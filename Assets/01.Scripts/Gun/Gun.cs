using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private Transform _shootTrm;
    [SerializeField] private ParticleSystem _shootParticle;
    [SerializeField] private PlayerInput _playerInput;

    private bool _isPainting = false;

    private int _paintAmount = 100; // ���� ��

    private void Start()
    {
        _playerInput.OnFireEvent += PlayPaintParticle;
        _playerInput.OnFireStopEvent += StotPaintParticle;

        _shootParticle.transform.position = _shootTrm.transform.position;
    }

    private void OnDestroy()
    {
        _playerInput.OnFireEvent -= PlayPaintParticle;
        _playerInput.OnFireStopEvent -= StotPaintParticle;
    }
    
    private void PlayPaintParticle()
    {
        _isPainting = true;
        _shootParticle.Play();
    }
    
    private void StotPaintParticle()
    {
        _isPainting = false;
        _shootParticle.Stop();
    }

    public void SetPaintAmount(int paintAmount, bool isBool = false)
    {
        int targetAmount = paintAmount;
        // �ڿ������� �ö󰡱�
        // _paintAmount = targetAmount;
    }
}
