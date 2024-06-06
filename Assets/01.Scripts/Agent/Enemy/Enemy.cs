using UnityEngine;

public enum EnemyState
{
    Idle, Paint, Attack
}

public class Enemy : MonoBehaviour
{
    public EnemyStateMachine<EnemyState> StateMachine { get; private set; }

    #region Components
    [HideInInspector] public AgentAnimation EnemyAnimation { get; private set; }
    [HideInInspector] public EnemyMovement EnemyMovement { get; private set; }
    public EnemyGun EnemyGun;
    #endregion

    public bool isActive;

    [SerializeField] private LayerMask _whatIsPlayer;
    [SerializeField] private LayerMask _whatIsObstacle;
    [SerializeField] private LayerMask _whatIsNode;

    [Header("Attack Settings")]
    public float attackDistance;
    [HideInInspector] public Transform targetTrm;
    [HideInInspector] public CapsuleCollider colliderCompo;
    [HideInInspector] public Collider player;

    [Header("Paint Settings")]
    public Transform paintCheckTrm;
    public float paintDistance; // �̸�ŭ ������ �� ĥ���� �ִ���

    private void Awake()
    {
        EnemyAnimation = transform.Find("Visual").GetComponent<AgentAnimation>();
        EnemyMovement = GetComponent<EnemyMovement>();
        colliderCompo = GetComponent<CapsuleCollider>();

        // State ����
        StateMachine = new EnemyStateMachine<EnemyState>();
        StateMachine.AddState(EnemyState.Idle,
            new EnemyIdleState(this, StateMachine, EnemyState.Idle.ToString()));
        StateMachine.AddState(EnemyState.Paint,
            new EnemyPaintState(this, StateMachine, EnemyState.Paint.ToString()));
        StateMachine.AddState(EnemyState.Attack,
            new EnemyAttackState(this, StateMachine, EnemyState.Attack.ToString()));
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
    public Collider IsPlayerDetected()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position,
            attackDistance, _whatIsPlayer);

        foreach (Collider collider in colliders)
            return collider;

        return null;
    }

    // ���̿� ��ֹ��� �ִ���
    public bool IsObstacleInLine(float distance, Vector3 direction)
    {
        return Physics.Raycast(transform.position, direction, distance, _whatIsObstacle);
    }

    // �ֺ��� ĥ�� ���� �ִ��� (�̰� �Ⱦ��� ������
    public bool IsCanPaint()
    {
        RaycastHit[] colliders = Physics.SphereCastAll(paintCheckTrm.position,
            paintDistance, Vector3.up, 0, _whatIsNode);
        foreach (RaycastHit collider in colliders)
        {
            if (collider.transform.TryGetComponent(out GroundNode node))
                if (node.nodeId != transform.name) return false;
        }
        return true;
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
