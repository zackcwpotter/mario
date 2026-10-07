using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 25;
    public float maxSpeed = 30;
    public float upSpeed = 10;
    private bool moving = false;

    // Added reference to GameManager as per the refactoring instructions
    public GameManager gameManager;

    int collisionLayerMask = (1 << 6) | (1 << 7) | (1 << 8);

    // for animation
    public Animator marioAnimator;

    private Rigidbody2D marioBody;
    private bool onGroundState = true; 
    private SpriteRenderer marioSprite;
    private bool faceRightState = true;
    private Vector3 startPosition;
    private bool jumpedState = false;

    //To reset camera
    public Transform gameCamera;
    
    // for audio
    public AudioSource marioAudio;

    public AudioSource marioDeathAudio;
    public float deathImpulse = 15;

    // state
    [System.NonSerialized]
    public bool alive = true;

    void Start()
    {
        Application.targetFrameRate = 30;
        marioBody = GetComponent<Rigidbody2D>();
        marioSprite = GetComponent<SpriteRenderer>();
        startPosition = transform.position;

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
            marioDeathAudio.Play();
            alive = false;
        }
    }

    void PlayDeathImpulse()
    {
        marioBody.AddForce(Vector2.up * deathImpulse, ForceMode2D.Impulse);
    }

    void GameOverScene()
    {
        // Delegate the GameOver handling entirely to the GameManager
        if (gameManager != null)
        {
            gameManager.GameOver();
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