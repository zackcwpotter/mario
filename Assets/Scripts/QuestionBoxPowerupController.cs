using UnityEngine;

public class QuestionBoxPowerupController : MonoBehaviour, IPowerupController
{
    public Sprite disabledSprite;

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Sprite originalSprite;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        if (spriteRenderer != null)
            originalSprite = spriteRenderer.sprite;
    }

    public void Disable()
    {
        if (animator != null)
            animator.enabled = false;

        if (spriteRenderer != null && disabledSprite != null)
            spriteRenderer.sprite = disabledSprite;
    }

    public void ResetBox()
    {
        if (spriteRenderer != null)
            spriteRenderer.sprite = originalSprite;

        if (animator != null)
        {
            animator.enabled = true;
            animator.Play("QuestionBoxBlink", 0, 0f);
        }
    }
}