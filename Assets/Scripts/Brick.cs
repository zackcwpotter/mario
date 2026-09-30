using System.Collections;
using UnityEngine;

public class Brick : MonoBehaviour
{
    public bool hasCoin = false;
    public GameObject coinPrefab;
    public AudioClip coinSound; // Slot for your coin sound effect in the Inspector

    public float bounceHeight = 0.3f;
    public float bounceDuration = 0.15f;

    private Vector3 startPosition;
    private bool isUsed = false;
    private bool isBouncing = false;

    void Start()
    {
        startPosition = transform.localPosition;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            // Mario hits brick from below
            if (contact.normal.y > 0.5f && !isUsed && !isBouncing)
            {
                isUsed = true;

                StartCoroutine(Bounce());

                if (hasCoin)
                {
                    SpawnCoin();
                }

                break;
            }
        }
    }

    private IEnumerator Bounce()
    {
        isBouncing = true;

        Vector3 topPosition = startPosition + Vector3.up * bounceHeight;

        float time = 0f;

        while (time < bounceDuration)
        {
            time += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(
                startPosition,
                topPosition,
                time / bounceDuration
            );

            yield return null;
        }

        time = 0f;

        while (time < bounceDuration)
        {
            time += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(
                topPosition,
                startPosition,
                time / bounceDuration
            );

            yield return null;
        }

        transform.localPosition = startPosition;
        isBouncing = false;
    }

    private void SpawnCoin()
    {
        // Play the sound at the camera's position so it isn't muted
        if (coinSound != null)
        {
            AudioSource.PlayClipAtPoint(coinSound, Camera.main.transform.position);
        }

        GameObject coin = Instantiate(
            coinPrefab,
            transform.position + Vector3.up * 0.5f,
            Quaternion.identity
        );

        StartCoroutine(MoveCoin(coin));
    }

    private IEnumerator MoveCoin(GameObject coin)
    {
        Vector3 startCoinPosition = coin.transform.position;
        Vector3 topPosition = startCoinPosition + Vector3.up * 1.5f;

        float duration = 0.3f;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            coin.transform.position = Vector3.Lerp(
                startCoinPosition,
                topPosition,
                time / duration
            );

            yield return null;
        }

        time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            coin.transform.position = Vector3.Lerp(
                topPosition,
                startCoinPosition,
                time / duration
            );

            yield return null;
        }

        Destroy(coin);
    }

    public void ResetBrick()
    {
        isUsed = false;
        isBouncing = false;

        transform.localPosition = startPosition;
    }
}