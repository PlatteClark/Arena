using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] float spawnTime;
    [SerializeField] float spawnRange;
    [SerializeField] Transform player;

    float timer;

    void Start()
    {
        timer = spawnTime;
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if(timer <= 0)
        {
            SpawnEnemy();
            timer = spawnTime;
        }
    }

    void SpawnEnemy()
    {
        if (Utilities.RandomPoint(player.transform.position, spawnRange, out Vector3 pos))
        {
            GameObject.Instantiate(enemyPrefab, pos, Quaternion.identity);
        }
    }

    
}
