using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    [SerializeField] private Transform moveTarget;

    public Transform GetSpawnTransform()
    {
        return transform;
    }

    public Transform GetMoveTarget()
    {
        return moveTarget;
    }
}