using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static bool shuttingDown = false;
    private static T instance = null;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (T)FindAnyObjectByType(typeof(T));
                if (instance == null)
                {
                    GameObject temp = new GameObject(typeof(T).ToString());
                    instance = temp.AddComponent<T>();
                }
                DontDestroyOnLoad(instance);
            }
            return instance;
        }
    }

    private void OnApplicationQuit()//����� ���� ���� ��������
    {
        instance = null;
        shuttingDown = true;
    }

    private void OnDestroy()
    {
        instance = null;
        shuttingDown = true;
    }
}
