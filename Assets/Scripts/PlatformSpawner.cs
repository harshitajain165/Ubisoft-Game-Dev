using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    [Header("Platform Prefabs")]
    public GameObject movingPlatformPrefab;

    [Header("Spawn Settings")]
    public Transform player; 
    public float spawnDistance = 10f; // Distance ahead of the player to spawn
    public int numberOfPlatforms = 2; 
    public float platformSpacing = 10f; 
    public Vector2 verticalRange = new Vector2(-2f, 2f); 

    private float nextSpawnX = 0f;

    private void Update()
    {
        SpawnPlatformsIfNeeded();
    }

    private void SpawnPlatformsIfNeeded()
    {
        if (player.position.x + spawnDistance > nextSpawnX)
        {
            for (int i = 0; i < numberOfPlatforms; i++)
            {
                SpawnMovingPlatform();
            }
        }
    }

    private void SpawnMovingPlatform()
    {
        float spawnX = nextSpawnX + platformSpacing;
        float spawnY = Random.Range(verticalRange.x, verticalRange.y);
        Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0);
        
        Instantiate(movingPlatformPrefab, spawnPosition, Quaternion.identity);
        
        nextSpawnX = spawnX;
    }
}