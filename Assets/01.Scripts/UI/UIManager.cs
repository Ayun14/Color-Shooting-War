using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Observer
{
    [Header("Countdown")]
    [SerializeField] private Image _countDownPanel;
    [SerializeField] private TextMeshProUGUI _countdownText;

    [Header("GameOver")]
    [SerializeField] private Image _ResultPanel;

    [Header("Playing")]
    [SerializeField] private Image _playerPanel;
    [SerializeField] private TextMeshProUGUI _restTimeText;
    [SerializeField] private int _playTime; // �÷��� �ð�
    private int _restTime; // ���� �ð�

    private GameController _gameController;

    public override void Notify(Subject subject)
    {
        if (_gameController == null)
            _gameController = subject as GameController;

        if (_gameController != null)
        {
            _countDownPanel.gameObject.SetActive(_gameController.IsCountdown);
            _playerPanel.gameObject.SetActive(!_gameController.IsOver);
            _ResultPanel.gameObject.SetActive(_gameController.IsOver);

            if (_gameController.IsCountdown)
                StartCoroutine(CountdownRoutine());
            else if (_gameController.IsPlaying)
                SetRestTime();
        }
    }

    private void Start()
    {
        // �÷��� �ð� �ʱ�ȭ
        _restTime = _playTime;
        UpdateRestText();
    }

    private IEnumerator CountdownRoutine()
    {
        int countdownDuration = 3;

        for (int i = countdownDuration; i > 0; i--)
        {
            _countdownText.text = i.ToString();
            yield return new WaitForSeconds(1.0f);
        }

        _gameController.ChangeGameState(GameState.Playing);
    }

    private void SetRestTime() // ���� �ð�
    {
        DOTween.To(() => _restTime, x => _restTime = x, 0, _playTime)
            .SetEase(Ease.Linear)
            .OnUpdate(UpdateRestText)
            .OnComplete(GameOver);
    }

    private void UpdateRestText()
    {
        int minutes = _restTime / 60;
        int seconds = _restTime % 60;
        _restTimeText.text = string.Format("{0:00} : {1:00}", minutes, seconds);

        if (_restTime <= 10)
        {
            _restTimeText.color = Color.red;
            // �Ҹ� ���ų� ��� ������
        }
        else
            _restTimeText.color = Color.white;
    }

    private void GameOver()
    {
        // �÷��̾�, �� �����������
        _gameController.ChangeGameState(GameState.Over);
    }
}