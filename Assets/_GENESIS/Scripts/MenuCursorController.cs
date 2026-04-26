using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MenuCursor : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image muzzleFlash;

    [Header("Timing")]
    [SerializeField] private float muzzleFlashTime = 0.05f;

    [Header("SFX")]
    [SerializeField] private AudioResource shootSFX;
    [SerializeField, Range(0f, 1f)] private float sfxVolume = 0.25f;

    private RectTransform cursorTransform;

    private void Awake()
    {
        cursorTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        Cursor.visible = false;

        if (muzzleFlash != null)
        {
            muzzleFlash.enabled = false;
        }
    }

    private void Update()
    {
        cursorTransform.position = Input.mousePosition;

        if (Input.GetMouseButtonDown(0))
        {
            PlayClickEffect();
        }
    }

    public void PlayClickEffect()
    {
        if (shootSFX != null)
        {
            SFXManager.instance.PlaySFXClip(shootSFX, transform, sfxVolume);
        }

        if (muzzleFlash != null)
        {
            StartCoroutine(MuzzleFlashRoutine());
        }
    }

    private IEnumerator MuzzleFlashRoutine()
    {
        muzzleFlash.enabled = true;
        yield return new WaitForSeconds(muzzleFlashTime);
        muzzleFlash.enabled = false;
    }
}