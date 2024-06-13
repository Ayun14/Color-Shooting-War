using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;
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
    [SerializeField] private float _respawnTargetX;
    [SerializeField] private float _respawnOriginX;
    [SerializeField] private TextMeshProUGUI _respawnTimeText;
    private int _spawnDelayTime = 5; // ���� �ð�

    [Header("Ranking")]
    [SerializeField] private Image _rankingPanel;
    [SerializeField] private List<Image> _rankImageList = new List<Image>();
    [SerializeField] private List<TextMeshProUGUI> _rankTextList = new List<TextMeshProUGUI>();
    
    [SerializeField] private Image _playerRankImage; // Player
    [SerializeField] private TextMeshProUGUI _playerRankText;
    [SerializeField] private float _rankTargetX;
    [SerializeField] private float _rankOriginX;
    private Color _playerRankColor;

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
            _rankingPanel.gameObject.SetActive(_gameController.IsPlaying);

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
                _playerRankColor = mat.color;
                break;
            }
        }
    }

    private void LateUpdate()
    {
        //if (_gameController.IsPlaying)
            //UpdateRanking();
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
        _respawnTimeText.text = $"Spawn Time...{_spawnDelayTime}";

        _respawnImage.rectTransform
            .DOAnchorPosX(_respawnTargetX, 0.6f).SetEase(Ease.InOutSine);
        StartCoroutine(SpawnCountdownRoutine());
    }

    private IEnumerator SpawnCountdownRoutine()
    {
        for (int i = _spawnDelayTime; i >= 0; --i)
        {
            _respawnTimeText.text = $"Spawn Time...{i}";

            if (i == 0)
                break;

            yield return new WaitForSeconds(1f);
        }

        CloseRespawnUI();
    }

    private void CloseRespawnUI()
    {
        _respawnImage.rectTransform
            .DOAnchorPosX(_respawnOriginX, 0.6f).SetEase(Ease.InSine);
    }

    private void UpdateRanking()
    {
        Dictionary<string, float> ranking = GroundManager.Instance.GroundRanking();
        
        // �� �������� ���� ������ ����
        ranking = ranking.OrderByDescending(item => item.Value)
            .ToDictionary(x => x.Key, x => x.Value);

        int rank = 0;
        int playerRank = 0;
        foreach (var entry in ranking)
        {
            if (rank < 3)
            {
                foreach (Material mat in _colorMatList)
                {
                    if ($"{entry.Key}ParticleMat"
                        == $"Enemy_{mat.name}")
                    {
                        _rankImageList[rank].color = mat.color;

                        string result = entry.Value.ToString("F2");
                        _rankTextList[rank].text = $"{rank + 1}  -  {result}%";
                        break;
                    }
                }
            }

            if (entry.Key == "Player")
                playerRank = rank;

            rank++;
        }

        // Player
        if (playerRank < 3)
        {
            if (_playerRankImage.rectTransform.position.x != _rankOriginX)
            {
                _playerRankImage.rectTransform
                    .DOAnchorPosX(_rankOriginX, 0.6f)
                    .SetEase(Ease.InSine); // out
            }

            _rankImageList[rank].color = _playerRankColor;
            string result = ranking["Player"].ToString("F2");
            _rankTextList[rank].text = $"{rank + 1}  -  {result}%";
        }
        else
        {
            if (_playerRankImage.rectTransform.position.x != _rankTargetX)
            {
                _playerRankImage.rectTransform
                    .DOAnchorPosX(_rankTargetX, 0.6f)
                    .SetEase(Ease.InOutSine); // in
            }

            string result = ranking["Player"].ToString("F2");
            _playerRankText.text = $"{playerRank + 1}  -  {result}%  Player";
        }
    }
}