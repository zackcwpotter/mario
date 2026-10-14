
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseButtonController : MonoBehaviour, IInteractiveButton
{
    private bool isPaused = false;
    public Sprite pauseIcon;
    public Sprite playIcon;
    private Image image;

    [Header("UI Elements")]
    public GameObject pausedText; // Drag your 'PausedText' object here in the Inspector

    void Start()
    {
        image = GetComponent<Image>();
        AudioListener.pause = false;

        if (pausedText != null)
            pausedText.SetActive(false);

        GameManager.instance.gameRestart.AddListener(ResetPause);
        GameManager.instance.playerDeath.AddListener(OnPlayerDeath);
    }

    public void ResetPause()
    {
        isPaused = false;
        AudioListener.pause = false;
        image.sprite = pauseIcon;

        if (pausedText != null)
            pausedText.SetActive(false);
    }

    public void ButtonClick()
    {
        Time.timeScale = isPaused ? 1.0f : 0.0f;
        isPaused = !isPaused;
        AudioListener.pause = isPaused;

        if (isPaused)
        {
            image.sprite = playIcon;
        }
        else
        {
            image.sprite = pauseIcon;
        }

        // Show when paused, hide when unpaused
        if (pausedText != null)
        {
            pausedText.SetActive(isPaused);
        }
    }
        void OnPlayerDeath()
    {
        // Unpause game & audio if paused so death sounds and animations run normally
        if (isPaused)
            ResetPause();
        gameObject.SetActive(false);
    }
    void OnDestroy()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.gameRestart.RemoveListener(ResetPause);
            GameManager.instance.playerDeath.RemoveListener(OnPlayerDeath);
        }
    }
}
