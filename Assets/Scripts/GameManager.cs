using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private int scoreRequired;

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
    [SerializeField] private GameObject heartPrefab; // Heart prefab
    private List<GameObject> heartIcons = new List<GameObject>(); // Store hearts

    [Header("Ring")]
    [SerializeField] private GameObject ringPrefab;
    [SerializeField] private float minSpawnRate;
    [SerializeField] private float maxSpawnRate;

    [Header("Panels")]
    [SerializeField] private GameObject pausedMenu;
    [SerializeField] private GameObject gameOver;

    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private TMP_Text nextLevelText;
    [SerializeField] public TMP_Text scoreText;

    [SerializeField] private GameObject winningPannel;

    private int playerScore = 0;    
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        pausedMenu.SetActive(false);
        gameOver.SetActive(false);
        winningPannel.SetActive(false);
        nextLevelText.gameObject.SetActive(false);
        Time.timeScale = 1f;
        InstantiateHeart();
        UpdateScoreUI();

        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            InvokeRepeating("SpawnRandomAsteroid", 1f, asteroidSpawnRate);
        }
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            InvokeRepeating("SpawnRandomAsteroid", 1f, asteroidSpawnRate);
            InvokeRepeating("SpawnEnemy", 2f, spawnRate);
        }
        if (SceneManager.GetActiveScene().buildIndex == 3)
        {
            scoreText.text = "";
            InvokeRepeating("SpawnRandomAsteroid", 1f, asteroidSpawnRate);
            InvokeRepeating("SpawnEnemy", 2f, spawnRate);
            SpawnBoss();
        }

        InvokeRepeating("InstantiateRing", Random.Range(4f, 10f), Random.Range(minSpawnRate, maxSpawnRate));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PausedMenu(true);
        }
    }

    void SpawnEnemy()
    {
        Vector3 enemyPos = new Vector3(Random.Range(minInstatiateValue, maxInstatiateValue), 6f);
        GameObject gm = Instantiate(enemyPrefab1, enemyPos, Quaternion.Euler(0f, 0f, 180f));
        Destroy(gm, destroyTime);
    }

    void SpawnBoss()
    {
        Vector3 enemyPos = new Vector3(-0.02f, 2.41f, 6f);
        GameObject gm = Instantiate(bossPrefab, enemyPos, Quaternion.Euler(0f, 0f, 0));
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

        // Define base position (make sure heartSpawnPoint is assigned in Inspector)
        Vector3 basePosition = heartSpawnPosition != null ? heartSpawnPosition.position : new Vector3(-4f, 4f, 0f);

        // Instantiate new hearts
        for (int i = 0; i < playerHealth; i++)
        {
            Vector3 heartPos = basePosition - new Vector3(i * 1.2f, 0, 0); // Adjust spacing
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
        } else
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

    public void RestartGameFromBegin()
    {
        Time.timeScale = 1f;
        gameOver.SetActive(false);
        SceneManager.LoadSceneAsync(1);

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
    }
    public void IncreasePlayerHealth()
    {
        if (playerHealth < maxHealth)
        {
            playerHealth++;
            InstantiateHeart(); // Update hearts
        }
    }

    public void AddScore(int points)
    {
        if (SceneManager.GetActiveScene().buildIndex == 3)
            return;

        playerScore += points;
        UpdateScoreUI();

        if(playerScore >= scoreRequired) { LevelCompleted(); }
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = playerScore.ToString() + "/" + scoreRequired;
    }

    public void BossDefeated()
    {
        CancelInvoke("SpawnEnemy");
        CancelInvoke("SpawnRandomAsteroid");
        CancelInvoke("InstantiateRing");

        StartCoroutine(ShowWiningText()); 
    }



    void LevelCompleted()
    {
        CancelInvoke("SpawnEnemy");
        CancelInvoke("SpawnRandomAsteroid");
        CancelInvoke("InstantiateRing");

        StartCoroutine(ShowBlinkingText());
    }

    private IEnumerator ShowWiningText()
    {
        winningPannel.SetActive(true); // Ensure the panel is active

        // Blink effect (6 times)
        for (int i = 0; i < 6; i++)
        {
            winningPannel.SetActive(!winningPannel.activeSelf);
            yield return new WaitForSeconds(0.5f);
        }

        winningPannel.SetActive(true); // Ensure it's visible at the end
        yield return new WaitUntil(() => Input.anyKeyDown);

        SceneManager.LoadScene(0);
    }
    private IEnumerator ShowBlinkingText()
    {
        nextLevelText.gameObject.SetActive(true);

        // Blink 3 times
        for (int i = 0; i < 6; i++)
        {
            nextLevelText.enabled = !nextLevelText.enabled;
            yield return new WaitForSeconds(0.5f);
        }

        nextLevelText.enabled = true; // Ensure it's visible
        yield return new WaitUntil(() => Input.anyKeyDown); // Wait for any key press

        LoadNextLevel();
    }

    private void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
