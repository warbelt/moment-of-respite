using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private PlayerController _player;
    [SerializeField] private UIController _uiController;
    [SerializeField] private ParticleSystem _bgParticles;

    [SerializeField] Enemy enemy;
    [SerializeField] float enemyStartY;
    [SerializeField] float enemyStartMinX;
    [SerializeField] float enemyStartMaxX;


    // Game configuration
    [SerializeField] float enemySpawnInterval;
    [SerializeField] float waveSpawnInterval;
    [SerializeField] float roundDuration = 10;

    //State
    bool _isPlaying;
    float nextEnemySpawn;
    List<Enemy> _spawnedEnemies;
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


    private void Awake()
    {
        _isPlaying = false;
        _player.enabled = false;
    }

    private void Start()
    {
        // Initialize UI
        _uiController.StartDown();
        StartCoroutine(StartRound());
        _uiController.OnUpgradeGaugeFull += UpgradePlayer;
        _uiController.OnReplayButtonPushed += ReplaySignal;

        // Initialize Enemy Spawn
        nextEnemySpawn = Time.time + waveSpawnInterval;
    }

    private void Update()
    {
        if (Time.time >= nextEnemySpawn)
        {
            StartCoroutine(Spawn());

            nextEnemySpawn = Time.time + waveSpawnInterval;
        }
    }

    private IEnumerator Spawn()
    {
        int waveSize = Random.Range(1, 5);

        for (int i = 0; i < waveSize; i++)
        {
            Enemy enemyInstance = Instantiate(enemy, new Vector3(Random.Range(enemyStartMinX, enemyStartMaxX), enemyStartY, 0), Quaternion.Euler(0,0,180));
            enemyInstance.OnDeath += EnemyDeathHandler;
            enemyInstance.OnBoundaryExit += EnemyOutOfBoundaryhandler;
            _spawnedEnemies.Add(enemyInstance);
            yield return new WaitForSeconds(enemySpawnInterval);
        }

        yield return null;
    }

    public void StartGame()
    {
        StartCoroutine(_uiController.ShiftUp());
        _player.onDeath += PlayerDead;
        _player.enabled = true;
        _player.InitializeState();
        GameScore = 0;
        _spawnedEnemies = new List<Enemy>();

        _isPlaying = true;
    }

    public IEnumerator EnterRespite()
    {
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
        yield return new WaitForSeconds(20);
        StartCoroutine(EnterRespite());
    }

    public void UpgradePlayer()
    {
        string upgradeText = _player.ApplyRandomUpgrade();
        StartCoroutine(_uiController.DisplayUpgradeText(upgradeText));
    }

    public void EnemyDeathHandler(Enemy caller)
    {
        GameScore += caller.GetPointsValue();
        _spawnedEnemies.Remove(caller);
        Destroy(caller, 0.5f);
    }

    private void SetScore(int score)
    {
        _uiController.SetScoreText(score);
    }

    private void PlayerDead()
    {
        _uiController.ActivateReplayButton();
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
        DespawnEnemies();

        _player.ResetGame();
        _uiController.ResetGame();
        
        GameScore = 0;
        StopAllCoroutines();
        StartCoroutine(StartRound());
    }

    private void DespawnEnemies()
    {
        foreach(Enemy enemy in _spawnedEnemies)
        {
            DespawnEnemy(enemy, false);
        }

        _spawnedEnemies.Clear();
    }

    private void EnemyOutOfBoundaryhandler(Enemy enemy)
    {
        DespawnEnemy(enemy);
    }

    private void DespawnEnemy(Enemy enemy, bool clearFromList = true)
    {
        if (clearFromList) _spawnedEnemies.Remove(enemy);
        Destroy(enemy.gameObject);
    }
}
