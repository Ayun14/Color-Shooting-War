using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartSceneUI : MonoBehaviour
{
    [SerializeField] private Image _colorSelectPanel;
    [SerializeField] private Image _settingPanel;

    public void PlayButtonClick()
    {
        // ���̵��� ó��
        SceneManager.LoadScene("Main");
    }

    public void ExitButtonClick()
    {
    }
}
