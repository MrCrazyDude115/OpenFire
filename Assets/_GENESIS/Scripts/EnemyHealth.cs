using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 3;

    [Header("Hit Flash")]
    [SerializeField] private SkinnedMeshRenderer enemyRenderer;
    [SerializeField] private Color hitColor = Color.red;
    [SerializeField] private float flashTime = 0.1f;

    [Header("Death")]
    [SerializeField] private float destroyDelay = 2f;

    [Header("SFX")]
    [SerializeField] private AudioResource hitSFX;
    [SerializeField] private AudioResource deathSFX;
    [SerializeField, Range(0f, 1f)] private float sfxVolume = 1f;

    private Animator animator;
    private ArenaController arenaController;
    private int currentHealth;
    private Color originalColor;
    private bool isDead = false;
    private int deathAnimationCount = 4;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        currentHealth = maxHealth;

        if (enemyRenderer != null)
        {
            originalColor = enemyRenderer.material.GetColor("_BaseColor");
        }

        if (animator != null)
        {
            animator.SetBool("isDead", false);
        }
    }

    private IEnumerator FlashOnHit()
    {
        enemyRenderer.material.SetColor("_BaseColor", hitColor);
        yield return new WaitForSeconds(flashTime);
        enemyRenderer.material.SetColor("_BaseColor", originalColor);
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }

    public void SetArenaController(ArenaController newArenaController)
    {
        arenaController = newArenaController;
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

        Debug.Log(gameObject.name + " took " + damageAmount + " damage. Health left: " + currentHealth);

        if (enemyRenderer != null)
        {
            StartCoroutine(FlashOnHit());
        }

        if (hitSFX != null && SFXManager.instance != null)
        {
            SFXManager.instance.PlaySFXClip(hitSFX, transform, sfxVolume);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead == false)
        {
            isDead = true;

            //Debug.Log(gameObject.name + " died.");

            EnemyCoverShooter enemyCoverShooter = GetComponentInParent<EnemyCoverShooter>();
            CapsuleCollider enemyCollider = GetComponentInParent<CapsuleCollider>();

            if (enemyCoverShooter != null)
            {
                enemyCoverShooter.Die();
            }

            if (enemyCollider != null)
            {
                enemyCollider.enabled = false;
            }

            if (animator != null)
            {
                animator.SetBool("isDead", true);
                int randomDeathIndex = Random.Range(0, deathAnimationCount);
                animator.SetInteger("DeathIndex", randomDeathIndex);
                animator.SetTrigger("Die");
            }

            if (hitSFX != null)
            {
                SFXManager.instance.PlaySFXClip(hitSFX, transform, sfxVolume);
            }

            if (deathSFX != null)
            {
                SFXManager.instance.PlaySFXClip(deathSFX, transform, sfxVolume);
            }

            if (arenaController != null)
            {
                arenaController.RegisterEnemyDeath();
            }

            StartCoroutine(DestroyAfterDelay());
        }
    }
}