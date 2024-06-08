using Cinemachine;
using UnityEngine;

public class CameraController : Observer
{
    [SerializeField] private CinemachineVirtualCamera _followCam;
    [SerializeField] private CinemachineVirtualCamera _topViewCam;

    private GameController _gameController;

    public override void Notify(Subject subject)
    {
        if (_gameController == null)
            _gameController = subject as GameController;

        if (_gameController != null)
            CameraChangeCheck();
    }

    private void CameraChangeCheck()
    {
        if (_gameController.IsOver) // ���� ��� ����ִ� Top View Camrea �����ֱ�
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
