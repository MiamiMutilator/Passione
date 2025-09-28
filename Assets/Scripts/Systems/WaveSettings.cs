using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Wave Data", menuName = "ScriptableObjects/Wave Data", order = 1)]
public class WaveSettings : ScriptableObject
{
    [Tooltip("The max amount of enemies that can be active at one time")]
    public int maxConcurrentEnemies;
    public EnemySpawnInfo[] enemySpawns;

    [Serializable]
    public class EnemySpawnInfo
    {
        public GameObject enemyPrefab;
        public float timeToSpawn;
    }
}
