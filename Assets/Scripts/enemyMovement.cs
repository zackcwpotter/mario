using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EnemyMovement : MonoBehaviour
{
    public Vector3 startPosition;
    public AudioClip stompSound;
    public Sprite stompedSprite;

    // Event + delegate
    public delegate void GoombaStompHandler();
    public event GoombaStompHandler OnGoombaStomp;

    private float originalX;
    private float maxOffset = 5.0f;
    private float enemyPatroltime = 2.0f;
    private int moveRight = -1;
    private Vector2 velocity;

    private Rigidbody2D enemyBody;
    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;
    private Sprite originalSprite;

    private bool stomped = false;

    void Start()
    {
        enemyBody = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        originalSprite = spriteRenderer.sprite;

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
            enemyBody.position + velocity * Time.fixedDeltaTime
        );
    }

    void FixedUpdate()
    {
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

        Rigidbody2D marioBody = other.GetComponent<Rigidbody2D>();

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

            // Tell subscribers that this Goomba was stomped
            OnGoombaStomp?.Invoke();
        }
    }

    private void Stomp()
    {
        if (stomped)
            return;

        stomped = true;
        velocity = Vector2.zero;

        // Sound
        PlayStompSound();

        // Flat Goomba
        spriteRenderer.sprite = stompedSprite;

        // Disappear shortly afterwards
        StartCoroutine(HideAfterStomp());
    }

    private IEnumerator HideAfterStomp()
    {
        yield return new WaitForSeconds(0.5f);

        spriteRenderer.enabled = false;
        GetComponent<Collider2D>().enabled = false;
    }

    public void PlayStompSound()
    {
        if (stompSound != null)
        {
            audioSource.PlayOneShot(stompSound);
        }
    }

    public void GameRestart()
    {
        stomped = false;

        spriteRenderer.enabled = true;
        spriteRenderer.sprite = originalSprite;
        GetComponent<Collider2D>().enabled = true;

        transform.localPosition = startPosition;
        originalX = transform.position.x;

        moveRight = -1;
        ComputeVelocity();
    }
}