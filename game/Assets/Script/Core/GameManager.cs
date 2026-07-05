using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameManager : MonoBehaviour
{
    // Private Properties
    [SerializeField] private List<GameObject> spiderPrefabs;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float minSpawnRadius = 6f;
    [SerializeField] private float maxSpawnRadius = 10f;
    private Transform playerTransform;
    private ActBot player;


    // Private Methods
    private void SpawnSpider()
    {
        // Return if conditions for spider spawning are not met
        if (spiderPrefabs == null || spiderPrefabs.Count == 0 || playerTransform == null)
        {
            return;
        }


        // Spawn spider
        Vector2 spawnOffset = GetRandomSpawnOffset();
        Vector3 spawnPos = playerTransform.position + (Vector3)spawnOffset;
        int prefabIndex = UnityEngine.Random.Range(0, spiderPrefabs.Count);
        GameObject prefabToSpawn = spiderPrefabs[prefabIndex];
        Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
    }
    private Vector2 GetRandomSpawnOffset()
    {
        float angle = UnityEngine.Random.Range(0f, 360f);
        float radius = UnityEngine.Random.Range(minSpawnRadius, maxSpawnRadius);
        float radians = angle * Mathf.Deg2Rad;

        return new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * radius;
    }
    private void EndGame()
    {
        // Stop spawning spiders
        CancelInvoke(nameof(SpawnSpider));


        // Freeze game
        Time.timeScale = 0f;


        // End Game Message
        Debug.Log("You loose!");
    }


    // Override Methods
    private void Start()
    {
        // Get player via tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        playerTransform = playerObj.transform;
        player = playerObj.GetComponent<ActBot>();


        // End game when player dies
        player.OnDeath += EndGame;


        // Spawn Spiders in fixed intervals
        InvokeRepeating(nameof(SpawnSpider), spawnInterval, spawnInterval);
    }
}
