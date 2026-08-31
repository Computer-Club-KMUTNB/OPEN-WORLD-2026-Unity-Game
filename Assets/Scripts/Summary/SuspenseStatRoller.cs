using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class SuspenseStatRoller : MonoBehaviour
{
    public static IEnumerator RollNumberCoroutine(
        TextMeshProUGUI textComponent,
        int targetNumber,
        string prefix = "",
        string suffix = "",
        float duration = 0.8f,
        int maxRandomNumber = 99,
        Action onComplete = null)
    {
        if (textComponent == null) yield break;

        float elapsed = 0f;
        float interval = 0.04f;
        Transform targetTransform = textComponent.transform;
        Vector3 originalScale = Vector3.one;
        targetTransform.localScale = originalScale;

        while (elapsed < duration)
        {
            int randomVal = UnityEngine.Random.Range(0, Mathf.Max(Mathf.Abs(targetNumber) + 10, maxRandomNumber));
            textComponent.text = $"{prefix}{randomVal}{suffix}";
            
            float scaleJitter = 1.0f + UnityEngine.Random.Range(-0.04f, 0.06f);
            targetTransform.localScale = originalScale * scaleJitter;

            elapsed += interval;
            yield return new WaitForSecondsRealtime(interval);
        }

        // Final Slam
        textComponent.text = $"{prefix}{targetNumber:N0}{suffix}";
        yield return BounceSlam(targetTransform, originalScale, 1.3f, 0.2f);

        onComplete?.Invoke();
    }

    public static IEnumerator RollTextChoicesCoroutine(
        TextMeshProUGUI textComponent,
        string[] fakeChoices,
        string finalChoice,
        float duration = 1.0f,
        Action onComplete = null)
    {
        if (textComponent == null) yield break;

        float elapsed = 0f;
        float interval = 0.05f;
        Transform targetTransform = textComponent.transform;
        Vector3 originalScale = Vector3.one;
        targetTransform.localScale = originalScale;
        int index = 0;

        while (elapsed < duration)
        {
            if (fakeChoices != null && fakeChoices.Length > 0)
            {
                textComponent.text = fakeChoices[index % fakeChoices.Length];
                index++;
            }
            
            float scaleJitter = 1.0f + UnityEngine.Random.Range(-0.05f, 0.08f);
            targetTransform.localScale = originalScale * scaleJitter;

            elapsed += interval;
            yield return new WaitForSecondsRealtime(interval);
        }

        // Slam Final
        textComponent.text = finalChoice;
        yield return BounceSlam(targetTransform, originalScale, 1.35f, 0.25f);

        onComplete?.Invoke();
    }

    public static IEnumerator BounceSlam(Transform t, Vector3 baseScale, float maxMultiplier = 1.3f, float duration = 0.2f)
    {
        if (t == null) yield break;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            
            // Elastic bounce curve
            float curve = Mathf.Sin(progress * Mathf.PI * 1.5f);
            float scale = Mathf.Lerp(maxMultiplier, 1.0f, curve);
            
            t.localScale = baseScale * scale;
            yield return null;
        }

        t.localScale = baseScale;
    }

    public static IEnumerator RevealCardWithPunch(GameObject cardObject, float duration = 0.25f)
    {
        if (cardObject == null) yield break;
        cardObject.SetActive(true);

        Transform t = cardObject.transform;
        Vector3 orig = Vector3.one;
        t.localScale = Vector3.zero;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(elapsed / duration);
            float s = Mathf.Sin(p * Mathf.PI * 0.7f) * 1.2f;
            if (p > 0.8f) s = Mathf.Lerp(1.2f, 1.0f, (p - 0.8f) / 0.2f);
            t.localScale = orig * s;
            yield return null;
        }
        t.localScale = orig;
    }
}
