using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class ExplosiveObject : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 3;

    [Header("Hit Flash")]
    [SerializeField] private Renderer meshRenderer;
    [SerializeField] private Color hitColor = Color.red;
    [SerializeField] private float flashTime = 0.1f;

    [Header("Explosion")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private LayerMask enemyLayers;

    [Header("SFX")]
    [SerializeField] private AudioResource explosionSFX;
    [SerializeField, Range(0f, 1f)] public float sfxVolume = 1f;

    private int currentHealth;
    private Color originalColor;
    private bool isDestroyed = false;

    private void Start()
    {
        currentHealth = maxHealth;

        if (meshRenderer != null)
        {
            originalColor = meshRenderer.material.GetColor("_BaseColor");
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDestroyed == false)
        {
            currentHealth -= damageAmount;

            if (meshRenderer != null)
            {
                StartCoroutine(FlashOnHit());
            }

            if (currentHealth <= 0)
            {
                Explode();
            }
        }
    }

    private IEnumerator FlashOnHit()
    {
        meshRenderer.material.SetColor("_BaseColor", hitColor);
        yield return new WaitForSeconds(flashTime);
        meshRenderer.material.SetColor("_BaseColor", originalColor);
    }

    private void Explode()
    {
        isDestroyed = true;

        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(explosion, 3f);
        }

        if (explosionSFX != null)
        {
            SFXManager.instance.PlaySFXClip(explosionSFX, transform, sfxVolume);
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius, enemyLayers);

        for (int i = 0; i < hitColliders.Length; i++)
        {
            EnemyHealth enemyHealth = hitColliders[i].GetComponentInParent<EnemyHealth>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(999);
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}