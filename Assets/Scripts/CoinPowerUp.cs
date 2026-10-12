using UnityEngine;

public class CoinPowerup : BasePowerup
{
    protected override void Start()
    {
        base.Start();
        this.type = PowerupType.Coin;
    }

    // Called by the Animation Event to award points
    public override void SpawnPowerup()
    {
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

    // Called by the Animation Event at the final frame of coinjumping
    public void DestroyPowerup()
    {
        if(gameObject != null)
        {
            Destroy(gameObject);
        }
        
    }
}