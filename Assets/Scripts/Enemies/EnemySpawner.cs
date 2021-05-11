using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] Enemy enemy;
    [SerializeField] float enemyStartY;
    [SerializeField] float enemyStartMinX;
    [SerializeField] float enemyStartMaxX;

    [SerializeField] float enemySpawnInterval;
    [SerializeField] float waveSpawnInterval;

    private bool _spawning;

    float nextEnemySpawn;
    List<Enemy> _spawnedEnemies;

    public event Action<int> onPointsGained;

    private void Update()
    {
        if (Time.time >= nextEnemySpawn && _spawning)
        {
            StartCoroutine(Spawn());

            nextEnemySpawn = Time.time + waveSpawnInterval;
        }
    }

    private IEnumerator Spawn()
    {
        int waveSize = UnityEngine.Random.Range(1, 5);

        for (int i = 0; i < waveSize; i++)
        {
            Enemy enemyInstance = Instantiate(enemy, new Vector3(UnityEngine.Random.Range(enemyStartMinX, enemyStartMaxX), enemyStartY, 0), Quaternion.Euler(0, 0, 180));
            enemyInstance.OnDeath += EnemyDeathHandler;
            enemyInstance.OnBoundaryExit += EnemyOutOfBoundaryhandler;
            _spawnedEnemies.Add(enemyInstance);
            yield return new WaitForSeconds(enemySpawnInterval);
        }

        yield return null;
    }

    public void StartSpawning()
    {
        nextEnemySpawn = Time.time + waveSpawnInterval;
        _spawning = true;
    }

    public void StopSpawning()
    {
        _spawning = false;
    }

    public void Initialize()
    {
        _spawnedEnemies = new List<Enemy>();

        // Initialize Enemy Spawn
        nextEnemySpawn = Time.time + waveSpawnInterval;
    }

    public void EnemyDeathHandler(Enemy caller)
    {
        onPointsGained?.Invoke(caller.GetPointsValue());
        _spawnedEnemies.Remove(caller);
        Destroy(caller.gameObject, 0.5f);
    }

    private void EnemyOutOfBoundaryhandler(Enemy enemy)
    {
        DespawnEnemy(enemy);
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
}
