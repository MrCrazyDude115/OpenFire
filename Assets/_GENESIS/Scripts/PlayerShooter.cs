using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerShooter : MonoBehaviour
{
    public static PlayerShooter instance;

    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private PlayerCoverController coverController;

    [Header("Shooting Settings")]
    [SerializeField] private int damagePerShot = 1;
    [SerializeField] public int maxAmmo = 6;
    [SerializeField] private float maxShootDistance = 100f;
    [SerializeField] private LayerMask shootableLayers;

    [Header("Impulse FX")]
    [SerializeField] private float shakeForce = 0.1f;

    [Header("SFX")]
    [SerializeField] private AudioResource shootSFX;
    [SerializeField] private AudioResource dryshootSFX;
    [SerializeField] public AudioResource reloadSFX;
    [SerializeField, Range(0f, 1f)] public float sfxVolume = 1f;

    public int currentAmmo;
    private bool announcedReload = false;
    private CinemachineImpulseSource impulseSource;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void Start()
    {
        currentAmmo = maxAmmo;
        UIAmmo.instance.UpdateAmmoDisplay();
    }

    private void Update()
    {
        if (coverController.isPeeking)
        {
            if (Input.GetMouseButtonDown(0))
            {
                FireShot();
            }
        }
    }

    private void FireShot()
    {
        if (currentAmmo > 0)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            currentAmmo--;
            UIAmmo.instance.UpdateAmmoDisplay();
            UICrosshair.instance.ShowMuzzleFlash();
            impulseSource.GenerateImpulse(shakeForce);

            if (shootSFX != null && SFXManager.instance != null)
            {
                SFXManager.instance.PlaySFXClip(shootSFX, transform, sfxVolume);
            }

            if (Physics.Raycast(ray, out hit, maxShootDistance, shootableLayers))
            {
                EnemyHealth enemyHealth = hit.collider.GetComponentInParent<EnemyHealth>();

                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(damagePerShot);
                }

                ExplosiveObject explosiveObject = hit.collider.GetComponentInParent<ExplosiveObject>();

                if (explosiveObject != null)
                {
                    explosiveObject.TakeDamage(damagePerShot);
                }
            }
        }
        else
        {
            Debug.Log("NEED TO RELOAD!");

            if (!UIAnnouncerText.instance.GetReloadTextStatus())
            {
                UIAnnouncerText.instance.ShowReloadText();
            }
                
            if (dryshootSFX != null)
            {
                SFXManager.instance.PlaySFXClip(dryshootSFX, transform, sfxVolume);
            }

            if (!announcedReload)
            {
                if (GameFlowController.instance.announcerReloadSFX != null)
                {
                    SFXManager.instance.PlaySFXClip(GameFlowController.instance.announcerReloadSFX, transform, sfxVolume);
                    announcedReload = true;
                }
            }
        }
    }

    public void Reload()
    {
        currentAmmo = maxAmmo;
        SFXManager.instance.PlaySFXClip(reloadSFX, transform, sfxVolume);
        UIAmmo.instance.UpdateAmmoDisplay();

        announcedReload = false;

        if (UIAnnouncerText.instance.GetReloadTextStatus())
        {
            UIAnnouncerText.instance.HideReloadText();
        }
        //Debug.Log("Reloading");
    }

    public int GetCurrentAmmo()
    {
        return currentAmmo;
    }
}