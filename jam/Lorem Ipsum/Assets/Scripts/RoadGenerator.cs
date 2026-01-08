using System.Collections.Generic;
using UnityEngine;

public class RoadGenerator : MonoBehaviour
{
    public GameObject roadPrefab;
    public GameObject[] obstaclePrefabs; 
    public Transform playerTransform;
    
    private List<GameObject> activeRoads = new List<GameObject>();
    private float[] lanes = { -20.5f, -12.5f, -3.5f, 4.5f }; 
    
    public float roadLength = 50f;
    public int maxRoadsOnScreen = 5;
    public int obstaclesPerSegment = 3; 
    private float spawnZ = 0f;

    void Start()
    {
        for (int i = 0; i < maxRoadsOnScreen; i++)
        {
            SpawnRoad(i < 2); 
        }
    }

    void Update()
    {
        float safeZone = 30f; 

        if (playerTransform.position.z > (spawnZ - (maxRoadsOnScreen * roadLength)))
        {
            SpawnRoad(false);
        }

        if (activeRoads.Count > 0)
        {
            if (playerTransform.position.z > activeRoads[0].transform.position.z + roadLength + safeZone)
            {
                DeleteOldRoad();
            }
        }
    }

    void SpawnRoad(bool isSafe)
    {
        GameObject go = Instantiate(roadPrefab, transform.forward * spawnZ, Quaternion.identity);
        activeRoads.Add(go);

        if (!isSafe)
        {
            SpawnObstaclesOnRoad(go);
        }

        spawnZ += roadLength;
    }

    void SpawnObstaclesOnRoad(GameObject roadSegment)
    {
        for (int i = 0; i < obstaclesPerSegment; i++)
        {
            // Pick a random prefab
            GameObject prefabToSpawn = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];

            // Pick a random lane
            float randomX = lanes[Random.Range(0, lanes.Length)];

            // Pick a random Z position within current road segment
            float randomZ = Random.Range(roadSegment.transform.position.z, roadSegment.transform.position.z + roadLength);

            Vector3 spawnPos = new Vector3(randomX, 0.5f, randomZ); // Adjust Y (0.5f) to match your road height

            // Spawn 
            GameObject obstacle = Instantiate(prefabToSpawn, spawnPos, Quaternion.Euler(0, 90, 0));

            // Child of road
            obstacle.transform.SetParent(roadSegment.transform);
        }
    }

    void DeleteOldRoad()
    {
        Destroy(activeRoads[0]);
        activeRoads.RemoveAt(0);
    }
}