using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class UIAnnouncerText : MonoBehaviour
{
    public static UIAnnouncerText instance;

    [SerializeField] private TextMeshProUGUI subtitleText;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Image middlePanel;

    private bool isReloadText = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        subtitleText.enabled = false;
        titleText.enabled = false;
        middlePanel.enabled = false;
    }

    IEnumerator HideTextAfterDelay(TextMeshProUGUI textObject, float time)
    {
        yield return new WaitForSeconds(time);
        textObject.enabled = false;
    }

    public void ShowWaitText()
    {
        if (!subtitleText.enabled)
        {
            subtitleText.enabled = true;
        }

        SetText(subtitleText, "WAIT", Color.red);
        SFXManager.instance.PlaySFXClip(GameFlowController.instance.waitSFX, transform, PlayerShooter.instance.sfxVolume);
    }

    public void ShowOpenFireText()
    {
        if (!subtitleText.enabled)
        {
            subtitleText.enabled = true;
        }

        SetText(subtitleText, "OPEN FIRE", Color.green);
        SFXManager.instance.PlaySFXClip(GameFlowController.instance.announcerActionSFX, transform, PlayerShooter.instance.sfxVolume);
        StartCoroutine(HideTextAfterDelay(subtitleText, 1f));
    }

    public void ShowReloadText()
    {
        if (!titleText.enabled)
        {
            titleText.enabled = true;
        }

        if (!middlePanel.enabled)
        {
            middlePanel.enabled = true;
        }

        SetText(titleText, "RELOAD", Color.yellow);
        isReloadText = true;
    }

    public void HideReloadText()
    {
        titleText.enabled = false;
        middlePanel.enabled = false;
        isReloadText = false;
    }

    private void SetText(TextMeshProUGUI textObject, string text, Color colour)
    {
        textObject.text = text;
        textObject.color = colour;
    }

    public bool GetReloadTextStatus()
    {
        return isReloadText;
    }
}
