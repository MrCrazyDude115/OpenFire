using UnityEngine;
using UnityEngine.Audio;

public class EnemyWeapon : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public Transform muzzlePoint;
    [SerializeField] private Transform leftHandIKTarget;

    [Header("FX")]
    [SerializeField] private GameObject tracerPrefab;
    [SerializeField] private GameObject dangerTracerPrefab;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private float tracerSpeed = 20f;
    [SerializeField] private float dangerTracerSpeed = 20f;
    [SerializeField] public float tracerSpread = 0.3f;

    [Header("SFX")]
    [SerializeField] private AudioResource shotSFX;
    [SerializeField, Range(0f, 1f)] public float sfxVolume = 0.5f;

    public Transform GetMuzzlePoint()
    {
        return muzzlePoint;
    }

    public Transform GetLeftHandIKTarget()
    {
        return leftHandIKTarget;
    }

    public void PlayShotSFX()
    {
        SFXManager.instance.PlaySFXClip(shotSFX, muzzlePoint.transform, sfxVolume);
    }

    public void SpawnMuzzleFlash()
    {
        if (muzzlePoint != null && muzzleFlash != null)
        {
            GameObject flash = Instantiate(muzzleFlash, muzzlePoint.position, muzzlePoint.rotation);
            Destroy(flash, 1f);
        }
    }

    public void SpawnTracerPosition(Vector3 targetPosition)
    {
        if (muzzlePoint != null && tracerPrefab != null)
        {
            GameObject tracer = Instantiate(tracerPrefab, muzzlePoint.position, Quaternion.identity);
            EnemyTracer enemyTracer = tracer.GetComponent<EnemyTracer>();

            if (enemyTracer != null)
            {
                enemyTracer.InitPosition(targetPosition, tracerSpeed);
            }
        }
    }

    public void SpawnDangerTracerPosition(Transform target)
    {
        if (muzzlePoint != null && dangerTracerPrefab != null && target != null)
        {
            GameObject tracer = Instantiate(dangerTracerPrefab, muzzlePoint.position, Quaternion.identity);
            EnemyTracer enemyTracer = tracer.GetComponent<EnemyTracer>();

            if (enemyTracer != null)
            {
                enemyTracer.Init(target, dangerTracerSpeed);
            }
        }
    }
}