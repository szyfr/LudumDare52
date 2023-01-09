using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;

    float ranX;
    Vector2 spawnLocation;
    private float spawnRate = 4f;
    public float nextSpawn = 0.0f;
    int maxSpawns = 8;
    // Start is called before the first frame update
    void Start()
    {
        spawnLocation = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time > nextSpawn)
        {
            if (GameObject.FindGameObjectsWithTag("enemy").Length < maxSpawns)
            {
                nextSpawn = Time.time + spawnRate;
                Instantiate(enemyPrefab, spawnLocation, Quaternion.identity);
            }
        }
    }
}
