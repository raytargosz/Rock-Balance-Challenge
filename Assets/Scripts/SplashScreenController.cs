using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SplashScreenController : MonoBehaviour
{
    public TextMeshProUGUI[] titleTexts;
    public TextMeshProUGUI promptText;
    public GameObject playButton;
    public GameObject exitButton;
    public AudioSource anyKeySFX;

    private bool hasStarted = false;

    void Update()
    {
        if (!hasStarted && Input.anyKey)
        {
            hasStarted = true;
            anyKeySFX.Play();

            foreach (TextMeshProUGUI title in titleTexts)
            {
                StartCoroutine(FadeOutText(title));
            }

            StartCoroutine(FadeOutText(promptText));
        }
    }

    System.Collections.IEnumerator FadeOutText(TextMeshProUGUI text)
    {
        float duration = 1.0f;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            float alpha = 1 - (elapsedTime / duration);
            text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        text.gameObject.SetActive(false);

        if (text == promptText)
        {
            // Once done, activate the buttons
            yield return new WaitForSeconds(1f);
            playButton.SetActive(true);
            exitButton.SetActive(true);
        }
    }
}