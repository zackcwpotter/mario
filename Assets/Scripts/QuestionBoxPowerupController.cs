using UnityEngine;
public class QuestionBoxPowerupController : MonoBehaviour, IPowerupController
{
    public Animator powerupAnimator;
    public BasePowerup powerup;
    private bool isUsed = false; // 1. Prevents activating an already-used block

    private void OnCollisionEnter2D(Collision2D other)
    {
        // 2. Check if the block has already been used, and check that powerup is not null
        if (other.gameObject.CompareTag("Player") && !isUsed && powerup != null && !powerup.hasSpawned)
        {
            isUsed = true;

            // Trigger disabled block animation on the box
            GetComponent<Animator>().SetTrigger("spawned");

            // 3. Null check: only trigger the animator if it hasn't been destroyed
            if (powerupAnimator != null)
            {
                powerupAnimator.SetTrigger("spawned");
            }
        }
    }

    // Called by the Animation Event in question_disabled.anim
    public void Disable()
    {
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        transform.localPosition = Vector3.zero;
    }
}