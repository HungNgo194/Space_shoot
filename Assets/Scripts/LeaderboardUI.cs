using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class LeaderboardUI : MonoBehaviour
{
    public Transform leaderboardContent; // Parent for leaderboard entries
    public GameObject entryPrefab; // Prefab for each entry
    public Toggle survivalTab; // Toggle for Survival Mode

    private List<GameData> savedData; // Store loaded data

    private void Start()
    {
        // Load data
        savedData = SaveSystem.LoadGameData();

        // Set Survival Mode as the default tab
        survivalTab.isOn = true;
        ShowSurvivalLeaderboard();

        // Add listeners to the toggles
       

        survivalTab.onValueChanged.AddListener((isOn) => {
            if (isOn) ShowSurvivalLeaderboard();
        });
    }

    // Show entries for Survival Mode
    private void ShowSurvivalLeaderboard()
    {
        ClearLeaderboard();
        var survivalData = savedData.FindAll(data => data.isSurvivalMode);
        PopulateLeaderboard(survivalData);
    }

    // Clear existing entries
    private void ClearLeaderboard()
    {
        foreach (Transform child in leaderboardContent)
        {
            Destroy(child.gameObject);
        }
    }

    // Populate the leaderboard with data
    private void PopulateLeaderboard(List<GameData> data)
    {
        data.Sort((a, b) => b.score.CompareTo(a.score));

        float entrySpacing = 50f;

        for (int i = 0; i < data.Count; i++)
        {
            GameData entry = data[i];
            GameObject entryObj = Instantiate(entryPrefab, leaderboardContent);
            TMP_Text textComponent = entryObj.GetComponent<TMP_Text>();
            textComponent.text = $"{entry.playerName} - Score: {entry.score} - Time: {entry.time:F2}s";
            RectTransform entryTransform = entryObj.GetComponent<RectTransform>();
            entryTransform.anchoredPosition = new Vector2(0, -i * entrySpacing);
        }
    }

    // Close the leaderboard panel
    public void CloseLeaderboard()
    {
        gameObject.SetActive(false);
    }
}