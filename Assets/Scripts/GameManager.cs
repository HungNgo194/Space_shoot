using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UIElements;
using Unity.VisualScripting;

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
    [SerializeField] private List<GameObject> listEnemy;


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

    [SerializeField] public GameObject winningPannel;

    [Header("Survival Mode")]
    [SerializeField] public bool isSurvivalMode;
    [SerializeField] public float survivalTime = 0f;
    [SerializeField] private float timeToIncreaseDifficulty = 20f;
    [SerializeField] private float spawnRateDecrease = 0.1f;
    [SerializeField] private TMP_Text survivalTimeText;

    [Header("Game Over UI")]
    [SerializeField] private TMP_Text gameOverScoreText; // Text to display the score
    [SerializeField] private TMP_Text gameOverTimeText; // Text to display the time
    [SerializeField] private GameObject saveButton; // Save button

    [SerializeField] public GameObject saveUIPanel;


    
    public int playerScore = 0;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Time.timeScale = 1f;
        pausedMenu.SetActive(false);
        gameOver.SetActive(false);
        winningPannel.SetActive(false);
        nextLevelText.gameObject.SetActive(false);
        saveUIPanel.SetActive(false);
        
        UpdateScoreUI();
        InstantiateHeart();

        if (isSurvivalMode)
        {
            survivalTimeText.gameObject.SetActive(true);
            StartSurvivalMode();
            survivalTime = 0f;
            if (survivalTimeText != null)
                survivalTimeText.text = "Time: 0";
        }
        else
        {
            survivalTimeText.gameObject.SetActive(false);
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
        }
        InvokeRepeating("InstantiateRing", Random.Range(4f, 10f), Random.Range(minSpawnRate, maxSpawnRate));
    }
    private int lastProcessedTime = -1;
    private int timeOfIncrease = 0;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PausedMenu(true);
        }
        survivalTime += Time.deltaTime;
        if (survivalTimeText != null)
            survivalTimeText.text = "Time: " + Mathf.FloorToInt(survivalTime).ToString();

        if (isSurvivalMode)
        {
            CancelInvoke("SpawnEnemy");
           

            int currentTime = Mathf.FloorToInt(survivalTime);
            if (currentTime % timeToIncreaseDifficulty == 0 && currentTime != lastProcessedTime && currentTime > 0)
            {
                Debug.Log("true");
                timeOfIncrease++;
                lastProcessedTime = currentTime;
                IncreaseDifficulty();
            }
        }
    }

    private void IncreaseDifficulty()
    {
        if (asteroidSpawnRate > 0.1)
        {
            asteroidSpawnRate -= spawnRateDecrease;
            CancelInvoke("SpawnRandomAsteroid");
            InvokeRepeating("SpawnRandomAsteroid", 1f, asteroidSpawnRate);
        }
        if (spawnRate > 0.1)
        {
            spawnRate -= spawnRateDecrease;
            CancelInvoke("SpawnEnemy");
            InvokeRepeating("SpawnEnemy", 2f, spawnRate);
        }

    }

    void SpawnEnemy()
    {
        Vector3 enemyPos = new Vector3(Random.Range(minInstatiateValue, maxInstatiateValue), 6f);
        GameObject gm = Instantiate(enemyPrefab1, enemyPos, Quaternion.Euler(0f, 0f, 180f));
        EnemyController controller = gm.GetComponent<EnemyController>();
        if (timeOfIncrease > 0) 
        { 
            controller.increaseHP(timeOfIncrease); 
        }
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
        AsteroidsController controller = asteroid.GetComponent<AsteroidsController>();
        if (timeOfIncrease > 0) 
        { 
            controller.increaseHP(timeOfIncrease); 
        }
        Destroy(asteroid, destroyTime);
    }

    void SpawnRandomEnemy()
    {
        if (listEnemy.Count == 0) return;
        GameObject enemyPrefab = listEnemy[Random.Range(0, listEnemy.Count)];
        Vector3 spawnPos = new Vector3(Random.Range(minInstatiateValue, maxInstatiateValue), 6f);
        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.Euler(0f, 0f, 0f));
        Destroy(enemy, destroyTime);
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
        ResetSurvivalMode();
        SceneManager.LoadSceneAsync(1);
    }

    public void RestartGameFromBegin()
    {
        Time.timeScale = 1f;
        gameOver.SetActive(false);
        ResetSurvivalMode();
        SceneManager.LoadSceneAsync(1);
    }

    public bool canAddHeart = false;
    public void ReducePlayerHealth()
    {
        if (playerHealth > 0)
        {
            playerHealth--;
            canAddHeart = true;
            InstantiateHeart();
            if (playerHealth <= 0)
            {
                Invoke("ShowGameOver", 1.5f);
            }
        }
    }

    private void ShowGameOver()
    {
        Time.timeScale = 0f;


        if (isSurvivalMode)
        {
            saveUIPanel.SetActive(true);

            if (gameOverScoreText != null)
                gameOverScoreText.text = playerScore.ToString();

            if (gameOverTimeText != null)
                gameOverTimeText.text = Mathf.FloorToInt(survivalTime).ToString() + "s";

        }
        else
        {
            gameOver.SetActive(true);
        }
    }

    public void IncreasePlayerHealth()
    {
        if (playerHealth < maxHealth)
        {
            playerHealth++;
            InstantiateHeart(); // Update hearts
        } else if (playerHealth ==  maxHealth)
        {
            canAddHeart = false ;
        }
    }

    public void AddScore(int points)
    {
        if (isSurvivalMode)
        {
            playerScore += points;
            UpdateScoreUI();
        }
        else if (SceneManager.GetActiveScene().buildIndex == 3)
        {
            return;
        }
        else
        {
            playerScore += points;
            UpdateScoreUI();

            if (playerScore >= scoreRequired)
            {
                LevelCompleted();
            }
        }
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            if (isSurvivalMode)
            {
                scoreText.text = "Score: " + playerScore.ToString();
            }
            else
                scoreText.text = playerScore.ToString() + "/" + scoreRequired;
        }
    }

    public void BossDefeated()
    {
        if (!isSurvivalMode)
        {
            CancelInvoke("SpawnEnemy");
            CancelInvoke("SpawnRandomAsteroid");
            CancelInvoke("InstantiateRing");
            StartCoroutine(ShowWiningText());
        }
    }

    private void LevelCompleted()
    {
        if (!isSurvivalMode)
        {
            CancelInvoke("SpawnEnemy");
            CancelInvoke("SpawnRandomAsteroid");
            CancelInvoke("InstantiateRing");
            StartCoroutine(ShowBlinkingText());
        }
    }
    
    private IEnumerator ShowWiningText()
    {
        // Ensure the winning panel is active
        yield return new WaitForSeconds(0.5f);
        winningPannel.SetActive(true);
        TMP_Text winText = winningPannel.GetComponentInChildren<TMP_Text>();
        if (winText != null)
        {
            winText.text = $"Congratulations! You have completed this campaign in!";
        }
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

    public void StartSurvivalMode()
    {
        isSurvivalMode = true;
        survivalTime = 0f;
        playerScore = 0;
        playerHealth = maxHealth;
        InstantiateHeart();
        UpdateScoreUI();

        // Reset spawn rates
        asteroidSpawnRate = 2f; // Initial spawn rate for asteroids
        spawnRate = 2f; // Initial spawn rate for enemies

        // Start spawning
        InvokeRepeating("SpawnRandomAsteroid", 1f, asteroidSpawnRate);
        InvokeRepeating("SpawnRandomEnemy", 10f, spawnRate);
        InvokeRepeating("InstantiateRing", Random.Range(4f, 10f), Random.Range(minSpawnRate, maxSpawnRate));
    }

    public void ResetSurvivalMode()
    {
        isSurvivalMode = false;
        survivalTime = 0f;
        if (survivalTimeText != null)
            survivalTimeText.text = "Time: 0";
    }

   public void ClickSave()
    {
        saveUIPanel.SetActive(true);
    }
}