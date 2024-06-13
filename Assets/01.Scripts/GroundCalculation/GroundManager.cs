using System.Collections.Generic;
using UnityEngine;

public class GroundManager : Singleton<GroundManager>
{
    private Dictionary<string, float> _rankingDictionary 
        = new Dictionary<string, float>();
    private List<GroundNode> _groundNodeList = new List<GroundNode>();

    [SerializeField] private LayerMask _node;

    public void AddNodeList(GroundNode node)
    {
        _groundNodeList.Add(node);
    }

    // ���� ������ ����
    public void GroundPainted(Vector3 pos, float radius, string id)
    {
        _rankingDictionary.ContainsKey(id);

        RaycastHit[] result =
            Physics.SphereCastAll(pos, radius, Vector3.up, 0, _node);

        foreach (RaycastHit hit in result)
        {
            if (hit.transform.TryGetComponent(out GroundNode node))
                node.nodeId = id;
        }
    }

    // �� �������� �̸�, �������� �Ѱ���
    public Dictionary<string, float> GroundRanking()
    {
        foreach (KeyValuePair<string, float> pair in _rankingDictionary)
        {
            float persent = GroundResult(pair.Key);
            _rankingDictionary[pair.Key] = persent;
        }

        return _rankingDictionary;
    }

    // Id�� ���� �� ���� �ۼ�Ʈ
    private float GroundResult(string id)
    {
        float sum = 0;

        foreach (GroundNode node in _groundNodeList)
            if (node.nodeId == id) ++sum;

        return (sum / _groundNodeList.Count) * 100f;
    }
}
