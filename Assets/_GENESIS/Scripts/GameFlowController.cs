using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//This is the last script to be built for this protoype. While doing so, I learned when it's best to use: "gameObject.SetActive(false)" OR ".enabled = false" for UI text and elements.
//For the sake of time, I will not go back and update my other UI elements to use "gameObject.SetActive(false)" instead of ".enabled = false" even if I know it would be the best case.
//I'll leave that for a clean-up after submission. This will explain the inconsistency.

public class GameFlowController : MonoBehaviour
{
    public static GameFlowController instance;

    [Header("UI")]
    [SerializeField] private GameObject gameplayHUD;
    [SerializeField] private GameObject otherHUD;
    [SerializeField] private Image blackFadeImage;
    [SerializeField] private Image topPanel;
    [SerializeField] private TextMeshProUGUI topText;
    [SerializeField] private TextMeshProUGUI middleText;

    [Header("Scenes")]
    [SerializeField] private string mainMenuScene;
    [SerializeField] private string nextScene;

    [Header("Timing")]
    [SerializeField] private float winDelayBeforeText = 1.5f;
    [SerializeField] private float winTextDuration = 6f;
    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private float gameOverTextDuration = 5f;

    [Header("Events")]
    [SerializeField] private UnityEvent onLevelStart;

    [Header("SFX")]
    [SerializeField] private AudioResource levelMusic;
    [SerializeField] public AudioResource damageSFX;
    [SerializeField] public AudioResource winSFX;
    [SerializeField] public AudioResource waitSFX;
    [SerializeField] public AudioResource announcerGameOverSFX;
    [SerializeField] public AudioResource announcerReloadSFX;
    [SerializeField] public AudioResource announcerActionSFX;
    [SerializeField, Range(0f, 1f)] public float sfxVolume = 1f;
    [SerializeField, Range(0f, 1f)] public float musicVolume = 1f;

    private bool sequenceStarted = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        //topPanel.gameObject.SetActive(false);
        //topText.gameObject.SetActive(false);
        //middleText.gameObject.SetActive(false);

        onLevelStart.Invoke();
    }

    public void StartLevelMusic()
    {
        if (levelMusic != null)
        {
            SFXManager.instance.PlayMusicClip(levelMusic, musicVolume, 2f);
        }
    }

    public void StartWinSequence()
    {
        if (sequenceStarted)
        {
            return;
        }

        sequenceStarted = true;
        StartCoroutine(WinRoutine());
    }

    public void StartGameOverSequence()
    {
        if (sequenceStarted)
        {
            return;
        }

        sequenceStarted = true;
        StartCoroutine(GameOverRoutine());
    }

    private IEnumerator WinRoutine()
    {
        PlayerShooter.instance.enabled = false;
        PlayerCoverController.instance.enabled = false;

        PlayerCoverController.instance.SetPeekingState(true);
        PlayerHealth.instance.SetProtectedState(true);

        yield return new WaitForSeconds(winDelayBeforeText);

        SFXManager.instance.StopMusicClip(0f);

        gameplayHUD.SetActive(false);
        topPanel.gameObject.SetActive(true);
        topText.gameObject.SetActive(true);

        topText.text = "LEVEL CLEARED";

        SFXManager.instance.PlaySFXClip(winSFX, transform, sfxVolume);

        yield return new WaitForSeconds(winTextDuration);

        yield return StartCoroutine(FadeToBlack());

        if (nextScene != null)
        {
            SceneManager.LoadScene(nextScene);
        }
    }

    private IEnumerator GameOverRoutine()
    {
        PlayerShooter.instance.enabled = false;
        PlayerCoverController.instance.enabled = false;

        SFXManager.instance.StopMusicClip(fadeDuration);

        yield return StartCoroutine(FadeToBlack());

        gameplayHUD.SetActive(false);
        middleText.gameObject.SetActive(true);

        StartCoroutine(FadeInText(middleText, 2f));

        SFXManager.instance.PlaySFXClip(announcerGameOverSFX, transform, sfxVolume);

        middleText.text = "GAME OVER";

        yield return new WaitForSeconds(gameOverTextDuration);

        StartCoroutine(FadeOutText(middleText, 2f));

        yield return new WaitForSeconds(2f);

        if (mainMenuScene != null)
        {
            SceneManager.LoadScene(mainMenuScene);
        }
    }

    private IEnumerator FadeInText(TextMeshProUGUI text, float duration)
    {
        float timer = 0f;

        Color startColor = text.color;
        startColor.a = 0f;

        Color endColor = text.color;
        endColor.a = 1f;

        text.color = startColor;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = timer / duration;
            text.color = Color.Lerp(startColor, endColor, t);

            yield return null;
        }

        text.color = endColor;
    }

    private IEnumerator FadeOutText(TextMeshProUGUI text, float duration)
    {
        float timer = 0f;

        Color startColor = text.color;
        Color endColor = startColor;
        endColor.a = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            text.color = Color.Lerp(startColor, endColor, timer / duration);
            yield return null;
        }

        text.color = endColor;
    }

    private IEnumerator FadeToBlack()
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