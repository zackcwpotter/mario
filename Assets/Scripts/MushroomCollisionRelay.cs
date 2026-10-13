using UnityEngine;

public class MushroomCollisionRelay : MonoBehaviour
{
    private MagicMushroomPowerup parentPowerup;

    void Awake()
    {
        parentPowerup = GetComponentInParent<MagicMushroomPowerup>();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (parentPowerup != null)
        {
            parentPowerup.OnCustomCollisionEnter2D(col);
        }
    }

    // Also forwards the Animation Event if triggered on child Animator
    public void TriggerSpawn()
    {
        if (parentPowerup != null)
        {
            parentPowerup.TriggerSpawn();
        }
    }
}

