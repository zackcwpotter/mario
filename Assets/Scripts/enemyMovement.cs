using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EnemyMovement : MonoBehaviour
{
    public Vector3 startPosition;

    // Event and delegate for Goomba stomp
    public delegate void GoombaStompHandler();
    public event GoombaStompHandler OnGoombaStomp;

    // Goomba movement
    private float originalX;
    private float maxOffset = 5.0f;
    private float enemyPatroltime = 2.0f;
    private int moveRight = -1;
    private Vector2 velocity;

    private Rigidbody2D enemyBody;

    // Sprite
    public Sprite stompedSprite;
    private SpriteRenderer spriteRenderer;
    private Sprite originalSprite;

    // Stomp state
    private bool stomped = false;

    // Audio
    public AudioClip stompSound;
    private AudioSource audioSource;


    void Start()
    {
        enemyBody = GetComponent<Rigidbody2D>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        originalSprite = spriteRenderer.sprite;

        audioSource = GetComponent<AudioSource>();

        startPosition = transform.localPosition;
        originalX = transform.position.x;

        ComputeVelocity();
    }


    void ComputeVelocity()
    {
        velocity = new Vector2(
            moveRight * maxOffset / enemyPatroltime,
            0
        );
    }


    void Movegoomba()
    {
        enemyBody.MovePosition(
            enemyBody.position +
            velocity * Time.fixedDeltaTime
        );
    }


    void FixedUpdate()
    {
        // Don't move after being stomped
        if (stomped)
            return;

        if (Mathf.Abs(enemyBody.position.x - originalX) < maxOffset)
        {
            Movegoomba();
        }
        else
        {
            moveRight *= -1;
            ComputeVelocity();
            Movegoomba();
        }
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || stomped)
            return;

        Rigidbody2D marioBody =
            other.GetComponent<Rigidbody2D>();

        bool marioIsAbove =
            other.transform.position.y >
            transform.position.y + 0.3f;

        bool marioIsFalling =
            marioBody != null &&
            marioBody.linearVelocity.y < 0;

        if (marioIsAbove && marioIsFalling)
        {
            Debug.Log("GOOMBA STOMPED!");

            Stomp();

            // Notify subscribers that Goomba was stomped
            OnGoombaStomp?.Invoke();
        }
    }


    private void Stomp()
    {
        if (stomped)
            return;

        stomped = true;

        // Stop Goomba
        velocity = Vector2.zero;

        // Play stomp sound
        if (stompSound != null)
        {
            audioSource.PlayOneShot(stompSound);
        }

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


    public void GameRestart()
    {
        stomped = false;

        // Restore normal Goomba
        spriteRenderer.enabled = true;
        spriteRenderer.sprite = originalSprite;
        GetComponent<Collider2D>().enabled = true;

        // Reset position
        transform.localPosition = startPosition;
        originalX = transform.position.x;

        // Reset movement
        moveRight = -1;
        ComputeVelocity();
    }


    // Can still be called separately if needed
    public void PlayStompSound()
    {
        if (stompSound != null)
        {
            audioSource.PlayOneShot(stompSound);
        }
    }
}