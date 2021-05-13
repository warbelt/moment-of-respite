using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] EnemySpawnInfo[] _enemiesInfo;
    [SerializeField] float enemyStartY;
    [SerializeField] float enemyStartMinX;
    [SerializeField] float enemyStartMaxX;

    [SerializeField] float enemySpawnInterval;
    [SerializeField] float waveSpawnInterval;
    [SerializeField] int _maxEnemyWaveCount = 6;
    
    private bool _spawning;

    int _currentRound = 0;
    float _nextEnemySpawn;
    List<Enemy> _spawnedEnemies;

    public event Action<int> onPointsGained;


    private void Update()
    {
        if (Time.time >= _nextEnemySpawn && _spawning)
        {
            StartCoroutine(SpawnEnemyWave());

            _nextEnemySpawn = Time.time + waveSpawnInterval;
        }
    }

    public void Initialize()
    {
        _spawnedEnemies = new List<Enemy>();

        // Initialize Enemy Spawn
        _nextEnemySpawn = Time.time + waveSpawnInterval;
    }

    public void StartSpawning(int round)
    {
        _nextEnemySpawn = Time.time + waveSpawnInterval;
        _spawning = true;
        _currentRound = round;
    }

    public void StopSpawning()
    {
        _spawning = false;
    }

    private IEnumerator SpawnEnemyWave()
    {
        // Waves have 1 more enemy every 4 rounds
        int waveSize = Mathf.Min(UnityEngine.Random.Range(
            1 + (int) Math.Floor(_currentRound / 4f), 
            3 + (int) Math.Floor(_currentRound / 4f)
        ), _maxEnemyWaveCount);

        for (int i = 0; i < waveSize; i++)
        {
            Enemy selectedPrefabToSpawn = SelectEnemyToSpawn(_currentRound);

            Enemy enemyInstance = Instantiate(selectedPrefabToSpawn, new Vector3(UnityEngine.Random.Range(enemyStartMinX, enemyStartMaxX), enemyStartY, 0), Quaternion.Euler(0, 0, 180));
            enemyInstance.OnDeath += EnemyDeathHandler;
            enemyInstance.OnBoundaryExit += EnemyOutOfBoundaryhandler;
            _spawnedEnemies.Add(enemyInstance);
            yield return new WaitForSeconds(enemySpawnInterval);
        }

        yield return null;
    }

    private Enemy SelectEnemyToSpawn(int round)
    {
        Enemy toSpawn = null;

        while (toSpawn == null)
        {
            var enemyInfo = _enemiesInfo[UnityEngine.Random.Range(0, _enemiesInfo.Length)];
            if (enemyInfo.MinRound <= _currentRound && _currentRound <= enemyInfo.MaxRound)
            {
                toSpawn = enemyInfo.EnemyPrefab;
            }
        }

        return toSpawn;
    }

    public void DespawnAllEnemies()
    {
        foreach (Enemy enemy in _spawnedEnemies)
        {
            DespawnEnemy(enemy, false);
        }

        _spawnedEnemies.Clear();
    }

    private void DespawnEnemy(Enemy enemy, bool clearFromList = true)
    {
        if (clearFromList) _spawnedEnemies.Remove(enemy);
        Destroy(enemy.gameObject);
    }

    private void EnemyDeathHandler(Enemy caller)
    {
        onPointsGained?.Invoke(caller.GetPointsValue());
        _spawnedEnemies.Remove(caller);
        Destroy(caller.gameObject, 0.5f);
    }

    private void EnemyOutOfBoundaryhandler(Enemy enemy)
    {
        DespawnEnemy(enemy);
    }
}
