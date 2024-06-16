using System;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class Gun : MonoBehaviour
{
    public Action<float> OnPaintChange;

    [SerializeField] protected Transform _shootTrm;
    [SerializeField] protected ParticleSystem _shootParticle;

    protected bool _isPainting = false;

    protected float _usePaintAmount = 2; // ��� ���Ǵ� ����Ʈ ��
    protected float _fillPaintAmount = 2; // �Ƚ�� ä������ ����Ʈ ��
    protected float _currentPaintAmount = 0;
    protected float _paintMax = 100;

    protected float _currentPaintingTime = 0;
    protected float _paintTime = 0.3f; // ����Ʈ ��� ��

    protected float _currentFillingTime = 0;
    protected float _fillTime = 0.5f; // ����Ʈ ä������ ��

    protected virtual void Start()
    {
        _currentPaintAmount = _paintMax;
        _shootParticle.transform.position = _shootTrm.transform.position;
    }

    protected virtual void Update()
    {
        if (_isPainting)
        {
            _currentPaintingTime += Time.deltaTime;
            if (_currentPaintingTime >= _paintTime)
            {
                _currentPaintingTime = 0;
                SetPaintAmount(_usePaintAmount);
            }

            if (_currentPaintAmount <= 0)
                StopPaintParticle();
        }
        else
        {
            _currentFillingTime += Time.deltaTime;
            if (_currentFillingTime >= _fillTime)
            {
                _currentFillingTime = 0;
                SetPaintAmount(-_fillPaintAmount);
            }
        }
    }

    public void PlayPaintParticle()
    {
        if (_currentPaintAmount <= 0) return;

        _isPainting = true;
        _shootParticle.Play();
    }

    public void StopPaintParticle()
    {
        _isPainting = false;
        _shootParticle.Stop();
    }

    private void SetPaintAmount(float paintAmount)
    {
        _currentPaintAmount -= paintAmount;
        float paintValue = _currentPaintAmount / _paintMax;
        OnPaintChange?.Invoke(paintValue);

        if (_currentPaintAmount > _paintMax)
            _currentPaintAmount = _paintMax;
    }
}
