using UnityEngine;


public interface IPowerup
{
    void DestroyPowerup();
    void SpawnPowerup();
    void ApplyPowerup(MonoBehaviour i);
    PowerupType powerupType { get; }
    bool hasSpawned { get; }
}

public interface IPowerupApplicable
{
    void RequestPowerupEffect(IPowerup i);
}