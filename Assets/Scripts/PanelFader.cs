using UnityEngine;

public class PanelFader : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float fadeDuration = 0.5f;

    public void FadeOutAndDisable()
    {
        StartCoroutine(FadeOutCoroutine());
    }

    private System.Collections.IEnumerator FadeOutCoroutine()
    {
        float time = 0f;
        float startAlpha = canvasGroup.alpha;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, time / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }
}
