using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Sequence = DG.Tweening.Sequence;

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

    [Header("Respawn")]
    [SerializeField] private Image _respawnImage;
    [SerializeField] private float _targetX;
    [SerializeField] private float _originX;
    [SerializeField] private TextMeshProUGUI _respawnTimeText;
    private float _currentRespawnTime; // ���� �ð�

    [Header("Ranking")]
    [SerializeField] private Image _rankingPanel;
    [SerializeField] private Image _firstRankImage; // 1��
    [SerializeField] private TextMeshProUGUI _firstRankText;
    [SerializeField] private Image _secondRankImage; // 2��
    [SerializeField] private TextMeshProUGUI _secondRankText;
    [SerializeField] private Image _thirdRankImage; // 3��
    [SerializeField] private TextMeshProUGUI _thirdRankText;
    [SerializeField] private Image _playerRankImage; // Player
    [SerializeField] private TextMeshProUGUI _playerRankText;
    [SerializeField] private List<Material> _colorMatList = new List<Material>();

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
            _rankingPanel.gameObject.SetActive(!_gameController.IsOver);

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

        // Player Ranking Color Set
        foreach (Material mat in _colorMatList)
        {
            if (mat.name == $"{AgentManager.Instance.AgentColor}ParticleMat")
            {
                _playerRankImage.color = mat.color;
                break;
            }
        }
    }

    private void Update()
    {
        if (!_gameController.IsOver)
            UpdateRanking();
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
        _gameController.ChangeGameState(GameState.Over);
    }

    public void OpenRespawnUI()
    {
        int spawnDelayTime = 5;
        _currentRespawnTime = spawnDelayTime;
        UpdateRespawnText();

        Sequence sequence = DOTween.Sequence();
        sequence.Append(_respawnImage.rectTransform
            .DOAnchorPosX(_targetX, 0.6f).SetEase(Ease.InOutSine));
        sequence.Append(DOTween.To(() => _currentRespawnTime,
            x => _currentRespawnTime = x, 0, spawnDelayTime))
            .OnUpdate(UpdateRespawnText)
            .OnComplete(CloseRespawnUI);
    }

    private void UpdateRespawnText()
    {
        int currentTime = Mathf.CeilToInt(_currentRespawnTime);
        _respawnTimeText.text = $"Spawn Time...{currentTime.ToString()}";
    }

    private void CloseRespawnUI()
    {
        _respawnImage.rectTransform
            .DOAnchorPosX(_originX, 0.6f).SetEase(Ease.InSine)
            .SetUpdate(true);
    }

    private void UpdateRanking()
    {
        // �÷��̾� 3�� �ȿ� �ƴϸ� ��ũ ���� ����ְ� UI ���� ���Ե� �ؾ���
        // ���� �� �ۼ�Ʈ �԰� �ִ��� �˷������ GroundManager��

        // ���� �ٲ������
        // ���� Enemy_���� �̰ɷ� id���� ������
        // GroundManager�� id ����ִ� List����
        // _colorMatList���鼭 Ȯ���ϸ� �� (mat�̸� : ����ParticleMat)
        foreach (string s in GroundManager.Instance.idList)
        {
        //    if (s == $"Enemy_{�����̾��̸���}")
        //    {
        //        _playerRankImage.color = mat.color;
        //        break;
        //    }
        }
    }
}