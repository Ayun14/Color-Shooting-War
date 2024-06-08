using Cinemachine;
using UnityEngine;

public class CameraController : Observer
{
    [SerializeField] private CinemachineVirtualCamera _followCam;
    [SerializeField] private CinemachineVirtualCamera _topViewCam;

    private GameController _subject;

    public override void Notify(Subject subject)
    {
        if (_subject == null)
            _subject = subject as GameController;

        if (_subject != null)
            CameraChangeCheck();
    }

    private void CameraChangeCheck()
    {
        if (_subject.IsOver) // ���� ��� ����ִ� Top View Camrea �����ֱ�
        {
            _followCam.Priority = 0;
            _topViewCam.Priority = 1;
        }
        else // Follow Camera �����ֱ�
        {
            _topViewCam.Priority = 0;
            _followCam.Priority = 1;
        }
    }
}
