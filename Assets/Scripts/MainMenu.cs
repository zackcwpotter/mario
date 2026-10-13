using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public IntVariable gameScore;
    public TMP_Text highScoreText;

    void Start()
    {
        UpdateHighScoreText();
    }

    void UpdateHighScoreText()
    {
        highScoreText.text = "High Score: " + gameScore.previousHighestValue.ToString("D4");
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Loading Scene");
    }

    public void ResetHighscore()
    {
        gameScore.ResetHighestValue();
        UpdateHighScoreText();

        GameObject eventSystem = GameObject.Find("EventSystem");
        eventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
    }
}