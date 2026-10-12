using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(QuestionBoxPowerupController))]
public class QuestionBox : MonoBehaviour
{
    public float bounceHeight = 0.3f;
    public float bounceDuration = 0.15f;

    public GameObject coin;
    public AudioClip coinSound;
    public MagicMushroomPowerup mushroom;

    private Vector3 startPosition;
    private Vector3 mushroomStartPosition;

    private bool isBouncing = false;
    private bool isUsed = false;

    private AudioSource audioSource;
    private QuestionBoxPowerupController powerupController;
    private Rigidbody2D mushroomBody;

    void Start()
    {
        startPosition = transform.localPosition;

        audioSource = GetComponent<AudioSource>();
        powerupController = GetComponent<QuestionBoxPowerupController>();

        if (coin != null)
            coin.SetActive(false);

        if (mushroom != null)
        {
            mushroomStartPosition = mushroom.transform.position;
            mushroomBody = mushroom.GetComponent<Rigidbody2D>();
            mushroom.gameObject.SetActive(false);
        }

        if (GameManager.instance != null)
            GameManager.instance.gameRestart.AddListener(ResetBox);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f && !isBouncing && !isUsed)
            {
                isUsed = true;

                StartCoroutine(Bounce());

                if (mushroom != null)
                {
                    mushroom.transform.position = mushroomStartPosition;
                    mushroom.gameObject.SetActive(true);
                    mushroom.SpawnPowerup();
                }
                else
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

        powerupController.Disable();
    }

    private void SpawnCoin()
    {
        if (coinSound != null)
            audioSource.PlayOneShot(coinSound);

        if (coin != null)
        {
            coin.SetActive(true);
            StartCoroutine(MoveCoin(coin));
        }
    }

    private IEnumerator MoveCoin(GameObject coinObj)
    {
        Vector3 startCoinPos = coinObj.transform.position;
        Vector3 topPosition = startCoinPos + Vector3.up * 1.5f;

        float duration = 0.3f;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            coinObj.transform.position = Vector3.Lerp(
                startCoinPos,
                topPosition,
                time / duration
            );

            yield return null;
        }

        time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            coinObj.transform.position = Vector3.Lerp(
                topPosition,
                startCoinPos,
                time / duration
            );

            yield return null;
        }

        coinObj.SetActive(false);
    }

    public void ResetBox()
    {
        StopAllCoroutines();

        isUsed = false;
        isBouncing = false;

        transform.localPosition = startPosition;

        powerupController.ResetBox();

        if (coin != null)
            coin.SetActive(false);

        if (mushroom != null)
        {
            mushroom.gameObject.SetActive(false);
            mushroom.transform.position = mushroomStartPosition;
            mushroom.spawned = false;

            if (mushroomBody != null)
            {
                mushroomBody.linearVelocity = Vector2.zero;
                mushroomBody.angularVelocity = 0f;
            }
        }
    }

    void OnDestroy()
    {
        if (GameManager.instance != null)
            GameManager.instance.gameRestart.RemoveListener(ResetBox);
    }
}