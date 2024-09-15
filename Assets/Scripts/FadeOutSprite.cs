using UnityEngine;
using System.Collections;

public class FadeOutSprite : MonoBehaviour
{
    public float fadeDuration = 2f;  // Time it takes to fade out
    public float fadeStartTime = 9f;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();  // Get the SpriteRenderer component
        originalColor = spriteRenderer.color;  // Store the original color of the sprite

        // Start the fading process after 9 seconds
        Invoke("StartFading", fadeStartTime);
    }

    void StartFading()
    {
        // Start the fade-out process
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float elapsedTime = 0f;

        // Gradually reduce the alpha value
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);  // Lerp from 1 to 0 over time
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;  // Wait for the next frame
        }

        // Optionally, you can destroy the GameObject after fading out
        Destroy(gameObject);
    }
}
