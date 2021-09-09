using UnityEngine;
using System;

[Serializable]
public struct EnemySpawnInfo
{
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private int minRound;
    [SerializeField] private int maxRound;

    public Enemy EnemyPrefab => enemyPrefab;
    public int MinRound => minRound;
    public int MaxRound => maxRound;
}