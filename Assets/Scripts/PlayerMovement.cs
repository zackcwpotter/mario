using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 25;
    public float maxSpeed = 30;
    public float upSpeed = 10;
    public float jumpBufferTime = 0.2f;

    public TextMeshProUGUI scoreText;
    public GameObject enemies;
    public JumpOverGoomba jumpOverGoomba;
    public GameObject gameOverPanel;

    // 1. New variables to control the UI positions
    public RectTransform scoreTextRect;
    public RectTransform restartButtonRect;

    private Rigidbody2D marioBody;
    private float jumpBufferCounter;
    private bool onGroundState = false;
    private SpriteRenderer marioSprite;
    private bool faceRightState = true;
    private Vector3 startPosition;

    // Variables to remember where the UI originally was
    private Vector2 scoreOrigPos, scoreOrigAnchorMin, scoreOrigAnchorMax;
    private Vector2 buttonOrigPos, buttonOrigAnchorMin, buttonOrigAnchorMax;

    //To reset camera
    public Transform gameCamera;

    void Start()
    {
        Application.targetFrameRate = 30;
        marioBody = GetComponent<Rigidbody2D>();
        marioSprite = GetComponent<SpriteRenderer>();
        startPosition = transform.position;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // 2. Save original layout at the start of the game
        if (scoreTextRect != null)
        {
            scoreOrigPos = scoreTextRect.anchoredPosition;
            scoreOrigAnchorMin = scoreTextRect.anchorMin;
            scoreOrigAnchorMax = scoreTextRect.anchorMax;
        }
        if (restartButtonRect != null)
        {
            buttonOrigPos = restartButtonRect.anchoredPosition;
            buttonOrigAnchorMin = restartButtonRect.anchorMin;
            buttonOrigAnchorMax = restartButtonRect.anchorMax;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown("a") && faceRightState)
        {
            faceRightState = false;
            marioSprite.flipX = true;
        }

        if (Input.GetKeyDown("d") && !faceRightState)
        {
            faceRightState = true;
            marioSprite.flipX = false;
        }

        if (Input.GetKeyDown("space"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");

        if (Mathf.Abs(moveHorizontal) > 0)
        {
            Vector2 movement = new Vector2(moveHorizontal, 0);

            if (Mathf.Abs(marioBody.linearVelocity.x) < maxSpeed)
            {
                marioBody.AddForce(movement * speed);
            }
        }

        if (jumpBufferCounter > 0f && onGroundState)
        {
            marioBody.AddForce(Vector2.up * upSpeed, ForceMode2D.Impulse);
            onGroundState = false;
            jumpBufferCounter = 0f;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground")) onGroundState = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Time.timeScale = 0.0f;

            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }

            // 3. Move the UI to the center when Mario dies
            if (scoreTextRect != null)
            {
                scoreTextRect.anchorMin = new Vector2(0.5f, 0.5f);
                scoreTextRect.anchorMax = new Vector2(0.5f, 0.5f);
                // Moves it 20 pixels above the exact center
                scoreTextRect.anchoredPosition = new Vector2(0, 20);
            }
            if (restartButtonRect != null)
            {
                restartButtonRect.anchorMin = new Vector2(0.5f, 0.5f);
                restartButtonRect.anchorMax = new Vector2(0.5f, 0.5f);
                // Moves it 60 pixels below the exact center
                restartButtonRect.anchoredPosition = new Vector2(0, -100);
            }
        }
    }

    public void RestartButtonCallback(int input)
    {
        ResetGame();
        Time.timeScale = 1.0f;
    }

    public void ResetGame()
    {
        marioBody.transform.position = startPosition;
        faceRightState = true;
        marioSprite.flipX = false;
        scoreText.text = "Score: 0";

        foreach (Transform eachChild in enemies.transform)
        {
            EnemyMovement enemyScript = eachChild.GetComponent<EnemyMovement>();
            if (enemyScript != null)
            {
                eachChild.localPosition = enemyScript.startPosition;
            }
        }

        jumpOverGoomba.score = 0;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // 4. Reset UI back to the corners when the game restarts
        if (scoreTextRect != null)
        {
            scoreTextRect.anchorMin = scoreOrigAnchorMin;
            scoreTextRect.anchorMax = scoreOrigAnchorMax;
            scoreTextRect.anchoredPosition = scoreOrigPos;
        }
        if (restartButtonRect != null)
        {
            restartButtonRect.anchorMin = buttonOrigAnchorMin;
            restartButtonRect.anchorMax = buttonOrigAnchorMax;
            restartButtonRect.anchoredPosition = buttonOrigPos;
        }

        // Reset camera to starting position
        gameCamera.position = new Vector3(0.47f, 1f, -36.1f);
    }
}