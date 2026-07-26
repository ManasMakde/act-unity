using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;


[Serializable]
public struct Range
{
    public float min;
    public float max;
}


[Serializable]
public class GameManager : MonoBehaviour
{
    // Private Properties
    [SerializeField] private List<GameObject> spiderPrefabs;
    [SerializeField] private Range spawnInterval = new Range { min = 5f, max = 10f };
    [SerializeField] private int maxSpawnCount = 3;
    [SerializeField] private float minSpawnRadius = 6f;
    [SerializeField] private float maxSpawnRadius = 10f;
    [SerializeField] private UIDocument gameOverDocument;
    [SerializeField] private string gameOverRootName = "game-over-root";
    [SerializeField] private string restartButtonName = "restart-button";
    [SerializeField] private UIDocument startGameDocument;
    [SerializeField] private string startGameRootName = "instructions-root";
    [SerializeField] private string startButtonName = "start-button";
    private Transform playerTransform;
    private Player player;
    private VisualElement gameOverRoot;
    private Button restartButton;
    private VisualElement startGameRoot;
    private Button startButton;


    // Private Methods
    private void SpawnSpider()
    {
        // Return if conditions for spider spawning are not met
        if (spiderPrefabs == null || spiderPrefabs.Count == 0 || playerTransform == null)
        {
            Debug.LogWarning("SpawnSpider skipped, missing prefabs or player transform");
            return;
        }


        // Spawn random amount of spiders
        int count = UnityEngine.Random.Range(1, maxSpawnCount + 1);
        for (int i = 0; i < count; i++)
        {
            Vector2 spawnOffset = GetRandomSpawnOffset();
            Vector3 spawnPos = playerTransform.position + (Vector3)spawnOffset;
            int prefabIndex = UnityEngine.Random.Range(0, spiderPrefabs.Count);
            GameObject prefabToSpawn = spiderPrefabs[prefabIndex];
            Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
        }


        // Queue up next spawn with random interval
        ScheduleNextSpawn();
    }
    private void ScheduleNextSpawn()
    {
        float nextInterval = UnityEngine.Random.Range(spawnInterval.min, spawnInterval.max);
        Invoke(nameof(SpawnSpider), nextInterval);
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
    private void ShowStartGameScreen()
    {
        // Freeze game until player starts
        Time.timeScale = 0f;


        // Return if start game root is missing
        if (startGameRoot == null)
        {
            Debug.LogWarning("ShowStartGameScreen skipped, start game root not found");
            return;
        }

        startGameRoot.style.display = DisplayStyle.Flex;
    }
    private void BeginGame()
    {
        // Hide start game screen if present
        if (startGameRoot != null)
        {
            startGameRoot.style.display = DisplayStyle.None;
        }


        // Unfreeze game
        Time.timeScale = 1f;


        // Enable player now that game has begun
        player.enabled = true;


        // Spawn spiders with random interval between spawns
        ScheduleNextSpawn();


        // Spawn first wave instantly
        SpawnSpider();
    }


    // Override Methods
    private void Start()
    {
        // Get player via tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        playerTransform = playerObj.transform;
        player = playerObj.GetComponent<Player>();
        player.OnDeath += EndGame;


        // Disable player until game actually begins
        player.enabled = false;


        // Setup game over UI references
        SetupGameOverUI();


        // Setup start game UI references
        SetupStartGameUI();


        // Show start screen and wait for player input
        ShowStartGameScreen();
    }
    private void SetupGameOverUI()
    {
        // Return if game over document is missing
        if (gameOverDocument == null)
        {
            Debug.LogWarning("SetupGameOverUI skipped, game over document not assigned");
            return;
        }

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
    private void SetupStartGameUI()
    {
        // Return if start game document is missing
        if (startGameDocument == null)
        {
            Debug.LogWarning("SetupStartGameUI skipped, start game document not assigned");
            return;
        }

        VisualElement root = startGameDocument.rootVisualElement;
        startGameRoot = root.Q<VisualElement>(startGameRootName);
        startButton = root.Q<Button>(startButtonName);


        // Return if expected elements are missing
        if (startGameRoot == null || startButton == null)
        {
            Debug.LogWarning("SetupStartGameUI skipped, expected elements not found in UI document");
            return;
        }

        startButton.clicked += BeginGame;
    }
}
