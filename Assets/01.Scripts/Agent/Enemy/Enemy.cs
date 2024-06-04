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
    #endregion

    public float moveSpeed;
    private float _defaultMoveSpeed;
    public bool isActive;

    [SerializeField] private LayerMask _whatIsPlayer;
    [SerializeField] private LayerMask _whatIsObstacle;
    [SerializeField] private LayerMask _whatIsNode;

    [Header("Attack Settings")]
    public float runAwayDistance; //��׷ΰ� ������ ������ �Ÿ�
    public float attackDistance;
    [HideInInspector] public Transform targetTrm;
    [HideInInspector] public CapsuleCollider colliderCompo;

    [Header("Paint Settings")]
    public float paintDistance; // �̸�ŭ ������ �� ĥ���� �ִ���
    public float paintCheckDistance; // �󸶳� �տ��� üũ�Ұ���

    private void Awake()
    {
        _defaultMoveSpeed = moveSpeed;
        EnemyAnimation = transform.Find("Visual").GetComponent<AgentAnimation>();
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

    // �÷��̾ ����������
    public Collider IsPlayerDetected()
    {
        bool isHit = Physics.SphereCast(transform.position,
            runAwayDistance, Vector3.up, out RaycastHit hit, 0, _whatIsPlayer);
        return isHit ? hit.collider : null;
    }

    // ���̿� ��ֹ��� �ִ���
    public bool IsObstacleInLine(float distance, Vector3 direction)
    {
        return Physics.Raycast(transform.position, direction, distance, _whatIsObstacle);
    }

    // �ֺ��� ĥ�� ���� �ִ���
    public bool IsCanPaint()
    {
        RaycastHit[] colliders = Physics.SphereCastAll(transform.position, 
            paintDistance, Vector3.forward, paintCheckDistance, _whatIsNode);
        foreach (RaycastHit collider in colliders)
        {
            if (collider.transform.TryGetComponent(out GroundNode node))
                if (node.nodeId != transform.name) return false;
        }
        return true;
    }

    public void AnimationEndTrigger()
    {
        StateMachine.CurrentState.AnimationFinishTrigger();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, runAwayDistance); //�� �����Ÿ�
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
        Gizmos.color = Color.white;
    }
}
