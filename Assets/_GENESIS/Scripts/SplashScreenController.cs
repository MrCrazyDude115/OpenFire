using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashScreenController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI startText;
    [SerializeField] private Image blackFadeImage;

    [Header("Scenes")]
    [SerializeField] private string nextScene;

    [Header("Timing")]
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private float startTextFlashInterval = 0.5f;

    private bool gameStarted = false;
    void Start()
    {
        StartCoroutine(FlashStartText());
    }

    private void Update()
    {
        if (!gameStarted)
        {
            if (Input.GetMouseButtonDown(0))
            {
                gameStarted = true;
                StartCoroutine(LoadMainMenu());
            }
        }
    }
    IEnumerator FlashStartText()
    {
        while (gameStarted == false)
        {
            startText.enabled = true;
            yield return new WaitForSeconds(startTextFlashInterval);

            startText.enabled = false;
            yield return new WaitForSeconds(startTextFlashInterval);
        }
    }

    IEnumerator LoadMainMenu()
    {
        startText.enabled = false;
        StartCoroutine(FadeToBlack());

        yield return new WaitForSeconds(fadeDuration + 0.5f);

        SceneManager.LoadScene(nextScene);
    }

    IEnumerator FadeToBlack()
    {
        float timer = 0f;
        Color startColor = blackFadeImage.color;
        Color endColor = new Color(0f, 0f, 0f, 1f);

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;
            blackFadeImage.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }

        blackFadeImage.color = endColor;
    }

}
