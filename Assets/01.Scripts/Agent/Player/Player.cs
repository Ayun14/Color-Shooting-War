using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer _renderer; // �Ǻλ�
    [HideInInspector] public PlayerInput PlayerInput { get; private set; }
    [HideInInspector] public AgentMovement PlayerMovement { get; private set; }
    [HideInInspector] public PlayerParticleController PlayerParticleController { get; private set; }
    [HideInInspector] public Health PlayerHealth { get; private set; }
    [HideInInspector] public PlayerAnimation PlayerAnimation { get; private set; }
    public AgentGun AgentGun;
    private CapsuleCollider _collider;

    private Vector3 _spawnPos; // ����, �������Ǵ� ���

    private float _spawnDelayTime = 6f;

    private void Awake()
    {
        _collider = GetComponent<CapsuleCollider>();
        PlayerInput = GetComponent<PlayerInput>();
        PlayerMovement = GetComponent<AgentMovement>();
        PlayerHealth = GetComponent<Health>();
        PlayerAnimation = transform.Find("Visual").GetComponent<PlayerAnimation>();
        PlayerParticleController = GetComponent<PlayerParticleController>();
    }

    private void Start()
    {
        _spawnPos = transform.position;

        Material mat = AgentManager.Instance.GetAgentMat();
        if (mat != null)
        {
            _renderer.material = mat;
            PlayerParticleController.PaintColorSet(mat);
        }
    }

    public void PlayerColorSet(Material mat)
    {
        _renderer.material = mat;
    }

    public void Respawn()
    {
        StartCoroutine(RespawnRoutine(_spawnDelayTime));
    }

    private IEnumerator RespawnRoutine(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        PlayerInput.SetPlayerInput(true);
        _collider.enabled = true;
        PlayerHealth.HealthReset();

        PlayerAnimation.ChangeAnimation(AnimationType.Idle.ToString());
        transform.position = _spawnPos;
    }

    public void SetDeath()
    {
        AgentGun.StopPaintParticle();
        PlayerInput.SetPlayerInput(false);
        PlayerMovement.StopImmediately();
        _collider.enabled = false;
    }

    public void SetGameOver()
    {
        AgentGun.StopPaintParticle();
        SetDeath();
    }
}
