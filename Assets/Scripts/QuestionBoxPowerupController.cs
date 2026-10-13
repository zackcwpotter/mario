using UnityEngine;

public class QuestionBoxPowerupController : MonoBehaviour, IPowerupController
{
    public Animator powerupAnimator;
    public BasePowerup powerup;
    public AudioClip coinSound;

    private AudioSource audioSource;
    private bool isUsed = false;
    private Vector3 originalLocalPos;
    private RigidbodyType2D originalBodyType = RigidbodyType2D.Kinematic;
    private Vector3 powerupLocalPos;
    private Vector3 animatorLocalPos;

    void Awake()
    {
        EnsureLocalReferences();
    }

    private void EnsureLocalReferences()
    {
        // 1. Locate the local powerupAnimator first
        if (powerupAnimator == null || !powerupAnimator.gameObject.scene.IsValid())
        {
            if (transform.parent != null)
            {
                Animator[] anims = transform.parent.GetComponentsInChildren<Animator>(true);
                foreach (var a in anims)
                {
                    if (a.gameObject != this.gameObject) // Don't pick the question box's own animator
                    {
                        powerupAnimator = a;
                        break;
                    }
                }
            }
        }

        // 2. If the powerupAnimator GameObject has a BasePowerup (like CoinPowerup on child 'coin'),
        // bind to that exact component! This guarantees the controller and the animator event share the SAME instance!
        if (powerupAnimator != null)
        {
            BasePowerup animPowerup = powerupAnimator.GetComponent<BasePowerup>();
            if (animPowerup != null)
            {
                powerup = animPowerup;
            }
        }

        // 3. Fallback for mushroom boxes (where MagicMushroomPowerup is on parent Powerup)
        if (powerup == null || !powerup.gameObject.scene.IsValid())
        {
            if (transform.parent != null)
            {
                powerup = transform.parent.GetComponentInChildren<BasePowerup>(true);
            }
            else
            {
                powerup = GetComponentInChildren<BasePowerup>(true);
            }
        }

        // Disable any duplicate CoinPowerup on the parent object to prevent double scoring
        if (powerup != null && transform.parent != null)
        {
            CoinPowerup[] allCoins = transform.parent.GetComponentsInChildren<CoinPowerup>(true);
            foreach (var c in allCoins)
            {
                if (c != powerup)
                {
                    c.enabled = false;
                }
            }
        }
    }

    void Start()
    {
        EnsureLocalReferences();
        audioSource = GetComponent<AudioSource>();

        originalLocalPos = transform.localPosition;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            originalBodyType = rb.bodyType;
        }

        // Store original local positions
        if (powerup != null)
        {
            powerupLocalPos = powerup.transform.localPosition;
        }
        if (powerupAnimator != null)
        {
            animatorLocalPos = powerupAnimator.transform.localPosition;
        }

        // Subscribe to game restart
        if (GameManager.instance != null)
        {
            GameManager.instance.gameRestart.AddListener(ResetBox);
        }
    }

    void OnDestroy()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.gameRestart.RemoveListener(ResetBox);
        }
    }

    public void ResetBox()
    {
        StopAllCoroutines();
        isUsed = false;

        // 1. Reset the Question Box itself
        Animator boxAnim = GetComponent<Animator>();
        if (boxAnim != null)
        {
            boxAnim.Rebind();
            boxAnim.Update(0f);
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = originalBodyType;
            rb.linearVelocity = Vector2.zero;
        }
        transform.localPosition = originalLocalPos;

        // 2. Reactivate and reset the Powerup script object
        if (powerup != null)
        {
            powerup.gameObject.SetActive(true);
            powerup.transform.localPosition = powerupLocalPos;
            powerup.spawned = false;

            if (powerup is MagicMushroomPowerup mushroom)
            {
                mushroom.ResetPowerup();
            }
            else if (powerup is CoinPowerup coin)
            {
                coin.ResetPowerup();
            }
            else
            {
                // Reset physics in case it's a generic powerup
                Rigidbody2D pRb = powerup.GetComponent<Rigidbody2D>();
                if (pRb != null)
                {
                    pRb.bodyType = RigidbodyType2D.Kinematic;
                    pRb.linearVelocity = Vector2.zero;
                }
                Collider2D pCol = powerup.GetComponent<Collider2D>();
                if (pCol != null)
                {
                    pCol.enabled = false;
                }
            }
        }

        // 3. Reactivate and re-enable the Sprite & Animator object
        if (powerupAnimator != null)
        {
            powerupAnimator.gameObject.SetActive(true);
            powerupAnimator.transform.localPosition = animatorLocalPos;
            powerupAnimator.enabled = true; // In case the mushroom script disabled it
            powerupAnimator.Rebind();       // Return animation back to idle
            powerupAnimator.Update(0f);

            // Also reset physics if attached directly to the animator object
            Rigidbody2D aRb = powerupAnimator.GetComponent<Rigidbody2D>();
            if (aRb != null)
            {
                aRb.bodyType = RigidbodyType2D.Kinematic;
                aRb.linearVelocity = Vector2.zero;
            }
            Collider2D aCol = powerupAnimator.GetComponent<Collider2D>();
            if (aCol != null)
            {
                aCol.enabled = false;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") && !isUsed)
        {
            isUsed = true;

            // Trigger disabled block animation on the box
            Animator boxAnim = GetComponent<Animator>();
            if (boxAnim != null)
            {
                boxAnim.SetTrigger("spawned");
            }

            // Trigger rising animation on the powerup
            if (powerupAnimator != null)
            {
                SpriteRenderer sr = powerupAnimator.GetComponent<SpriteRenderer>();
                if (sr != null)
                    sr.enabled = true;
                powerupAnimator.SetTrigger("spawned");
            }

            // For mushrooms, delay physical movement until rising animation completes (0.75s)
            if (powerup is MagicMushroomPowerup)
            {
                StartCoroutine(SpawnPowerupRoutine(0.75f));
            }
            else if (powerup != null)
            {
                powerup.SpawnPowerup();
            }

            // Play coin chime
            AudioClip clipToPlay = coinSound != null ? coinSound : (audioSource != null ? audioSource.clip : null);
            if (clipToPlay != null && audioSource != null)
            {
                audioSource.PlayOneShot(clipToPlay);
            }
        }
    }

    private System.Collections.IEnumerator SpawnPowerupRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (powerup != null && !powerup.hasSpawned)
        {
            powerup.SpawnPowerup();
        }
    }

    public void Disable()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Static;
        }
        transform.localPosition = originalLocalPos;
    }
}