using UnityEngine;

public class BrickPowerupController : MonoBehaviour, IPowerupController
{
    public Animator powerupAnimator;
    public BasePowerup powerup; // Drag your CoinPowerup child object here
    public bool breakable = false; // Toggle in Inspector!

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // If the brick has a powerup (like a Coin) that hasn't spawned yet
            if (powerup != null && !powerup.hasSpawned)
            {
                // Play the coin pop-up animation & mark as spawned
                powerupAnimator.SetTrigger("spawned");
                powerup.SpawnPowerup();

                // Only show the disabled/used block sprite if it is NOT breakable
                if (!breakable)
                {
                    this.GetComponent<Animator>().SetTrigger("spawned");
                }
            }
            // If it IS breakable (or already spawned its coin), let the brick bounce!
            else if (breakable)
            {
                this.GetComponent<Animator>().SetTrigger("bounce");
            }
        }
    }

    // Called by the Brick's Animator when transitioning to the disabled state
    public void Disable()
    {
        if (!breakable)
        {
            this.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            transform.localPosition = Vector3.zero;
        }
    }
}