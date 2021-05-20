using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameController : MonoBehaviour
{
    string CONST_PLAYERPREFS_KEY_MAXSCORE = "savedMaxScore";


    [SerializeField] private PlayerController _player;
    [SerializeField] private UIController _uiController;
    [SerializeField] private ParticleSystem _bgParticles;
    [SerializeField] private EnemySpawner _enemySpawner;

    // Game configuration
    [SerializeField] float roundDuration = 10;

    //State
    bool _isPlaying;
    int _maxScore;
    int MaxScore
    {
        get => _maxScore;
        set
        {
            _maxScore = value;
            SetMaxScore(_maxScore);
        }
    }

    int _gameScore;
    int GameScore
    {
        get { return _gameScore; }
        set
        {
            _gameScore = value;
            SetScore(_gameScore);
        }
    }
    int _round;

    private void Awake()
    {
        _isPlaying = false;
        _player.enabled = false;
        MaxScore = LoadMaxScore();
    }

    private void Start()
    {
        // Initialize UI
        _uiController.StartDown();
        _uiController.OnUpgradeGaugeFull += UpgradePlayer;
        _uiController.OnReplayButtonPushed += ReplaySignal;

        // Initialize Managers
        _enemySpawner.Initialize();

        // Start game
        StartCoroutine(StartRound());
    }

    private void OnDestroy()
    {
        SaveMaxScore();
    }

    public void StartGame()
    {
        StartCoroutine(_uiController.ShiftUp());
        _player.onDeath += PlayerDead;
        _player.enabled = true;
        _player.InitializeState();
        GameScore = 0;
        _round = 0;

        _enemySpawner.onPointsGained += PointsGainedHandler;
        _isPlaying = true;
    }

    public IEnumerator EnterRespite()
    {
        _enemySpawner.StopSpawning();

        float activateDuration = 1;
        float respiteDuration = 10;

        _player.enabled = false;
        StartCoroutine(_uiController.ActivateRespiteUI(activateDuration, respiteDuration));

        yield return new WaitForSeconds(activateDuration);

        var vel = _bgParticles.velocityOverLifetime;
        vel.speedModifier = 20;
        var emission = _bgParticles.emission;
        emission.rateOverTimeMultiplier = 5;
        var particleMain = _bgParticles.main;

        yield return new WaitForSeconds(10);

        vel.speedModifier = 1;
        emission.rateOverTimeMultiplier = 1;

        _player.enabled = true;
        _uiController.DisableRespiteUI();

        StartCoroutine(StartRound());
    }

    private IEnumerator StartRound()
    {
        _round += 1;
        _enemySpawner.StartSpawning(_round);

        yield return new WaitForSeconds(30);
        StartCoroutine(EnterRespite());
    }

    public void UpgradePlayer()
    {
        string upgradeText = _player.ApplyRandomUpgrade();
        StartCoroutine(_uiController.DisplayUpgradeText(upgradeText));
    }

    private void SetScore(int score)
    {
        _uiController.SetScoreText(score);
    }

    private void SetMaxScore(int maxScore)
    {
        _uiController.SetMaxScoreText(maxScore);
    }

    private void CheckMaxScore()
    {
        if (GameScore > MaxScore)
        {
            MaxScore = GameScore;
        }
    }

    private void PlayerDead()
    {
        _uiController.ActivateReplayMenu();
        CheckMaxScore();
    }

    private void ReplaySignal()
    {
        if(_isPlaying)
        {
            RestartGame();
        }
    }

    private void RestartGame()
    {
        StopAllCoroutines();
        
        _enemySpawner.DespawnAllEnemies();
        _enemySpawner.DespawnAllEnemyProjectiles();

        _player.ResetGame();
        _uiController.ResetGame();
        
        GameScore = 0;
        _round = 0;

        StartCoroutine(StartRound());
    }

    private void PointsGainedHandler(int points)
    {
        GameScore += points;
    }

    private void SaveMaxScore()
    {
        PlayerPrefs.SetInt(CONST_PLAYERPREFS_KEY_MAXSCORE, MaxScore);
    }

    private int LoadMaxScore()
    {
        return PlayerPrefs.GetInt(CONST_PLAYERPREFS_KEY_MAXSCORE, 0);
    }
}
