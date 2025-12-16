using System;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    public bool spawnObstacles = true;

    [Header("Timing Settings")]
    [Tooltip("Average time (in seconds) between spawns. Higher = Less Often.")]
    public float spawnIntervalMean = 5.0f; // Increased for "Less Often"

    [Tooltip("How much the time varies. Higher = More Randomness.")]
    public float spawnIntervalStd = 3.5f;  // Increased for "Bigger Randomness"

    [Header("Object Settings")]
    public float3 spawnOffset = new float3(0.0f, 0.0f, 0.0f);
    public float spawnSize = 1.0f;
    public string spawnLayer = "Obstacle";
    public GameObject obstaclePrefab;

    private float spawnAccumulator = 0.0f;
    private float nextSpawnIn = 0.0f;
    private int obstacle_count = 0;
     private float originalMean;
    private float originalStd;

    void Start()
    {
        originalMean = spawnIntervalMean;
        originalStd = spawnIntervalStd;
        ResetSpawn();
    }

    void Update()
    {
        if (spawnObstacles)
        {
            spawnAccumulator += Time.deltaTime;

            if (spawnAccumulator >= nextSpawnIn)
            {
                SpawnObstacle();
                
                // Hard reset the timer to prevent "bursts" of obstacles
                spawnAccumulator = 0.0f;
                
                // Calculate when the next spawn happens
                CalculateNextSpawnTime();
            }
        }
    }

    private void CalculateNextSpawnTime()
    {
        if (obstacle_count > 0 && obstacle_count % 5 == 0)
        {
            spawnIntervalStd = Mathf.Max(3.0f, spawnIntervalStd - 0.1f);
        }

        if (obstacle_count > 0 && obstacle_count % 10 == 0)
        {
            spawnIntervalMean = Mathf.Max(1.0f, spawnIntervalMean - 0.1f);
        }

        float randomDelay = RandomNormal(spawnIntervalMean, spawnIntervalStd);

        nextSpawnIn = Mathf.Max(0.1f, randomDelay);
    }


    public void SpawnObstacle()
    {
        if (obstaclePrefab == null) return;

        obstacle_count++;

        var obstacle = Instantiate(obstaclePrefab, transform);

        var spawnDown = RandomBool();
        obstacle.transform.position += (Vector3)(spawnDown ? spawnOffset + (1.0f - spawnSize) / 2.0f : -spawnOffset - (1.0f - spawnSize) / 2.0f);
        
        obstacle.transform.localScale = new Vector3(spawnSize, spawnSize, spawnSize);
        
        // Safety check for layers
        int layerID = LayerMask.NameToLayer(spawnLayer);
        if (layerID >= 0) 
        {
            obstacle.layer = layerID;
        }
    }

    public void ClearObstacles()
    {
        var obstacleLayer = LayerMask.NameToLayer(spawnLayer);
        // Iterate backwards when destroying to avoid collection modification errors
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            GameObject child = transform.GetChild(i).gameObject;
            if (child.layer == obstacleLayer)
            {
                Destroy(child);
            }
        }
    }
    
    public void ResetSpawn()
    {
        spawnAccumulator = 0.0f;
        obstacle_count = 0;
        
        // Restore the difficulty to the start values
        spawnIntervalMean = originalMean;
        spawnIntervalStd = originalStd;
        
        CalculateNextSpawnTime();
    }

    public void ModifyObstacleSpeed(float multiplier)
    {
        var obstacleLayer = LayerMask.NameToLayer(spawnLayer);
        var xMultiplier = new Vector2(multiplier, 1.0f);
        
        foreach (Transform child in transform)
        {
            if (child.gameObject.layer == obstacleLayer) 
            {
                var rb = child.GetComponent<Rigidbody2D>();
                if(rb != null) rb.velocity *= xMultiplier;
            }
        }
    }

    public static float RandomNormal(float mean, float std)
    {
        var v1 = 1.0f - Random.value;
        var v2 = 1.0f - Random.value;
        
        var standard = Math.Sqrt(-2.0f * Math.Log(v1)) * Math.Sin(2.0f * Math.PI * v2);
        
        return (float)(mean + std * standard);
    }
    
    public static bool RandomBool()
    {
        return Random.value >= 0.5f;
    }
}