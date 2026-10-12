using System.Collections;
using UnityEngine;

public class MagicMushroomPowerup : BasePowerup
{
    private Collider2D col2D;
    private Animator anim;

    protected override void Start()
    {
        base.Start();
        this.type = PowerupType.MagicMushroom;

        col2D = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();

        // Keep physics & collider off while hidden inside the box
        if (rigidBody != null)
            rigidBody.bodyType = RigidbodyType2D.Kinematic;
        if (col2D != null)
            col2D.enabled = false;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player") && spawned)
        {
            ApplyPowerup(this);
            DestroyPowerup();
        }
        else if (col.gameObject.layer == 8 || col.gameObject.layer == 10) // Obstacles or Pipes
        {
            if (spawned)
            {
                goRight = !goRight;
                rigidBody.linearVelocity = new Vector2((goRight ? 1 : -1) * 3f, rigidBody.linearVelocity.y);
            }
        }
    }

    // Called by Animation Event at end of mushroom_spawn.anim
    public override void SpawnPowerup()
    {
        if (rigidBody == null)
            rigidBody = GetComponent<Rigidbody2D>();
        if (col2D == null)
            col2D = GetComponent<Collider2D>();
        if (anim == null)
            anim = GetComponent<Animator>();

        spawned = true;

        // 1. Turn off Animator so it stops locking the Transform position
        if (anim != null)
            anim.enabled = false;

        // 2. Turn collider back ON so it has a hitbox to collide with Mario & ground
        if (col2D != null)
            col2D.enabled = true;

        // 3. Switch physics to Dynamic and freeze Z rotation
        rigidBody.bodyType = RigidbodyType2D.Dynamic;
        rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;

        // 4. Start moving
        StartCoroutine(PushMushroom());
    }

    private IEnumerator PushMushroom()
    {
        // Wait 1 physics frame for Dynamic body type to register
        yield return new WaitForFixedUpdate();
        rigidBody.linearVelocity = new Vector2(3f, rigidBody.linearVelocity.y);
    }

    public override void ApplyPowerup(MonoBehaviour i)
    {
        Debug.Log("Magic Mushroom consumed by Mario!");
    }

    public void TriggerSpawn()
    {
        SpawnPowerup();
    }
}