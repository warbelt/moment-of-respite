using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    const string CONST_PLAYERPREFS_KEY_MAXSCORE = "savedMaxScore";
    const string CONST_PLAYERPREFS_TUTORIAL_SEEN = "tutorialSeen";
    const int CONST_TUTORIAL_SCENE = 1;

    [SerializeField] private PlayerController _player;
    [SerializeField] private UIController _uiController;
    [SerializeField] private ParticleSystem _bgParticles;
    [SerializeField] private EnemySpawner _enemySpawner;
    [SerializeField] private MusicManager _musicManager;

    [SerializeField] private TutorialPanel _tutorialPanel;

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

    Coroutine _activeCoroutine = null;

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

        _tutorialPanel.onTutorialEnded += finishTutorial;

        // Initialize Managers
        _enemySpawner.Initialize();
        _musicManager.ActivateMenuMusic();
    }

    private void OnDestroy()
    {
        SaveMaxScore();
    }
    public void tutorialButtonHandler()
    {
        setTutorialSeen(false);
        StartGame();
    }

    public void StartGame()
    {
        StartCoroutine(_uiController.ShiftUp());

        _player.onDeath += PlayerDead;
        _enemySpawner.onPointsGained += PointsGainedHandler;

        _player.enabled = true;
        _player.InitializeState();

        GameScore = 0;
        _round = 0;

        _musicManager.ActivateGamePlayMusic();

        _isPlaying = true;


        if (isTutorialSeen())
        {
            _activeCoroutine = StartCoroutine(StartRound());
        }
        else
        {
            _activeCoroutine = StartCoroutine(StartTutorial());
        }
    }

    public IEnumerator EnterRespite()
    {
        StartCoroutine(_musicManager.DimMusic(0.5f));
        _enemySpawner.StopSpawning();

        float activateDuration = 1;
        float respiteDuration = 10;

        _player.enabled = false;
        _player.SetRespiteShieldActive(true);
        StartCoroutine(_uiController.ActivateRespiteUI(activateDuration, respiteDuration));

        yield return new WaitForSeconds(activateDuration);

        var vel = _bgParticles.velocityOverLifetime;
        var emission = _bgParticles.emission;

        ParticleSystem.MinMaxCurve baseParticlesVelocityOverTimeSpeedModifier = vel.speedModifier;
        float baseParticlesRateOverTimeModifier = emission.rateOverTimeMultiplier;
        
        vel.speedModifier = 20;
        emission.rateOverTimeMultiplier = 5;

        yield return new WaitForSeconds(10);

        vel.speedModifier = baseParticlesVelocityOverTimeSpeedModifier;
        emission.rateOverTimeMultiplier = baseParticlesRateOverTimeModifier;

        _player.enabled = true;
        _player.SetRespiteShieldActive(false);
        _uiController.DisableRespiteUI();

        StartCoroutine(_musicManager.RestoreMusic(0.5f));
        _activeCoroutine = StartCoroutine(StartRound());
    }

    private IEnumerator StartRound()
    {
        _round += 1;
        _enemySpawner.StartSpawning(_round);

        yield return new WaitForSeconds(roundDuration);
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
        _musicManager.ActivateMenuMusic();
        StopCoroutine(_activeCoroutine);
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
        _enemySpawner.DespawnAllEnemies();
        _enemySpawner.DespawnAllEnemyProjectiles();

        _player.ResetGame();
        _uiController.ResetGame();
        
        GameScore = 0;
        _round = 0;

        StopAllCoroutines();
        _musicManager.ActivateGamePlayMusic();
        _activeCoroutine = StartCoroutine(StartRound());

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

    private bool isTutorialSeen()
    {
        return PlayerPrefs.GetInt(CONST_PLAYERPREFS_TUTORIAL_SEEN, 0) == 1;
    }

    private void setTutorialSeen(bool seen)
    {
        PlayerPrefs.SetInt(CONST_PLAYERPREFS_TUTORIAL_SEEN, seen? 1 : 0);
        PlayerPrefs.Save();
    }

    private IEnumerator StartTutorial()
    {
        _uiController.SetHiddenForTutorial(true);

        _tutorialPanel.gameObject.SetActive(true);

        yield return 0;
    }

    private void finishTutorial()
    {
        _tutorialPanel.gameObject.SetActive(false);
        _uiController.SetHiddenForTutorial(false);

        setTutorialSeen(true);
        _player.InitializeState();
        _activeCoroutine = StartCoroutine(StartRound());
    }
}
