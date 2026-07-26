using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;


[Serializable]
public class GameManager : MonoBehaviour
{
    // Private Properties
    [SerializeField] private List<GameObject> spiderPrefabs;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float minSpawnRadius = 6f;
    [SerializeField] private float maxSpawnRadius = 10f;
    [SerializeField] private UIDocument gameOverDocument;
    [SerializeField] private string gameOverRootName = "game-over-root";
    [SerializeField] private string restartButtonName = "restart-button";
    private Transform playerTransform;
    private Player player;
    private VisualElement gameOverRoot;
    private Button restartButton;


    // Private Methods
    private void SpawnSpider()
    {
        // Return if conditions for spider spawning are not met
        if (spiderPrefabs == null || spiderPrefabs.Count == 0 || playerTransform == null)
        {
            Debug.LogWarning("SpawnSpider skipped, missing prefabs or player transform");
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


        // Show game over screen
        ShowGameOverScreen();
    }
    private void ShowGameOverScreen()
    {
        // Return if game over root is missing
        if (gameOverRoot == null)
        {
            Debug.LogWarning("ShowGameOverScreen skipped, game over root not found");
            return;
        }

        gameOverRoot.style.display = DisplayStyle.Flex;
    }
    private void RestartGame()
    {
        // Unfreeze game before reload
        Time.timeScale = 1f;


        // Reload current scene
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }


    // Override Methods
    private void Start()
    {
        // Get player via tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        playerTransform = playerObj.transform;
        player = playerObj.GetComponent<Player>();


        // End game when player dies
        player.OnDeath += EndGame;


        // Spawn Spiders in fixed intervals
        InvokeRepeating(nameof(SpawnSpider), spawnInterval, spawnInterval);


        // Setup game over UI references
        SetupGameOverUI();
    }
    private void SetupGameOverUI()
    {
        gameOverDocument = GetComponent<UIDocument>();
        VisualElement root = gameOverDocument.rootVisualElement;
        gameOverRoot = root.Q<VisualElement>(gameOverRootName);
        restartButton = root.Q<Button>(restartButtonName);


        // Return if expected elements are missing
        if (gameOverRoot == null || restartButton == null)
        {
            Debug.LogWarning("SetupGameOverUI skipped, expected elements not found in UI document");
            return;
        }

        restartButton.clicked += RestartGame;
    }
}
