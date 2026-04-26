using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private RectTransform startButton;
    [SerializeField] private RectTransform quitButton;

    [Header("Slide Settings")]
    [SerializeField] private float offScreenDistance = 1200f;
    [SerializeField] private float buttonSlideDuration = 1f;

    [Header("UI Fade")]
    [SerializeField] private Image blackFadeImage;

    [Header("Scenes")]
    [SerializeField] private string gameSceneName;

    [Header("Timing")]
    [SerializeField] private float fadeDuration = 1.5f;

    [Header("Music")]
    [SerializeField] private AudioResource menuMusic;
    [SerializeField, Range(0f, 1f)] private float musicVolume = 1f;

    private Vector2 startButtonVisiblePosition;
    private Vector2 quitButtonVisiblePosition;

    private Vector2 startButtonHiddenPosition;
    private Vector2 quitButtonHiddenPosition;

    private bool optionSelected = false;

    private void Start()
    {
        startButtonVisiblePosition = startButton.anchoredPosition;
        quitButtonVisiblePosition = quitButton.anchoredPosition;

        startButtonHiddenPosition = startButtonVisiblePosition + new Vector2(offScreenDistance, 0f);
        quitButtonHiddenPosition = quitButtonVisiblePosition + new Vector2(-offScreenDistance, 0f);

        startButton.anchoredPosition = startButtonHiddenPosition;
        quitButton.anchoredPosition = quitButtonHiddenPosition;

        blackFadeImage.color = new Color(0f, 0f, 0f, 0f);

        if (menuMusic != null)
        {
            SFXManager.instance.PlayMusicClip(menuMusic, musicVolume, 1.5f);
        }

        StartCoroutine(SlideButtonsIn());
    }

    public void StartGame()
    {
        if (optionSelected == false)
        {
            optionSelected = true;
            StartCoroutine(StartGameRoutine());
        }
    }

    public void QuitGame()
    {
        if (optionSelected == false)
        {
            optionSelected = true;
            StartCoroutine(QuitGameRoutine());
        }
    }

    private IEnumerator StartGameRoutine()
    {
        StartCoroutine(SlideButtonsOut());
        yield return StartCoroutine(FadeToBlack());

        SceneManager.LoadScene(gameSceneName);
    }

    private IEnumerator QuitGameRoutine()
    {
        StartCoroutine(SlideButtonsOut());
        yield return StartCoroutine(FadeToBlack());

        Application.Quit();
    }

    private IEnumerator SlideButtonsIn()
    {
        float timer = 0f;

        while (timer < buttonSlideDuration)
        {
            timer += Time.deltaTime;
            float t = timer / buttonSlideDuration;
            t = Mathf.SmoothStep(0f, 1f, t);

            startButton.anchoredPosition = Vector2.Lerp(startButtonHiddenPosition, startButtonVisiblePosition, t);
            quitButton.anchoredPosition = Vector2.Lerp(quitButtonHiddenPosition, quitButtonVisiblePosition, t);

            yield return null;
        }

        startButton.anchoredPosition = startButtonVisiblePosition;
        quitButton.anchoredPosition = quitButtonVisiblePosition;
    }

    private IEnumerator SlideButtonsOut()
    {
        float timer = 0f;

        Vector2 startButtonCurrentPosition = startButton.anchoredPosition;
        Vector2 quitButtonCurrentPosition = quitButton.anchoredPosition;

        while (timer < buttonSlideDuration)
        {
            timer += Time.deltaTime;
            float t = timer / buttonSlideDuration;
            t = Mathf.SmoothStep(0f, 1f, t);

            startButton.anchoredPosition = Vector2.Lerp(startButtonCurrentPosition, startButtonHiddenPosition, t);
            quitButton.anchoredPosition = Vector2.Lerp(quitButtonCurrentPosition, quitButtonHiddenPosition, t);

            yield return null;
        }

        startButton.anchoredPosition = startButtonHiddenPosition;
        quitButton.anchoredPosition = quitButtonHiddenPosition;
    }

    private IEnumerator FadeToBlack()
    {
        float timer = 0f;

        Color startColor = blackFadeImage.color;
        Color endColor = new Color(0f, 0f, 0f, 1f);

        SFXManager.instance.StopMusicClip(fadeDuration);

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