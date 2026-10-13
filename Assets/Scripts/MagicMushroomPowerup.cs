using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicMushroomPowerup : BasePowerup
{
    [Header("Audio")]
    public AudioClip powerupSound; // Drag smb_powerup.wav here in Inspector

    [Header("Movement")]
    public float speed = 1.5f;     // Gentle, soft movement
    public bool goRight = true;    // Always moves to the RIGHT by default

    private Collider2D[] allColliders;
    private Rigidbody2D[] allRigidbodies;
    private Rigidbody2D activeRb;
    private Collider2D activeCol;
    private Animator anim;
    private SpriteRenderer sr;
    private Vector3 startLocalPos;
    private Vector3 childStartLocalPos;
    private Transform childTransform;

    protected override void Start()
    {
        base.Start();
        this.type = PowerupType.MagicMushroom;
        startLocalPos = transform.localPosition;

        FindComponents();

        if (childTransform != null)
        {
            childStartLocalPos = childTransform.localPosition;
        }

        // Keep mushroom hidden, unsimulated, and all colliders OFF at start
        SetDormantState();
    }

    private void FindComponents()
    {
        allColliders = GetComponentsInChildren<Collider2D>(true);
        allRigidbodies = GetComponentsInChildren<Rigidbody2D>(true);
        anim = GetComponentInChildren<Animator>(true);
        sr = GetComponentInChildren<SpriteRenderer>(true);

        Transform child = transform.Find("MagicMushroom");
        if (child != null)
        {
            childTransform = child;
            // Prefer child's components if present, otherwise fallback to parent
            activeRb = child.GetComponent<Rigidbody2D>() ?? GetComponent<Rigidbody2D>();
            activeCol = child.GetComponent<Collider2D>() ?? GetComponent<Collider2D>();
        }
        else
        {
            activeRb = GetComponent<Rigidbody2D>() ?? GetComponentInChildren<Rigidbody2D>();
            activeCol = GetComponent<Collider2D>() ?? GetComponentInChildren<Collider2D>();
        }
    }

    private void SetDormantState()
    {
        // 1. Hide sprite until block is bumped
        if (sr != null)
            sr.enabled = false;

        // 2. Ensure ALL colliders on parent and child are completely OFF
        if (allColliders == null || allColliders.Length == 0)
            allColliders = GetComponentsInChildren<Collider2D>(true);
        foreach (var col in allColliders)
        {
            if (col != null) col.enabled = false;
        }

        // 3. Ensure ALL rigidbodies on parent and child are Kinematic and not simulated
        if (allRigidbodies == null || allRigidbodies.Length == 0)
            allRigidbodies = GetComponentsInChildren<Rigidbody2D>(true);
        foreach (var rb in allRigidbodies)
        {
            if (rb != null)
            {
                rb.simulated = false;
                rb.bodyType = RigidbodyType2D.Kinematic;
                rb.linearVelocity = Vector2.zero;
            }
        }
    }

    void FixedUpdate()
    {
        // Continuously drive soft horizontal movement while dynamic
        if (spawned && activeRb != null && activeRb.bodyType == RigidbodyType2D.Dynamic)
        {
            activeRb.linearVelocity = new Vector2((goRight ? 1f : -1f) * speed, activeRb.linearVelocity.y);
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        HandleCollision(col);
    }

    public void OnCustomCollisionEnter2D(Collision2D col)
    {
        HandleCollision(col);
    }

    private void HandleCollision(Collision2D col)
    {
        if (!spawned) return;

        // 1. Mario consumes the mushroom
        if (col.gameObject.CompareTag("Player"))
        {
            PlayerMovement player = col.gameObject.GetComponent<PlayerMovement>();
            if (player != null && player.marioAudio != null)
            {
                if (powerupSound != null)
                {
                    player.marioAudio.PlayOneShot(powerupSound);
                }
                else
                {
                    Debug.LogWarning("[MagicMushroomPowerup] powerupSound is not assigned in the Inspector!");
                }
            }

            ApplyPowerup(this);
            gameObject.SetActive(false); // Hide / consume
        }
        // 2. Flip direction when hitting Pipe (layer 10) or a vertical obstacle (layer 8 wall)
        // Checking contacts[0].normal.x ensures it flips ONLY on walls, NEVER on the floor/box top!
        else if (col.gameObject.layer == 10 || (col.gameObject.layer == 8 && col.contactCount > 0 && Mathf.Abs(col.contacts[0].normal.x) > 0.5f))
        {
            goRight = !goRight;
            if (activeRb != null)
            {
                activeRb.linearVelocity = new Vector2((goRight ? 1f : -1f) * speed, activeRb.linearVelocity.y);
            }
        }
    }

    // Called when the rising animation completes (0.75s)
    public override void SpawnPowerup()
    {
        if (spawned) return;
        spawned = true;

        // Disable Animator so it does not override physics position
        if (anim != null)
            anim.enabled = false;

        // Ensure sprite is visible
        if (sr != null)
            sr.enabled = true;

        // Ensure mushroom is sitting cleanly on top of the block before turning collider on
        // Box top is at root y = 0.51. Placing at y = 1.0 puts bottom at ~0.55 (safely resting on top of the block)
        if (childTransform != null)
        {
            childTransform.localPosition = new Vector3(childTransform.localPosition.x, 1.0f, 0f);
        }
        else
        {
            transform.localPosition = new Vector3(transform.localPosition.x, 1.0f, 0f);
        }

        // Enable the active collider now that rising is complete
        if (activeCol != null)
        {
            activeCol.enabled = true;
        }

        // Turn on dynamic physics and start soft movement to the left
        if (activeRb != null)
        {
            activeRb.simulated = true;
            activeRb.bodyType = RigidbodyType2D.Dynamic;
            activeRb.constraints = RigidbodyConstraints2D.FreezeRotation;
            activeRb.linearVelocity = new Vector2((goRight ? 1f : -1f) * speed, 0f);
        }
    }

    public override void ApplyPowerup(MonoBehaviour i)
    {
        Debug.Log("Magic Mushroom consumed by Mario!");
    }

    public void TriggerSpawn()
    {
        SpawnPowerup();
    }

    public void ResetPowerup()
    {
        StopAllCoroutines();
        spawned = false;
        goRight = true; // Reset to always moving right
        transform.localPosition = startLocalPos;
        if (childTransform != null)
        {
            childTransform.localPosition = childStartLocalPos;
        }
        gameObject.SetActive(true);

        SetDormantState();

        if (anim != null)
        {
            anim.enabled = true;
            anim.Rebind();
            anim.Play("mushroom_idle");
        }
    }
}