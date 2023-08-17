using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour
{
    public AudioSource playSFX;
    public AudioSource quitSFX;
    public Image fadeImage;
    public Sprite pressedSprite; // Drag your "pressed" sprite here
    private Button button;
    private Sprite normalSprite;

    private void Start()
    {
        button = GetComponent<Button>();
        normalSprite = button.image.sprite;
    }

    public void PlayButton()
    {
        playSFX.Play();
        button.image.sprite = pressedSprite;
        StartCoroutine(TransitionToNextScene());
    }

    public void ExitButton()
    {
        quitSFX.Play();
        button.image.sprite = pressedSprite;

#if UNITY_EDITOR
        Debug.Log("Quit game!");
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    System.Collections.IEnumerator TransitionToNextScene()
    {
        float duration = 1.0f;
        float elapsedTime = 0;

        fadeImage.gameObject.SetActive(true);

        while (elapsedTime < duration)
        {
            float alpha = elapsedTime / duration;
            fadeImage.color = new Color(0, 0, 0, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // Function to reset button sprite after button press (useful if you go back to the menu)
    public void ResetButtonSprite()
    {
        button.image.sprite = normalSprite;
    }
}
