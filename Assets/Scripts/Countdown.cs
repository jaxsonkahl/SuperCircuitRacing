using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Countdown : MonoBehaviour
{
    public int countdownTime;
    public TMP_Text countdownDisplay;
    public CarController car;

    private void Start()
    {
        StartCoroutine(CountdownToStart());
    }

    IEnumerator CountdownToStart()
    {
        while (countdownTime > 0)
        {
            countdownDisplay.text = countdownTime.ToString();
            StartCoroutine(AnimateText());  // Play scale animation
            yield return new WaitForSeconds(1f);
            countdownTime--;
        }

        countdownDisplay.text = "GO!";
        StartCoroutine(AnimateText());  // Animate "GO!" too
        car.canMove = true;
        yield return new WaitForSeconds(1f);
        countdownDisplay.gameObject.SetActive(false);
    }

    IEnumerator AnimateText()
    {
        Vector3 originalScale = countdownDisplay.transform.localScale;
        Vector3 targetScale = originalScale * 1.5f;
        float duration = 0.2f;
        float t = 0f;

        // Scale up
        while (t < duration)
        {
            countdownDisplay.transform.localScale = Vector3.Lerp(originalScale, targetScale, t / duration);
            t += Time.deltaTime;
            yield return null;
        }

        countdownDisplay.transform.localScale = targetScale;

        // Scale back down
        t = 0f;
        while (t < duration)
        {
            countdownDisplay.transform.localScale = Vector3.Lerp(targetScale, originalScale, t / duration);
            t += Time.deltaTime;
            yield return null;
        }

        countdownDisplay.transform.localScale = originalScale;
    }
}
