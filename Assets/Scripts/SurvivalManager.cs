using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SurvivalManager : MonoBehaviour
{
    public static SurvivalManager instance;

    [Header("Survival Mode Settings")]
    [SerializeField] private float survivalTime = 0f; // Track survival time
    [SerializeField] private TMP_Text survivalTimeText; // UI for survival time
    [SerializeField] private float difficultyIncreaseInterval = 30f; // Increase difficulty every 30 seconds
    [SerializeField] private float maxSpawnRateGame = 0.5f; // Maximum spawn rate for enemies/asteroids

    [Header("Enemy")]
    [SerializeField] private GameObject enemyPrefab1;
    [SerializeField] private float minInstatiateValue;
    [SerializeField] private float maxInstatiateValue;
    [SerializeField] private float destroyTime;
    [SerializeField] private float spawnRate;

    [Header("Asteroids")]
    [SerializeField] private List<GameObject> asteroidPrefabs;
    [SerializeField] private float asteroidSpawnRate;

    [Header("Health")]
    [SerializeField] public int playerHealth;
    [SerializeField] public int maxHealth;

    [Header("Particle Effects")]
    [SerializeField] public GameObject explosion;
    [SerializeField] public GameObject muzzleFlash;

    [Header("Heart UI")]
    [SerializeField] private Transform heartSpawnPosition;
    [SerializeField] private GameObject heartPrefab;
    private List<GameObject> heartIcons = new List<GameObject>();

    [Header("Ring")]
    [SerializeField] private GameObject ringPrefab;
    [SerializeField] private float minSpawnRate;
    [SerializeField] private float maxSpawnRate;

    [Header("Panels")]
    [SerializeField] private GameObject pausedMenu;
    [SerializeField] private GameObject gameOver;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // Initialize UI and game state
        pausedMenu.SetActive(false);
        gameOver.SetActive(false);
        Time.timeScale = 1f;
        InstantiateHeart();

        // Start spawning enemies, asteroids, and rings
        InvokeRepeating("SpawnRandomAsteroid", 1f, asteroidSpawnRate);
        InvokeRepeating("SpawnEnemy", 2f, spawnRate);
        InvokeRepeating("InstantiateRing", Random.Range(4f, 10f), Random.Range(minSpawnRate, maxSpawnRate));

        // Start increasing difficulty over time
        InvokeRepeating("IncreaseDifficulty", difficultyIncreaseInterval, difficultyIncreaseInterval);
    }

    private void Update()
    {
        // Pause game when Escape is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PausedMenu(true);
        }

        // Update survival time
        survivalTime += Time.deltaTime;
        if (survivalTimeText != null)
        {
            survivalTimeText.text = "Survival Time: " + Mathf.RoundToInt(survivalTime).ToString() + "s";
        }
    }

    private void IncreaseDifficulty()
    {
        // Increase spawn rates over time
        if (spawnRate > maxSpawnRateGame)
        {
            spawnRate -= 0.1f; // Increase enemy spawn rate
            asteroidSpawnRate -= 0.1f; // Increase asteroid spawn rate
        }
    }

    void SpawnEnemy()
    {
        Vector3 enemyPos = new Vector3(Random.Range(minInstatiateValue, maxInstatiateValue), 6f);
        GameObject gm = Instantiate(enemyPrefab1, enemyPos, Quaternion.Euler(0f, 0f, 180f));
        Destroy(gm, destroyTime);
    }

    void SpawnRandomAsteroid()
    {
        if (asteroidPrefabs.Count == 0) return;
        GameObject asteroidPrefab = asteroidPrefabs[Random.Range(0, asteroidPrefabs.Count)];
        Vector3 spawnPos = new Vector3(Random.Range(minInstatiateValue, maxInstatiateValue), 6f);
        GameObject asteroid = Instantiate(asteroidPrefab, spawnPos, Quaternion.Euler(0f, 0f, 0f));
        Destroy(asteroid, destroyTime);
    }

    public void InstantiateHeart()
    {
        // Remove existing hearts
        foreach (GameObject heart in heartIcons)
        {
            Destroy(heart);
        }
        heartIcons.Clear();

        // Define base position
        Vector3 basePosition = heartSpawnPosition != null ? heartSpawnPosition.position : new Vector3(-4f, 4f, 0f);

        // Instantiate new hearts
        for (int i = 0; i < playerHealth; i++)
        {
            Vector3 heartPos = basePosition - new Vector3(i * 1.1f, 0, 0); // Adjust spacing
            GameObject newHeart = Instantiate(heartPrefab, heartPos, Quaternion.identity);
            heartIcons.Add(newHeart);
        }
    }

    public void InstantiateRing()
    {
        Vector3 ringPos = new Vector3(Random.Range(minInstatiateValue, maxInstatiateValue), 6f);
        GameObject gm = Instantiate(ringPrefab, ringPos, Quaternion.Euler(0f, 0f, 0f));
        Destroy(gm, destroyTime);
    }

    public void PausedMenu(bool isPause)
    {
        if (isPause)
        {
            pausedMenu.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            pausedMenu.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    public void ResumeGame()
    {
        pausedMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void QuitGame()
    {
        SceneManager.LoadSceneAsync(0);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        pausedMenu.SetActive(false);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReducePlayerHealth()
    {
        if (playerHealth > 0)
        {
            playerHealth--;
            InstantiateHeart(); // Update hearts

            if (playerHealth <= 0)
            {
                Invoke("ShowGameOver", 1.5f);
            }
        }
    }

    private void ShowGameOver()
    {
        Time.timeScale = 0f;
        gameOver.SetActive(true);

        // Display final survival time
        if (survivalTimeText != null)
        {
            survivalTimeText.text = "You Survived: " + Mathf.RoundToInt(survivalTime).ToString() + "s";
        }
    }

    public void IncreasePlayerHealth()
    {
        if (playerHealth < maxHealth)
        {
            playerHealth++;
            InstantiateHeart(); // Update hearts
        }
    }
}
