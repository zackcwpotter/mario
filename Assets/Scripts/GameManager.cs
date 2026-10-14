using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Audio; // Required for AudioMixer snapshots
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public UnityEvent gameStart;
    public UnityEvent gameRestart;
    public UnityEvent<int> scoreChange;
    public UnityEvent gameOver;

    [Header("Audio Snapshots")]
    public AudioMixerSnapshot normalSnapshot;
    public AudioMixerSnapshot sadGameOverSnapshot;

    public IntVariable gameScore;

    public UnityEvent playerDeath;

    void Start()
    {
        gameScore.Value = 0; // Reset score when starting a new game
        gameStart.Invoke();
        Time.timeScale = 1.0f;

        SceneManager.activeSceneChanged += SceneSetup;
        
    }

    public void GameRestart()
    {
        gameScore.Value = 0;
        
        SetScore(gameScore.Value);
        gameRestart.Invoke();
        Time.timeScale = 1.0f;

        // Transition back to normal audio instantly when they hit replay
        if (normalSnapshot != null)
        {
            normalSnapshot.TransitionTo(0f);
        }
    }

    public void IncreaseScore(int increment)
    {
        gameScore.ApplyChange(increment);
        gameScore.SaveHighScore();
        SetScore(gameScore.Value);
    }

    public void SetScore(int score)
    {
        scoreChange.Invoke(score);
    }

    public void GameOver()
    {
        Time.timeScale = 0.0f;
        gameOver.Invoke();

        // Transition to the pitched-down snapshot over 1.5 seconds
        if (sadGameOverSnapshot != null)
        {
            sadGameOverSnapshot.TransitionTo(0f);
        }
    }

    public void ReturnToMenu()
    {
        // leave the game in a clean state, so the next Start Game begins fresh
        gameScore.Value = 0;
        Time.timeScale = 1.0f;
        AudioListener.pause = false;

        if (normalSnapshot != null)
        {
            normalSnapshot.TransitionTo(0f);
        }

        SceneManager.LoadScene("Main Menu");
    }

    public void SceneSetup(Scene current, Scene next)
    {
        gameStart.Invoke();

        Debug.Log("Score after scene change: " + gameScore.Value);
        SetScore(gameScore.Value);
    }
}