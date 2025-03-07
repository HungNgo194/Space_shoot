using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SaveGameUI : MonoBehaviour
{
    public TMP_InputField nameInputField;
    public Button saveButton;

    private void Start()
    {
        saveButton.onClick.AddListener(OnSaveButtonClicked);
    }

    private void OnSaveButtonClicked()
    {
        string playerName = nameInputField.text;
        if (string.IsNullOrEmpty(playerName))
        {
            Debug.LogWarning("Please enter a valid name!");
            return;
        }

        // Get current game data
        GameData data = new GameData
        {
            playerName = playerName,
            time = GameManager.instance.isSurvivalMode ? GameManager.instance.survivalTime : Time.time,
            score = GameManager.instance.playerScore,
            isSurvivalMode = GameManager.instance.isSurvivalMode,
            isWinning = !GameManager.instance.isSurvivalMode && GameManager.instance.winningPannel.activeSelf
        };

        // Load existing data, add new data, and save
        List<GameData> savedData = SaveSystem.LoadGameData();
        savedData.Add(data);
        SaveSystem.SaveGameData(savedData);
        SceneManager.LoadScene(0);
    }
}