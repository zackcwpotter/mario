using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public Vector3 startPosition;

    public delegate void GoombaStompHandler();
    public event GoombaStompHandler OnGoombaStomp;

    private float originalX;
    private float maxOffset = 5.0f;
    private float enemyPatroltime = 2.0f;
    private int moveRight = -1;
    private Vector2 velocity;

    private Rigidbody2D enemyBody;

    public Sprite stompedSprite;

    private SpriteRenderer spriteRenderer;
    private bool stomped = false;

    private Sprite originalSprite;

    public AudioSource goombaAudio;
    public AudioClip stompSound;

    void Start()
    {
        
        enemyBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalSprite = spriteRenderer.sprite;
        
        //Save the starting position when the game starts
        startPosition = transform.localPosition; 
        
        // get the starting position
        originalX = transform.position.x;
        ComputeVelocity();
    }

    void ComputeVelocity()
    {
        velocity = new Vector2((moveRight) * maxOffset / enemyPatroltime, 0);
    }

    void Movegoomba()
    {
        enemyBody.MovePosition(enemyBody.position + velocity * Time.fixedDeltaTime);
    }

    void FixedUpdate()
    {
        if (Mathf.Abs(enemyBody.position.x - originalX) < maxOffset)
        {
            // move goomba
            Movegoomba();
        }
        else
        {
            // change direction
            moveRight *= -1;
            ComputeVelocity();
            Movegoomba();
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || stomped)
            return;

        Rigidbody2D marioBody = other.GetComponent<Rigidbody2D>();

        bool marioIsAbove =
            other.transform.position.y > transform.position.y + 0.3f;

        bool marioIsFalling =
            marioBody != null && marioBody.linearVelocity.y < 0;

        if (marioIsAbove && marioIsFalling)
        {
            Debug.Log("GOOMBA STOMPED!");

            Stomp();
            OnGoombaStomp?.Invoke();
        }
    }

    public void GameRestart()
    {
        stomped = false;

        // Restore normal Goomba
        spriteRenderer.enabled = true;
        spriteRenderer.sprite = originalSprite;
        GetComponent<Collider2D>().enabled = true;

        // Reset position and movement
        transform.localPosition = startPosition;
        originalX = transform.position.x;
        moveRight = -1;
        ComputeVelocity();
    }

    private void Stomp()
    {
        if (stomped)
            return;

        stomped = true;

        // Stop Goomba
        velocity = Vector2.zero;

        // Play stomp sound
        goombaAudio.PlayOneShot(stompSound);

        // Make Goomba flat
        spriteRenderer.sprite = stompedSprite;

        // Hide Goomba after a short delay
        StartCoroutine(HideAfterStomp());
    }

    private IEnumerator HideAfterStomp()
    {
        yield return new WaitForSeconds(0.5f);

        spriteRenderer.enabled = false;
        GetComponent<Collider2D>().enabled = false;
    }
}