using System.Collections;
using UnityEngine;

public class QuestionBox : MonoBehaviour
{
    public float bounceHeight = 0.3f;
    public float bounceDuration = 0.15f;

    public GameObject coinPrefab;

    private Vector3 startPosition;
    private bool isBouncing = false;

    public Sprite disabledSprite;

    private bool isUsed = false;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    void Start()
    {
        startPosition = transform.localPosition;

        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Only react to Mario
        if (!collision.gameObject.CompareTag("Player"))
            return;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            // Mario hit the box from below
            if (contact.normal.y > 0.5f && !isBouncing && !isUsed)
            {
                isUsed = true;

                StartCoroutine(Bounce());
                SpawnCoin();

                DisableQuestionBox();

                break;
            }
        }
    }

    private IEnumerator Bounce() 
    {
        isBouncing = true;

        Vector3 topPosition = startPosition + Vector3.up * bounceHeight;

        // Move up
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

        // Move back down
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
        GameObject coin = Instantiate(
            coinPrefab,
            transform.position + Vector3.up * 0.5f,
            Quaternion.identity
        );

        StartCoroutine(MoveCoin(coin));
    }

    private IEnumerator MoveCoin(GameObject coin)
    {
        Vector3 startPosition = coin.transform.position;
        Vector3 topPosition = startPosition + Vector3.up * 1.5f;

        float duration = 0.3f;
        float time = 0f;

        // Move coin up
        while (time < duration)
        {
            time += Time.deltaTime;

            coin.transform.position = Vector3.Lerp(
                startPosition,
                topPosition,
                time / duration
            );

            yield return null;
        }

        // Move coin back down
        time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            coin.transform.position = Vector3.Lerp(
                topPosition,
                startPosition,
                time / duration
            );

            yield return null;
        }

        Destroy(coin);
    }

    private void DisableQuestionBox()
    {
        // Stop blinking animation
        animator.enabled = false;

        // Show disabled box
        spriteRenderer.sprite = disabledSprite;
    }


}