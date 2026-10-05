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

    // 1. New variables to control the UI positions
    public RectTransform scoreTextRect;
    public RectTransform restartButtonRect;

    // for animation
    public Animator marioAnimator;

    private Rigidbody2D marioBody;
    private bool onGroundState = true; 
    private SpriteRenderer marioSprite;
    private bool faceRightState = true;
    private Vector3 startPosition;
    private bool jumpedState = false;

    // Variables to remember where the UI originally was
    private Vector2 scoreOrigPos, scoreOrigAnchorMin, scoreOrigAnchorMax;
    private Vector2 buttonOrigPos, buttonOrigAnchorMin, buttonOrigAnchorMax;

    //To reset camera
    public Transform gameCamera;
    
    // for audio
    public AudioSource marioAudio;

    // --- NEWLY ADDED VARIABLES ---
    public AudioClip marioDeath;
    public float deathImpulse = 15;

    // state
    [System.NonSerialized]
    public bool alive = true;
    // -----------------------------

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

        // update animator state
        marioAnimator.SetBool("onGround", onGroundState);
    }

    void Update()
    {
        if (alive)
        {
            marioAnimator.SetFloat("xSpeed", Mathf.Abs(marioBody.linearVelocity.x));
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
                marioAnimator.SetTrigger("onSkid");
        }
        else if (value == 1 && !faceRightState)
        {
            faceRightState = true;
            marioSprite.flipX = false;

            if (marioBody.linearVelocity.x < -0.05f)
                marioAnimator.SetTrigger("onSkid");
        }
    }

    void FixedUpdate()
    {
        if (alive && moving)
        {
            Move(faceRightState == true ? 1 : -1);
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
        if (other.gameObject.CompareTag("Enemy") && alive)
        {
            Debug.Log("Collided with goomba!");

            // Disable collider so he falls through the ground
            GetComponent<Collider2D>().enabled = false;

            // play death animation
            marioAnimator.Play("mario-die");
            marioAudio.PlayOneShot(marioDeath);
            alive = false;
        }
    }

    void PlayDeathImpulse()
    {
        marioBody.AddForce(Vector2.up * deathImpulse, ForceMode2D.Impulse);
    }

    void GameOverScene()
    {
        // stop time
        Time.timeScale = 0.0f;

        // Trigger the game over screen (Moved from the old OnTriggerEnter2D)
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
            // Moves it 100 pixels below the exact center
            restartButtonRect.anchoredPosition = new Vector2(0, -100);
        }
    }

    public void RestartButtonCallback(int input)
    {
        ResetGame();
        Time.timeScale = 1.0f;
    }

    public void ResetGame()
    {
        // Re-enable collider for the new game
        GetComponent<Collider2D>().enabled = true;

        // reset position (using your dynamic startPosition instead of hardcoding the Vector3)
        marioBody.transform.position = startPosition;
        
        faceRightState = true;
        marioSprite.flipX = false;

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

        // reset animation and state
        marioAnimator.SetTrigger("gameRestart");
        alive = true;

        QuestionBox[] questionBoxes = FindObjectsByType<QuestionBox>(
            FindObjectsSortMode.None
        );

        foreach (QuestionBox box in questionBoxes)
        {
            box.ResetBox();
        }

        Brick[] bricks = FindObjectsByType<Brick>(
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
            // jump
            marioBody.AddForce(Vector2.up * upSpeed, ForceMode2D.Impulse);
            onGroundState = false;
            jumpedState = true;

            // update animator state
            marioAnimator.SetBool("onGround", onGroundState);
        }
    }

    public void JumpHold()
    {
        if (alive && jumpedState)
        {
            // jump higher
            marioBody.AddForce(Vector2.up * upSpeed * 30, ForceMode2D.Force);
            jumpedState = false;
        }
    }

    void Move(int value)
    {
        Vector2 movement = new Vector2(value, 0);

        // check if it doesn't go beyond maxSpeed
        if (marioBody.linearVelocity.magnitude < maxSpeed)
            marioBody.AddForce(movement * speed);
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
        // re-enable collider
        GetComponent<Collider2D>().enabled = true;

        // reset position
        marioBody.transform.position = startPosition;

        // reset sprite direction
        faceRightState = true;
        marioSprite.flipX = false;

        // reset animation
        marioAnimator.SetTrigger("gameRestart");
        alive = true;

        // reset camera position
        gameCamera.position = new Vector3(0.47f, 1f, -36.1f);
    }
}