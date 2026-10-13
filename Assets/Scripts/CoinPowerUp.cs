using UnityEngine;

public class CoinPowerup : BasePowerup
{
    private Vector3 startPosition;

    protected override void Start()
    {
        base.Start();
        this.type = PowerupType.Coin;
        startPosition = transform.localPosition;
    }

    public override void SpawnPowerup()
    {
        if (spawned) return;
        spawned = true;
        ApplyPowerup(this);
    }

    public override void ApplyPowerup(MonoBehaviour i)
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.IncreaseScore(1);
        }
    }

    // 1. Deactivate instead of deleting
    public void DestroyPowerup()
    {
        gameObject.SetActive(false);
    }

    // 2. Called on restart
    public void ResetPowerup()
    {
        spawned = false;
        transform.localPosition = startPosition;
        gameObject.SetActive(true);
        Animator anim = GetComponent<Animator>() ?? GetComponentInChildren<Animator>();
        if (anim != null)
        {
            anim.Play("idle");
        }
    }
}