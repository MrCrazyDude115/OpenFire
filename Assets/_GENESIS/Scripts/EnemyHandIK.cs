using UnityEngine;

public class EnemyHandIK : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private EnemyCoverShooter enemyCoverShooter;
    [SerializeField, Range(0f, 1f)] private float leftHandIKWeight = 1f;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (enemyCoverShooter == null)
        {
            enemyCoverShooter = GetComponentInParent<EnemyCoverShooter>();
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (animator != null && enemyCoverShooter != null)
        {
            if (enemyCoverShooter.CanUseLeftHandIK())
            {
                Transform leftHandTarget = enemyCoverShooter.GetLeftHandIKTarget();

                if (leftHandTarget != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftHandIKWeight);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, leftHandIKWeight);

                    animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);
                    animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandTarget.rotation);
                }
                else
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0f);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0f);
                }
            }
            else
            {
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0f);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0f);
            }
        }
    }
}