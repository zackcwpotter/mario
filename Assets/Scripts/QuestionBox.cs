using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class QuestionBox : MonoBehaviour, IPowerupController
{
    public float bounceHeight = 0.3f;
    public float bounceDuration = 0.15f;

    public GameObject coin; 
    public AudioClip coinSound; 
    public MagicMushroomPowerup mushroom;

    private Vector3 startPosition;
    private bool isBouncing = false;

    public Sprite disabledSprite;

    private bool isUsed = false;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private AudioSource audioSource; // Added AudioSource reference

    private Vector3 mushroomStartPosition;
    private bool mushroomInitiallyActive;
    

    void Start()
    {
        startPosition = transform.localPosition;
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>(); // Get the component

        if (mushroom != null)
        {
            mushroomStartPosition = mushroom.transform.position;
            mushroomInitiallyActive = mushroom.gameObject.activeSelf;
            mushroom.gameObject.SetActive(false);
        }

        if (coin != null) coin.SetActive(false);

        GameManager gm = Object.FindFirstObjectByType<GameManager>();
        if (gm != null)
        {
            gm.gameRestart.AddListener(ResetBox);
        }
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
                    Debug.Log("Spawning mushroom!");
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
            transform.localPosition = Vector3.Lerp(startPosition, topPosition, time / bounceDuration);
            yield return null;
        }

        time = 0f;
        while (time < bounceDuration)
        {
            time += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(topPosition, startPosition, time / bounceDuration);
            yield return null;
        }

        transform.localPosition = startPosition;
        isBouncing = false;
        DisableQuestionBox();
    }

    private void SpawnCoin()
    {
        // Replaced PlayClipAtPoint with the routable AudioSource
        if (coinSound != null)
        {
            audioSource.PlayOneShot(coinSound);
        }

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
            coinObj.transform.position = Vector3.Lerp(startCoinPos, topPosition, time / duration);
            yield return null;
        }

        time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            coinObj.transform.position = Vector3.Lerp(topPosition, startCoinPos, time / duration);
            yield return null;
        }

        coinObj.SetActive(false);
    }

    private void DisableQuestionBox()
    {
        animator.enabled = false;
        spriteRenderer.sprite = disabledSprite;
    }

    public void ResetBox()
    {
        isUsed = false;
        isBouncing = false;
        transform.localPosition = startPosition;

        animator.enabled = true;
        animator.Play("QuestionBoxBlink", 0, 0f);

        if (coin != null) coin.SetActive(false);

        if (mushroom != null)
        {
            mushroom.gameObject.SetActive(false);
            mushroom.transform.position = mushroomStartPosition;
            mushroom.spawned = false;

            Rigidbody2D rb = mushroom.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
        }
    }

    public void Disable()
    {
        DisableQuestionBox();
    }
}