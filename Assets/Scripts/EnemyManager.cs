using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private GameManager gameManager;

    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("Manager")
            .GetComponent<GameManager>();

        // Subscribe to stomp event for every Goomba
        foreach (Transform child in transform)
        {
            EnemyMovement enemy = child.GetComponent<EnemyMovement>();

            if (enemy != null)
            {
                enemy.OnGoombaStomp += HandleGoombaStomp;
            }
        }
    }

    private void HandleGoombaStomp()
    {
        Debug.Log("EnemyManager received stomp event");

        gameManager.IncreaseScore(1);
    }

    public void GameRestart()
    {
        foreach (Transform child in transform)
        {
            EnemyMovement enemy = child.GetComponent<EnemyMovement>();

            if (enemy != null)
            {
                enemy.GameRestart();
            }
        }
    }
}