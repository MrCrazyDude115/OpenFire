using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UICrosshair : MonoBehaviour
{
    public static UICrosshair instance;

    [Header("UI")]
    [SerializeField] private Image crosshair;
    [SerializeField] private Image muzzleFlash;

    [Header("Timing")]
    [SerializeField] private float muzzleFlashTime = 0.05f;

    private RectTransform rectTransform;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        Cursor.visible = false;
        muzzleFlash.enabled = false;
    }

    private void Update()
    {
        rectTransform.position = Input.mousePosition;
    }

    private IEnumerator MuzzleFlashRoutine()
    {
        muzzleFlash.enabled = true;
        yield return new WaitForSeconds(muzzleFlashTime);
        muzzleFlash.enabled = false;
    }

    public void ShowMuzzleFlash()
    {
        StartCoroutine(MuzzleFlashRoutine());
    }
}