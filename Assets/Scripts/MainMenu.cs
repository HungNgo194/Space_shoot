using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] public GameObject leaderBoard;
    [SerializeField] public GameObject modePanel;
    private void Start()
    {
        leaderBoard.gameObject.SetActive(false);
        modePanel.gameObject.SetActive(false);
    }
    public void PlayGame() {
        modePanel.gameObject.SetActive(true);
    }

    public void ExitGame() 
    { 
        Application.Quit();
    }

    public void LeaderBoard()
    {
        leaderBoard.gameObject.SetActive(true);
    }

    public void SurvivalMode()
    {
        SceneManager.LoadSceneAsync(5);
    }

    public void CampaignMode()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void CloseModelPanel()
    {
        modePanel.gameObject.SetActive(false);
    }
}
