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
        if (RandomPoint(player.transform.position, spawnRange, out Vector3 pos))
        {
            GameObject.Instantiate(enemyPrefab, pos, Quaternion.identity);
            Debug.DrawRay(pos, Vector3.up, Color.blue, 1.0f);
        }
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }
}
