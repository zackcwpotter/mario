using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public GameObject scoreText;
    public RectTransform restartButton;
    public GameObject gameOverPanel;

    private RectTransform scoreRect;

    public IntVariable gameScore;
    public GameObject highscoreText;

    public GameObject pauseButton;

    void Awake()
    {
        scoreRect = scoreText.GetComponent<RectTransform>();
    }

    void Start()
    {
        GameManager.instance.gameStart.AddListener(GameStart);
        GameManager.instance.gameRestart.AddListener(GameStart);
        GameManager.instance.scoreChange.AddListener(SetScore);
        GameManager.instance.gameOver.AddListener(GameOver);
        
        GameStart();
        Debug.Log("HUD Start score: " + GameManager.instance.gameScore.Value);
        SetScore(GameManager.instance.gameScore.Value);
    }

    public void GameStart()
    {
        gameOverPanel.SetActive(false);
        highscoreText.SetActive(false);
        if (pauseButton != null)
            pauseButton.SetActive(true);

        // Score: top left
        scoreRect.anchorMin = new Vector2(0, 1);
        scoreRect.anchorMax = new Vector2(0, 1);
        scoreRect.pivot = new Vector2(0, 1);
        scoreRect.anchoredPosition = new Vector2(40, -30);

        // Replay: top right
        restartButton.anchorMin = new Vector2(1, 1);
        restartButton.anchorMax = new Vector2(1, 1);
        restartButton.pivot = new Vector2(1, 1);
        restartButton.anchoredPosition = new Vector2(-40, -30);
    }

    public void SetScore(int score)
    {
        scoreText.GetComponent<TextMeshProUGUI>().text =
            "Score: " + score.ToString();
    }

    public void GameOver()
    {
        gameOverPanel.SetActive(true);
        if (pauseButton != null)
            pauseButton.SetActive(false);

        // Score: centered below Game Over text
        scoreRect.anchorMin = new Vector2(0.5f, 0.5f);
        scoreRect.anchorMax = new Vector2(0.5f, 0.5f);
        scoreRect.pivot = new Vector2(0.5f, 0.5f);
        scoreRect.anchoredPosition = new Vector2(0, -60);

        // Replay: centered below score
        restartButton.anchorMin = new Vector2(0.5f, 0.5f);
        restartButton.anchorMax = new Vector2(0.5f, 0.5f);
        restartButton.pivot = new Vector2(0.5f, 0.5f);
        restartButton.anchoredPosition = new Vector2(0, -190);

        highscoreText.GetComponent<TextMeshProUGUI>().text =
            "High Score: " + gameScore.previousHighestValue.ToString("D3");

        highscoreText.SetActive(true);
    }

    void OnDestroy()
    {
        if (GameManager.instance == null)
            return;

        GameManager.instance.gameStart.RemoveListener(GameStart);
        GameManager.instance.gameRestart.RemoveListener(GameStart);
        GameManager.instance.scoreChange.RemoveListener(SetScore);
        GameManager.instance.gameOver.RemoveListener(GameOver);
    }
}