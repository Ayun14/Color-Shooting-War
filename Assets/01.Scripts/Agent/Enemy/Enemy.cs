using Unity.VisualScripting;
using UnityEngine;

public enum EnemyState
{
    Idle, Paint, Attack, Death
}

public class Enemy : MonoBehaviour
{
    public EnemyStateMachine<EnemyState> StateMachine { get; private set; }

    #region Components
    [HideInInspector] public AgentAnimation EnemyAnimation { get; private set; }
    [HideInInspector] public EnemyMovement EnemyMovement { get; private set; }
    [HideInInspector] public Health EnemyHealth { get; private set; }
    public EnemyGun EnemyGun;
    #endregion

    public bool isActive;

    [SerializeField] private LayerMask _whatIsPlayer;
    [SerializeField] private LayerMask _whatIsEnemy;
    [SerializeField] private LayerMask _whatIsObstacle;
    [SerializeField] private LayerMask _whatIsNode;

    [Header("Attack Settings")]
    public float attackDistance;
    [HideInInspector] public Transform targetTrm;
    [HideInInspector] public CapsuleCollider colliderCompo;
    [HideInInspector] public Collider target;

    [Header("Paint Settings")]
    public Transform paintCheckTrm;
    public float paintDistance; // �̸�ŭ ������ �� ĥ���� �ִ���

    // ������
    [HideInInspector] public Vector3 spawnPos;
    [HideInInspector] public float spawnDelayTime = 5f;
    [HideInInspector] public float currentSpawnDelayTime = 0;

    private void Awake()
    {
        EnemyAnimation = transform.Find("Visual").GetComponent<AgentAnimation>();
        EnemyMovement = GetComponent<EnemyMovement>();
        EnemyHealth = GetComponent<Health>();
        colliderCompo = GetComponent<CapsuleCollider>();

        // State ����
        StateMachine = new EnemyStateMachine<EnemyState>();
        StateMachine.AddState(EnemyState.Idle,
            new EnemyIdleState(this, StateMachine, EnemyState.Idle.ToString()));
        StateMachine.AddState(EnemyState.Paint,
            new EnemyPaintState(this, StateMachine, EnemyState.Paint.ToString()));
        StateMachine.AddState(EnemyState.Attack,
            new EnemyAttackState(this, StateMachine, EnemyState.Attack.ToString()));
        StateMachine.AddState(EnemyState.Death,
            new EnemyDeathState(this, StateMachine, EnemyState.Death.ToString()));
    }

    private void Start()
    {
        StateMachine.Initialize(EnemyState.Idle, this);
    }

    private void Update()
    {
        StateMachine.CurrentState.UpdateState();
    }

    // Ÿ���� ���� ���� �ȿ� ���Դ���
    public Collider IsTargetDetected()
    {
        int layerMask = _whatIsPlayer | _whatIsEnemy;

        Collider[] colliders = Physics.OverlapSphere
            (transform.position, attackDistance, layerMask);

        foreach(Collider collider in colliders)
        {
            if (collider.name == transform.name)
                continue;

            return collider;
        }

        return null;
    }

    // ���� ��ֹ��� �ִ���
    public bool IsObstacleInFront()
    {
        Collider[] colliders = Physics.OverlapSphere(paintCheckTrm.position,
            paintDistance, _whatIsObstacle);
        return colliders.Length > 0;
    }

    public void SetDeath()
    {
        StateMachine.ChangeState(EnemyState.Death);
        EnemyMovement.StopImmediately();
        colliderCompo.enabled = false;
    }

    public void GameStart()
    {
        StateMachine.ChangeState(EnemyState.Paint);
    }

    public void GameOver()
    {
        StateMachine.ChangeState(EnemyState.Idle);
    }

    public void ChangeRandomDirection()
    {
        Vector3 randDir = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        EnemyMovement.SetMovement(randDir);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(paintCheckTrm.position, paintDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
        Gizmos.color = Color.white;
    }
}
