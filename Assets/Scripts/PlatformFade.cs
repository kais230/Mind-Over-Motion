using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class PlatformFade : MonoBehaviour
{
    [Header("Fade Settings")]
    [Tooltip("Seconds it takes to fade in/out")]
    public float fadeDuration = 1f;

    SpriteRenderer sr;
    Collider2D col;
    Coroutine fadeRoutine;
    bool isShown = false;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        // start invisible & non‐solid
        var c = sr.color; c.a = 0f; sr.color = c;
        col.enabled = false;
    }

    /// <summary>
    /// Fade the platform in, then enable its collider so it becomes solid.
    /// </summary>
    public void Show()
    {
        if (isShown) return;
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(Fade(0f, 1f, true));
    }

    /// <summary>
    /// Disable its collider immediately, then fade out visually.
    /// </summary>
    public void Hide()
    {
        if (!isShown) return;
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(Fade(1f, 0f, false));
    }

    IEnumerator Fade(float fromAlpha, float toAlpha, bool enableColliderAtEnd)
    {
        // on fade‐out, disable collider right away
        if (!enableColliderAtEnd)
            col.enabled = false;

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float a = Mathf.Lerp(fromAlpha, toAlpha, elapsed / fadeDuration);
            var c = sr.color; c.a = a; sr.color = c;
            yield return null;
        }

        // ensure final alpha
        var cf = sr.color; cf.a = toAlpha; sr.color = cf;

        // on fade‐in, enable collider once fully visible
        if (enableColliderAtEnd)
            col.enabled = true;

        isShown = enableColliderAtEnd;
        fadeRoutine = null;
    }
}
