using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance = null;
    private static bool isDestroyed = false;

    public static T Instance
    {
        get
        {
            if (isDestroyed)
                _instance = null; //�̹� �ı��Ǿ��ٸ� nulló���ϰ� �ٽ� ã�ƶ�.

            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<T>();
                if (_instance == null)
                    Debug.LogError($"{typeof(T).Name} singleton is not exist");
                else
                    isDestroyed = false;
            }

            return _instance;
        }
    }

    private void OnDestroy()
    {
        isDestroyed = true;
    }
}