using System.Collections;
using UnityEngine;

public class BrickPowerupController : MonoBehaviour, IPowerupController
{
    [Header("Powerup Setup")]
    public bool hasCoin = false;
    public BasePowerup powerup;
    public Animator powerupAnimator;
    public AudioClip coinSound;

    [Header("Brick Settings")]
    public bool breakable = false;
    public float bounceHeight = 0.3f;
    public float bounceDuration = 0.15f;

    private AudioSource audioSource;
    private Vector3 startPosition;
    private Vector3 powerupStartPosition;
    private bool isBouncing = false;

    void Awake()
    {
        EnsureLocalReferences();
    }

    private void EnsureLocalReferences()
    {
        if (powerup == null || !powerup.gameObject.scene.IsValid())
        {
            powerup = GetComponentInChildren<BasePowerup>(true);
        }

        if (powerupAnimator == null || !powerupAnimator.gameObject.scene.IsValid())
        {
            if (powerup != null)
            {
                powerupAnimator = powerup.GetComponentInChildren<Animator>(true);
            }
            else
            {
                powerupAnimator = GetComponentInChildren<Animator>(true);
            }
        }
    }

    void Start()
    {
        EnsureLocalReferences();
        startPosition = transform.localPosition;
        audioSource = GetComponent<AudioSource>();

        if (powerup != null)
        {
            powerupStartPosition = powerup.transform.localPosition;
        }

        // If this brick starts without a coin, hide the coin object
        if (!hasCoin && powerup != null)
        {
            powerup.gameObject.SetActive(false);
        }

        // Subscribe to game restart event
        if (GameManager.instance != null)
        {
            GameManager.instance.gameRestart.AddListener(ResetBrick);
        }
    }

    void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        if (GameManager.instance != null)
        {
            GameManager.instance.gameRestart.RemoveListener(ResetBrick);
        }
    }

    public void ResetBrick()
    {
        StopAllCoroutines();
        isBouncing = false;
        transform.localPosition = startPosition;

        // If it has a coin, reset its position, state, and animation
        if (hasCoin && powerup != null)
        {
            powerup.spawned = false;
            powerup.transform.localPosition = powerupStartPosition;
            powerup.gameObject.SetActive(true);

            // Rebind the coin animator back to idle
            if (powerupAnimator != null)
            {
                powerupAnimator.enabled = true;
                powerupAnimator.Rebind();
                powerupAnimator.Update(0f);
            }
        }
        else if (powerup != null)
        {
            powerup.gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        // Only trigger when Mario hits from below
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f && !isBouncing)
            {
                // 1. Brick always bounces
                StartCoroutine(Bounce());

                // 2. If it has a coin that hasn't spawned yet, spawn it and play the chime
                if (hasCoin && powerup != null && !powerup.hasSpawned)
                {
                    if (powerupAnimator != null)
                    {
                        powerupAnimator.SetTrigger("spawned");
                    }
                    powerup.SpawnPowerup();

                    // Play chime (uses coinSound or falls back to AudioSource's clip)
                    AudioClip clipToPlay = coinSound != null ? coinSound : (audioSource != null ? audioSource.clip : null);
                    if (clipToPlay != null && audioSource != null)
                    {
                        audioSource.PlayOneShot(clipToPlay);
                    }
                }

                break;
            }
        }
    }

    private IEnumerator Bounce()
    {
        isBouncing = true;
        Vector3 topPosition = startPosition + Vector3.up * bounceHeight;

        float time = 0f;
        while (time < bounceDuration)
        {
            time += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(startPosition, topPosition, time / bounceDuration);
            yield return null;
        }

        time = 0f;
        while (time < bounceDuration)
        {
            time += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(topPosition, startPosition, time / bounceDuration);
            yield return null;
        }

        transform.localPosition = startPosition;
        isBouncing = false;
    }

    // Required by IPowerupController interface
    public void Disable() { }
}