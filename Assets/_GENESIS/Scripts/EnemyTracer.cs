using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Audio;

public class EnemyTracer : MonoBehaviour
{
    [SerializeField] private bool isDangerShot = false;

    [Header("Impulse FX")]
    [SerializeField] private float shakeForce = 0.1f;

    [Header("SFX")]
    [SerializeField] private AudioResource impactCoverSFX;
    [SerializeField] private AudioResource missSFX;
    [SerializeField, Range(0f, 1f)] public float sfxVolume = 1f;

    private Transform target;
    private Vector3 targetPosition;
    private float moveSpeed;
    private bool useTransformTarget;

    private CinemachineImpulseSource impulseSource;

    public void Init(Transform newTarget, float newMoveSpeed)
    {
        target = newTarget;
        moveSpeed = newMoveSpeed;
        useTransformTarget = true;
    }

    public void InitPosition(Vector3 newTargetPosition, float newMoveSpeed)
    {
        targetPosition = newTargetPosition;
        moveSpeed = newMoveSpeed;
        useTransformTarget = false;
    }

    private void Awake()
    {
        impulseSource = GameObject.FindWithTag("Player").GetComponent<CinemachineImpulseSource>();
    }

    private void Update()
    {
        if (useTransformTarget)
        {
            if (target != null)
            {
                transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
                transform.LookAt(target);

                if (Vector3.Distance(transform.position, target.position) < 0.1f)
                {
                    if (isDangerShot)
                    {
                        if (PlayerCoverController.instance != null && PlayerHealth.instance != null)
                        {
                            if (PlayerCoverController.instance.GetPeekingState()) 
                            {
                                PlayerHealth.instance.TakeDamage(1);
                            }
                        }
                    }

                    Destroy(gameObject);
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            transform.LookAt(targetPosition);

            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                if (!PlayerCoverController.instance.GetPeekingState())
                {
                    SFXManager.instance.PlaySFXClip(impactCoverSFX, transform, sfxVolume);
                    impulseSource.GenerateImpulse(shakeForce);
                }
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}