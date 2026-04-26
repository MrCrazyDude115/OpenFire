using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth instance;

    [Header("Health")]
    [SerializeField] private int maxLives = 3;

    [Header("Damage Flash")]
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField] private Image damageFlashImage;
    [SerializeField] private float flashDuration = 0.15f;
    [SerializeField] private float shakeForce = 0.5f;
    [SerializeField] private Color flashColor = new Color(1f, 0f, 0f, 0.4f);

    private bool isDead = false;
    private bool isProtected = false;

    private int currentLives;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        currentLives = maxLives;
        UIHealth.instance.UpdateLivesDisplay();

        if (damageFlashImage != null)
        {
            damageFlashImage.color = new Color(1f, 0f, 0f, 0f);
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead == false && !isProtected)
        {
            currentLives -= damageAmount;
            UIHealth.instance.UpdateLivesDisplay();

            Debug.Log("HIT! Lives left: " + currentLives);

            if (damageFlashImage != null)
            {
                StartCoroutine(FlashRed());
                SFXManager.instance.PlaySFXClip(GameFlowController.instance.damageSFX, transform, 3f);
            }

            if (impulseSource != null)
            {
                impulseSource.GenerateImpulse(shakeForce);
            }

            if (currentLives <= 0)
            {
                Die();
            }
        }
    }

    private IEnumerator FlashRed()
    {
        damageFlashImage.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        damageFlashImage.color = new Color(1f, 0f, 0f, 0f);
    }

    private void Die()
    {
        isDead = true;
        GameFlowController.instance.StartGameOverSequence();
        //Debug.Log("Player died.");
    }

    public int GetCurrentLives()
    {
        return currentLives;
    }

    public bool GetProtectedState()
    {
        return isProtected;
    }

    public void SetProtectedState(bool protectedState)
    {
        isProtected = protectedState;
    }


    public bool GetDeadState()
    {
        return isDead;
    }
}