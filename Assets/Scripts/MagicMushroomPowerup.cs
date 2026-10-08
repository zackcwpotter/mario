using UnityEngine;

public class MagicMushroomPowerup : BasePowerup
{
    protected override void Start()
    {
        base.Start();
        this.type = PowerupType.MagicMushroom;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player") && spawned)
        {
            spawned = false;
            gameObject.SetActive(false);
        }
        else if (col.gameObject.layer == 10)
        {
            if (spawned)
            {
                goRight = !goRight;

                rigidBody.AddForce(
                    Vector2.right * 3 * (goRight ? 1 : -1),
                    ForceMode2D.Impulse
                );
            }
        }
    }

    public override void SpawnPowerup()
    {
        if (rigidBody == null)
            rigidBody = GetComponent<Rigidbody2D>();

        spawned = true;

        rigidBody.AddForce(
            Vector2.right * 3,
            ForceMode2D.Impulse
        );
    }

    public override void ApplyPowerup(MonoBehaviour i)
    {
        // TODO: implement mushroom effect
    }
}