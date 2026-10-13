using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public GameObject scoreText;
    public RectTransform restartButton;
    public GameObject gameOverPanel;

    private RectTransform scoreRect;

    public IntVariable gameScore;
    public GameObject highscoreText;

    public GameObject pauseButton;
    public GameObject homeButton;

    // top right corner buttons, right to left: replay, pause, home
    private const float buttonSize = 120f;
    private const float buttonSpacing = 10f;
    private static readonly Vector2 cornerMargin = new Vector2(40, 30);

    // "MAIN MENU" text shown under the replay button at game over
    private GameObject mainMenuText;

    void Awake()
    {
        scoreRect = scoreText.GetComponent<RectTransform>();

        // fall back to finding the buttons by name if they aren't assigned in the Inspector
        if (pauseButton == null)
            pauseButton = FindChild("Pause Button");
        if (homeButton == null)
            homeButton = FindChild("Home Button");

        CreateMainMenuText();
    }

    // built in code so both worlds get the same game over menu text
    private void CreateMainMenuText()
    {
        TextMeshProUGUI scoreTMP = scoreText.GetComponent<TextMeshProUGUI>();

        mainMenuText = new GameObject("Main Menu Text", typeof(RectTransform));
        mainMenuText.transform.SetParent(transform, false);

        TextMeshProUGUI label = mainMenuText.AddComponent<TextMeshProUGUI>();
        label.text = "MAIN MENU";
        label.font = scoreTMP.font;
        label.fontSize = scoreTMP.fontSize;
        label.color = Color.white;
        label.alignment = TextAlignmentOptions.Center;
        label.textWrappingMode = TextWrappingModes.NoWrap;

        Button button = mainMenuText.AddComponent<Button>();
        button.targetGraphic = label;
        button.onClick.AddListener(() => GameManager.instance.ReturnToMenu());

        RectTransform rect = mainMenuText.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(600, 80);
        rect.anchoredPosition = new Vector2(0, -310);

        mainMenuText.SetActive(false);
    }

    private GameObject FindChild(string childName)
    {
        foreach (Transform t in GetComponentsInChildren<Transform>(true))
        {
            if (t.name == childName)
                return t.gameObject;
        }
        return null;
    }

    private void PlaceInCorner(RectTransform rect, int slot)
    {
        if (rect == null)
            return;

        rect.localScale = Vector3.one;
        rect.anchorMin = new Vector2(1, 1);
        rect.anchorMax = new Vector2(1, 1);
        rect.pivot = new Vector2(1, 1);
        rect.sizeDelta = new Vector2(buttonSize, buttonSize);
        rect.anchoredPosition = new Vector2(
            -cornerMargin.x - slot * (buttonSize + buttonSpacing),
            -cornerMargin.y);
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

        // Replay, pause and home in a row in the top right corner
        PlaceInCorner(restartButton, 0);
        if (pauseButton != null)
            PlaceInCorner(pauseButton.GetComponent<RectTransform>(), 1);
        mainMenuText.SetActive(false);

        if (homeButton != null)
        {
            homeButton.SetActive(true);
            PlaceInCorner(homeButton.GetComponent<RectTransform>(), 2);
        }
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

        // Home button is replaced by the "MAIN MENU" text under replay
        if (homeButton != null)
            homeButton.SetActive(false);
        mainMenuText.transform.SetAsLastSibling(); // draw on top of the game over panel
        mainMenuText.SetActive(true);

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