using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class GroundManager : Singleton<GroundManager>
{
    [HideInInspector] public List<string> idList = new List<string>();
    private List<GroundNode> _groundNodeList = new List<GroundNode>();

    [SerializeField] private LayerMask _node;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log(GroundResult("Player"));
        }
    }

    public void AddNodeList(GroundNode node)
    {
        _groundNodeList.Add(node);
    }

    // ���� ������ ����
    public void GroundPainted(Vector3 pos, float radius, string id)
    {
        idList.Contains(id);

        RaycastHit[] result =
            Physics.SphereCastAll(pos, radius, Vector3.up, 0, _node);

        foreach (RaycastHit hit in result)
        {
            if (hit.transform.TryGetComponent(out GroundNode node))
                node.nodeId = id;
        }
    }

    // Id�� ���� �� ���� �ۼ�Ʈ
    public float GroundResult(string id)
    {
        float sum = 0;

        foreach (GroundNode node in _groundNodeList)
            if (node.nodeId == id) ++sum;

        return (sum / _groundNodeList.Count) * 100f;
        //string result = sum.ToString("F2"); �Ҽ��� �ӷ� 2�ڸ� ���� ���̴°�
    }
}
