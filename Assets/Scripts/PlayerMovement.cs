using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 25;
    public float maxSpeed = 30;
    public float upSpeed = 10;
    private bool moving = false;

    public GameObject gameOverPanel;
    int collisionLayerMask = (1 << 6) | (1 << 7) | (1 << 8);

    // UI
    public RectTransform scoreTextRect;
    public RectTransform restartButtonRect;

    // Animation
    public Animator marioAnimator;

    private Rigidbody2D marioBody;
    private bool onGroundState = true;
    private SpriteRenderer marioSprite;
    private bool faceRightState = true;
    private Vector3 startPosition;
    private bool jumpedState = false;

    // Remember original UI position
    private Vector2 scoreOrigPos;
    private Vector2 scoreOrigAnchorMin;
    private Vector2 scoreOrigAnchorMax;
    private Vector2 scoreOrigPivot;

    private Vector2 buttonOrigPos;
    private Vector2 buttonOrigAnchorMin;
    private Vector2 buttonOrigAnchorMax;

    // Camera
    public Transform gameCamera;

    // Audio
    public AudioSource marioAudio;
    public AudioClip marioDeath;
    public float deathImpulse = 15;

    // State
    [System.NonSerialized]
    public bool alive = true;


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

        // Set score position during normal gameplay
        if (scoreTextRect != null)
        {
            scoreTextRect.anchorMin = new Vector2(0, 1);
            scoreTextRect.anchorMax = new Vector2(0, 1);
            scoreTextRect.pivot = new Vector2(0, 1);

            // Distance from top-left corner
            scoreTextRect.anchoredPosition = new Vector2(40, -40);

            // Save normal gameplay layout
            scoreOrigPos = scoreTextRect.anchoredPosition;
            scoreOrigAnchorMin = scoreTextRect.anchorMin;
            scoreOrigAnchorMax = scoreTextRect.anchorMax;
            scoreOrigPivot = scoreTextRect.pivot;
        }

        // Save restart button layout
        if (restartButtonRect != null)
        {
            buttonOrigPos = restartButtonRect.anchoredPosition;
            buttonOrigAnchorMin = restartButtonRect.anchorMin;
            buttonOrigAnchorMax = restartButtonRect.anchorMax;
        }

        // Update animator state
        marioAnimator.SetBool("onGround", onGroundState);
    }


    void Update()
    {
        if (alive)
        {
            marioAnimator.SetFloat(
                "xSpeed",
                Mathf.Abs(marioBody.linearVelocity.x)
            );
        }
    }


    void FlipMarioSprite(int value)
    {
        if (!alive)
            return;

        if (value == -1 && faceRightState)
        {
            faceRightState = false;
            marioSprite.flipX = true;

            if (marioBody.linearVelocity.x > 0.05f)
            {
                marioAnimator.SetTrigger("onSkid");
            }
        }
        else if (value == 1 && !faceRightState)
        {
            faceRightState = true;
            marioSprite.flipX = false;

            if (marioBody.linearVelocity.x < -0.05f)
            {
                marioAnimator.SetTrigger("onSkid");
            }
        }
    }


    void FixedUpdate()
    {
        if (alive && moving)
        {
            Move(faceRightState ? 1 : -1);
        }
    }


    void OnCollisionEnter2D(Collision2D col)
    {
        if ((collisionLayerMask & (1 << col.gameObject.layer)) > 0)
        {
            onGroundState = true;
            marioAnimator.SetBool("onGround", true);
        }
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy") || !alive)
            return;

        bool marioIsAbove =
            transform.position.y >
            other.transform.position.y + 0.3f;

        bool marioIsFalling =
            marioBody.linearVelocity.y < 0;

        // Mario stomped Goomba
        if (marioIsAbove && marioIsFalling)
        {
            Debug.Log("Mario stomped Goomba!");
            return;
        }

        // Mario collided with Goomba from the side
        Debug.Log("Collided with goomba!");

        GetComponent<Collider2D>().enabled = false;

        marioAnimator.Play("mario-die");
        marioAudio.PlayOneShot(marioDeath);

        alive = false;
    }


    void PlayDeathImpulse()
    {
        marioBody.AddForce(
            Vector2.up * deathImpulse,
            ForceMode2D.Impulse
        );
    }


    void GameOverScene()
    {
        Time.timeScale = 0.0f;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // Move score to Game Over screen
        if (scoreTextRect != null)
        {
            scoreTextRect.anchorMin = new Vector2(0.5f, 0.5f);
            scoreTextRect.anchorMax = new Vector2(0.5f, 0.5f);
            scoreTextRect.pivot = new Vector2(0.5f, 0.5f);

            scoreTextRect.anchoredPosition = new Vector2(0, -50);
        }

        // Move restart button to Game Over screen
        if (restartButtonRect != null)
        {
            restartButtonRect.anchorMin = new Vector2(0.5f, 0.5f);
            restartButtonRect.anchorMax = new Vector2(0.5f, 0.5f);

            restartButtonRect.anchoredPosition = new Vector2(0, -180);
        }
    }


    // Old restart callback - can still stay here
    public void RestartButtonCallback(int input)
    {
        ResetGame();
        Time.timeScale = 1.0f;
    }


    public void ResetGame()
    {
        // Re-enable Mario collider
        GetComponent<Collider2D>().enabled = true;

        // Reset Mario position
        marioBody.transform.position = startPosition;

        // Reset direction
        faceRightState = true;
        marioSprite.flipX = false;

        // Hide Game Over panel
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // Put score back in top-left corner
        if (scoreTextRect != null)
        {
            scoreTextRect.anchorMin = scoreOrigAnchorMin;
            scoreTextRect.anchorMax = scoreOrigAnchorMax;
            scoreTextRect.pivot = scoreOrigPivot;
            scoreTextRect.anchoredPosition = scoreOrigPos;
        }

        // Restore restart button position
        if (restartButtonRect != null)
        {
            restartButtonRect.anchorMin = buttonOrigAnchorMin;
            restartButtonRect.anchorMax = buttonOrigAnchorMax;
            restartButtonRect.anchoredPosition = buttonOrigPos;
        }

        // Reset camera
        gameCamera.position =
            new Vector3(0.47f, 1f, -36.1f);

        // Reset animation/state
        marioAnimator.SetTrigger("gameRestart");
        alive = true;

        // Reset question boxes
        QuestionBox[] questionBoxes =
            FindObjectsByType<QuestionBox>(
                FindObjectsSortMode.None
            );

        foreach (QuestionBox box in questionBoxes)
        {
            box.ResetBox();
        }

        // Reset bricks
        Brick[] bricks =
            FindObjectsByType<Brick>(
                FindObjectsSortMode.None
            );

        foreach (Brick brick in bricks)
        {
            brick.ResetBrick();
        }
    }


    void PlayJumpSound()
    {
        if (marioBody.linearVelocity.y > 0.1f)
        {
            marioAudio.PlayOneShot(marioAudio.clip);
        }
    }


    public void Jump()
    {
        if (alive && onGroundState)
        {
            marioBody.AddForce(
                Vector2.up * upSpeed,
                ForceMode2D.Impulse
            );

            onGroundState = false;
            jumpedState = true;

            marioAnimator.SetBool(
                "onGround",
                onGroundState
            );
        }
    }


    public void JumpHold()
    {
        if (alive && jumpedState)
        {
            marioBody.AddForce(
                Vector2.up * upSpeed * 30,
                ForceMode2D.Force
            );

            jumpedState = false;
        }
    }


    void Move(int value)
    {
        Vector2 movement = new Vector2(value, 0);

        if (marioBody.linearVelocity.magnitude < maxSpeed)
        {
            marioBody.AddForce(movement * speed);
        }
    }


    public void MoveCheck(int value)
    {
        Debug.Log($"MoveCheck called with value: {value}");

        if (value == 0)
        {
            moving = false;
        }
        else
        {
            FlipMarioSprite(value);

            moving = true;

            Move(value);
        }
    }


    public void GameRestart()
    {
        // Re-enable collider
        GetComponent<Collider2D>().enabled = true;

        // Reset position
        marioBody.transform.position = startPosition;

        // Reset sprite direction
        faceRightState = true;
        marioSprite.flipX = false;

        // Reset animation
        marioAnimator.SetTrigger("gameRestart");
        alive = true;

        // Reset camera
        gameCamera.position =
            new Vector3(0.47f, 1f, -36.1f);
    }
}