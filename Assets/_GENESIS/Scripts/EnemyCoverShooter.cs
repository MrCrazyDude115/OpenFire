using System.Collections;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static Unity.Collections.Unicode;

//Useful sources (NOT INCLUDING OFFICIAL DOCUMENTATION)
//https://youtu.be/AmGSEH7QcDg?si=FJeprKe7buedhyeL
//https://youtu.be/aHFSDcEQuzQ?si=EEJ_zxDX57-2mRqw
//https://youtu.be/tveRasxUabo?si=UPpZ6NKm9r-GcELf
//https://youtu.be/YJB1QnEmlTs?si=CFZqbTugI0sE_db4
//https://youtu.be/uWI3JEBRMiA?si=8xV7j5Yhil5So-zb
//https://youtu.be/Q16KIxtomeo?si=nLVJ-52LqeMs1KfI
//https://youtu.be/ydhpu4j7fIk?si=mkTzz0Gjhn7R6eaC
//https://youtu.be/_lREXfAMUcE?si=Xe9m0htyKwBbmhU-
//https://youtu.be/fB0P0C_3sPU?si=K9sQnSFstN2bmWDc
//https://youtube.com/shorts/lNN2LRBemiM?si=GCB8y98oxZDBvfhO

public class EnemyCoverShooter : MonoBehaviour
{
    public enum EnemyExposureType
    {
        CoverPopUp,
        CrouchedOpen,
        StandingOpen
    }

    public enum EnemyWeaponType
    {
        Pistol,
        AssaultRifle,
        Machinegun
    }

    public enum EnemyFireType
    {
        Semi,
        Auto
    }

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform rightHandWeaponSocket;
    [SerializeField] private GameObject pistolPrefab;
    [SerializeField] private GameObject assaultRiflePrefab;
    [SerializeField] private GameObject machinegunPrefab;
    [SerializeField] private GameObject dangerShotWarningPrefab;

    [Header("Enemy Type")]
    [SerializeField] private EnemyExposureType exposureType = EnemyExposureType.CoverPopUp;
    [SerializeField] private EnemyWeaponType weaponType = EnemyWeaponType.Pistol;
    [SerializeField] private EnemyFireType fireType = EnemyFireType.Semi;

    [Header("Left Hand IK")]
    [SerializeField] private bool useLeftHandIK = true;

    [Header("Cover Pop-Up Timing")]
    [SerializeField] private float minHiddenTime = 1f;
    [SerializeField] private float maxHiddenTime = 3f;
    [SerializeField] private float minPeekTime = 2f;
    [SerializeField] private float maxPeekTime = 4f;

    [Header("Semi Shooting")]
    [SerializeField] private float minShotDelay = 0.8f;
    [SerializeField] private float maxShotDelay = 1.5f;

    [Header("Automatic Shooting")]
    [SerializeField] private int shotsPerBurst = 10;
    [SerializeField] private float fireRate = 5f;
    [SerializeField] private float burstCooldown = 2f;

    [Header("Danger Shot")]
    [SerializeField] private float dangerShotChance = 5f;
    [SerializeField] private float dangerShotSpawnDelay = 0.5f;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private bool shootWhileMoving = false;
    [SerializeField] private bool facePlayerWhileMoving = false;

    [Header("Roll")]
    [SerializeField] private float rollDistance = 2f;
    [SerializeField] private float rollDuration = 0.5f;

    private bool facePlayerCamera = true;
    private float rotationSpeed = 8f;

    private bool isDead = false;
    private bool isMovingToPosition = false;
    private bool doSpawnRoll = false;
    private bool moveAfterRoll = false;
    private bool isRolling = false;

    private Transform playerCameraTarget;
    private Transform moveTarget;
    private GameObject currentWeapon;
    private EnemyWeapon currentWeaponScript;
    private NavMeshAgent agent;

    //This was moved into Awake rather than Start because the arena spawner configures enemies immediately after they are instantiated. If the agent is only assigned in Start, other functions can run
    //too early and not do what they're supposed to. I'm still experimenting with the best use cases for both Awake and Start, but some online sources suggest just doing everything in Awake.
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        agent.updateRotation = false;
    }

    private void Start()
    {
        playerCameraTarget = PlayerCoverController.instance.GetPeekCameraTransform();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    private void Update()
    {
        if (isRolling)
        {
            return;
        }

        if (isMovingToPosition)
        {
            MoveToTargetPosition();

            if (facePlayerWhileMoving)
            {
                RotateTowardPlayerCamera();
            }
            else
            {
                RotateTowardMoveDirection();
            }
        }
        else if (facePlayerCamera)
        {
            RotateTowardPlayerCamera();
        }

        UpdateMovementAnimation();
    }

    //Using these to fill the lists I learned about from here: https://www.youtube.com/watch?v=Q16KIxtomeo & here: https://www.youtube.com/watch?v=uWI3JEBRMiA
    //I needed to find a way to fill the inspector values for the enemies AFTER they've been spawned. These videos + Google searches and Unity forum discussions + consultation from others recommended this
    public void SetSpawnSettings(
        EnemyExposureType newExposureType,
        EnemyWeaponType newWeaponType,
        EnemyFireType newFireType,
        bool newFacePlayerCamera,
        bool newDoSpawnRoll,
        bool newMoveAfterRoll)
    {
        exposureType = newExposureType;
        weaponType = newWeaponType;
        fireType = newFireType;
        facePlayerCamera = newFacePlayerCamera;
        doSpawnRoll = newDoSpawnRoll;
        moveAfterRoll = newMoveAfterRoll;
    }

    public void SetMoveTarget(Transform newMoveTarget, bool canShootWhileMoving, bool shouldFacePlayerWhileMoving)
    {
        moveTarget = newMoveTarget;
        shootWhileMoving = canShootWhileMoving;
        facePlayerWhileMoving = shouldFacePlayerWhileMoving;

        if (moveTarget != null && doSpawnRoll == false)
        {
            isMovingToPosition = true;
            DisableWeaponLayers();
            agent.isStopped = false;
            agent.SetDestination(moveTarget.position);
        }
    }

    public void InitEnemy()
    {
        SpawnStartingWeaponMesh();

        if (doSpawnRoll)
        {
            DisableWeaponLayers();
            StartCoroutine(SpawnRollRoutine());
        }
        else
        {
            SetStartingPose();

            if (!isMovingToPosition)
            {
                SetStartingWeaponAnimations();
            }

            StartCoroutine(EnemyRoutine());
        }
    }

    private IEnumerator SpawnRollRoutine()
    {
        isRolling = true;
        DisableWeaponLayers();
        Roll();

        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + transform.forward * rollDistance;

        float timer = 0f;

        while (timer < rollDuration)
        {
            timer += Time.deltaTime;
            float t = timer / rollDuration;
            t = Mathf.SmoothStep(0f, 1f, t);

            transform.position = Vector3.Lerp(startPosition, endPosition, t);
            yield return null;
        }

        transform.position = endPosition;
        isRolling = false;

        if (moveAfterRoll && moveTarget != null)
        {
            isMovingToPosition = true;
            DisableWeaponLayers();
            agent.Warp(transform.position);
            agent.isStopped = false;
            agent.SetDestination(moveTarget.position);
        }
        else
        {
            SetStartingPose();
            SnapTowardPlayerCamera();
            EnableCurrentWeaponLayer();
            StartCoroutine(EnemyRoutine());
        }
    }

    private IEnumerator EnemyRoutine()
    {
        while (!isDead)
        {
            if (exposureType == EnemyExposureType.CoverPopUp)
            {
                yield return StartCoroutine(CoverPopUpRoutine());
            }
            else if (exposureType == EnemyExposureType.CrouchedOpen)
            {
                yield return StartCoroutine(CrouchedOpenRoutine());
            }
            else
            {
                yield return StartCoroutine(StandingOpenRoutine());
            }
        }
    }

    private IEnumerator CoverPopUpRoutine()
    {
        SetCrouchState(true);

        float hiddenTime = Random.Range(minHiddenTime, maxHiddenTime);
        yield return new WaitForSeconds(hiddenTime);

        SetCrouchState(false);

        float peekTime = Random.Range(minPeekTime, maxPeekTime);
        float timer = 0f;

        while (timer < peekTime && !isDead)
        {
            Shoot();

            float shotDelay = Random.Range(minShotDelay, maxShotDelay);
            yield return new WaitForSeconds(shotDelay);

            timer += shotDelay;
        }

        SetCrouchState(true);
    }

    private IEnumerator CrouchedOpenRoutine()
    {
        SetCrouchState(true);

        while (!isDead && exposureType == EnemyExposureType.CrouchedOpen)
        {
            yield return StartCoroutine(FireRoutine());
        }
    }

    private IEnumerator StandingOpenRoutine()
    {
        SetCrouchState(false);

        while (!isDead && exposureType == EnemyExposureType.StandingOpen)
        {
            yield return StartCoroutine(FireRoutine());
        }
    }

    private IEnumerator FireRoutine()
    {
        if (fireType == EnemyFireType.Semi)
        {
            Shoot();

            float shotDelay = Random.Range(minShotDelay, maxShotDelay);
            yield return new WaitForSeconds(shotDelay);
        }
        else
        {
            float timeBetweenShots = 1f / fireRate;

            for (int i = 0; i < shotsPerBurst; i++)
            {
                Shoot();
                yield return new WaitForSeconds(timeBetweenShots);
            }

            yield return new WaitForSeconds(burstCooldown);
        }
    }

    private IEnumerator SpawnDangerShotAfterDelay()
    {
        yield return new WaitForSeconds(dangerShotSpawnDelay);

        Transform shotTarget = GetShotTarget();
        currentWeaponScript.SpawnDangerTracerPosition(shotTarget);
    }

    public Transform GetLeftHandIKTarget()
    {
        if (currentWeaponScript != null)
        {
            return currentWeaponScript.GetLeftHandIKTarget();
        }

        return null;
    }

    public bool CanUseLeftHandIK()
    {
        if (isDead) return false;
        if (!useLeftHandIK) return false;
        if (currentWeaponScript == null) return false;
        if (currentWeaponScript.GetLeftHandIKTarget() == null) return false;

        return true;
    }

    private void MoveToTargetPosition()
    {
        if (moveTarget == null)
        {
            return;
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            isMovingToPosition = false;
            agent.isStopped = true;

            SetStartingPose();
            EnableCurrentWeaponLayer();
            StartCoroutine(EnemyRoutine());
        }
    }

    private void RotateTowardMoveDirection()
    {
        Vector3 lookDirection = agent.velocity;
        lookDirection.y = 0f;

        if (lookDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void RotateTowardPlayerCamera()
    {
        playerCameraTarget = PlayerCoverController.instance.GetPeekCameraTransform();

        Vector3 lookDirection = playerCameraTarget.position - transform.position;
        lookDirection.y = 0f;

        if (lookDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void SnapTowardPlayerCamera()
    {
        playerCameraTarget = PlayerCoverController.instance.GetPeekCameraTransform();

        Vector3 lookDirection = playerCameraTarget.position - transform.position;
        lookDirection.y = 0f;

        if (lookDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }
    }

    /*For the sake of full academic integrity and transparency, AI was used to help me figure out how to actually get this next function working correctly as the animator is very tricky and 
     * I couldn't get it to do what I wanted without a "real-time guide" involved.*/
    private void UpdateMovementAnimation()
    {
        Vector3 moveDirection = agent.desiredVelocity;
        moveDirection.y = 0f;

        float currentSpeed = agent.velocity.magnitude;

        if (currentSpeed > 0.05f)
        {
            animator.SetBool("isMoving", true);

            moveDirection.Normalize();

            Vector3 referenceForward;
            Vector3 referenceRight;

            if (isMovingToPosition && facePlayerWhileMoving)
            {
                Vector3 toPlayer = PlayerCoverController.instance.GetPeekCameraTransform().position - transform.position;
                toPlayer.y = 0f;
                toPlayer.Normalize();

                referenceForward = toPlayer;
                referenceRight = Vector3.Cross(Vector3.up, referenceForward);
            }
            else
            {
                referenceForward = transform.forward;
                referenceRight = transform.right;
            }

            float moveX = Vector3.Dot(moveDirection, referenceRight);
            float moveY = Vector3.Dot(moveDirection, referenceForward);

            animator.SetFloat("MoveX", moveX, 0.1f, Time.deltaTime);
            animator.SetFloat("MoveY", moveY, 0.1f, Time.deltaTime);
            animator.SetFloat("MoveSpeed", 1f, 0.1f, Time.deltaTime);
        }
        else
        {
            animator.SetBool("isMoving", false);
            animator.SetFloat("MoveX", 0f, 0.1f, Time.deltaTime);
            animator.SetFloat("MoveY", 0f, 0.1f, Time.deltaTime);
            animator.SetFloat("MoveSpeed", 0f, 0.1f, Time.deltaTime);
        }
    }

    private Transform GetShotTarget()
    {
        return PlayerCoverController.instance.GetPeekCameraTransform();
    }

    private Vector3 GetNormalShotTargetPosition()
    {
        Transform cam = PlayerCoverController.instance.GetPeekCameraTransform();

        Vector3 randomOffset =
            cam.right * Random.Range(-currentWeaponScript.tracerSpread, currentWeaponScript.tracerSpread) +
            cam.up * Random.Range(-currentWeaponScript.tracerSpread, currentWeaponScript.tracerSpread);

        return cam.position + randomOffset;
    }


    public void SpawnDangerShotWarning()
    {
        if (currentWeaponScript.muzzlePoint != null && dangerShotWarningPrefab != null)
        {
            GameObject flash = Instantiate(dangerShotWarningPrefab, currentWeaponScript.muzzlePoint.position, currentWeaponScript.muzzlePoint.rotation);
            Destroy(flash, 0.35f);
        }
    }

    private void SetStartingPose()
    {
        if (exposureType == EnemyExposureType.CoverPopUp)
        {
            SetCrouchState(true);
        }
        else if (exposureType == EnemyExposureType.CrouchedOpen)
        {
            SetCrouchState(true);
        }
        else
        {
            SetCrouchState(false);
        }
    }

    private void SetStartingWeaponAnimations()
    {
        DisableWeaponLayers();

        if (weaponType == EnemyWeaponType.Pistol)
        {
            animator.SetLayerWeight(1, 1);
        }
        else if (weaponType == EnemyWeaponType.AssaultRifle)
        {
            animator.SetLayerWeight(2, 1);
        }
        else
        {
            animator.SetLayerWeight(3, 1);
        }
    }

    private void DisableWeaponLayers()
    {
        animator.SetLayerWeight(1, 0);
        animator.SetLayerWeight(2, 0);
        animator.SetLayerWeight(3, 0);
    }

    private void EnableCurrentWeaponLayer()
    {
        SetStartingWeaponAnimations();
    }

    private void SetCrouchState(bool crouching)
    {
        animator.SetBool("isCrouching", crouching);
        animator.SetBool("isPeeking", !crouching);
    }

    private void SpawnStartingWeaponMesh()
    {
        GameObject weaponToSpawn = null;

        if (weaponType == EnemyWeaponType.Pistol)
        {
            weaponToSpawn = pistolPrefab;
        }
        else if (weaponType == EnemyWeaponType.AssaultRifle)
        {
            weaponToSpawn = assaultRiflePrefab;
        }
        else if (weaponType == EnemyWeaponType.Machinegun)
        {
            weaponToSpawn = machinegunPrefab;
        }

        if (weaponToSpawn != null && rightHandWeaponSocket != null)
        {
            currentWeapon = Instantiate(weaponToSpawn, rightHandWeaponSocket);
            currentWeapon.transform.localPosition = Vector3.zero;
            currentWeapon.transform.localRotation = Quaternion.identity;
            currentWeaponScript = currentWeapon.GetComponent<EnemyWeapon>();
        }
    }

    private void Shoot()
    {
        if (isRolling) return;
        if (isMovingToPosition && !shootWhileMoving) return;
        if (PlayerHealth.instance.GetDeadState()) return;

        animator.SetTrigger("Shoot");
        currentWeaponScript.SpawnMuzzleFlash();
        currentWeaponScript.PlayShotSFX();

        float randomValue = Random.Range(0f, 100f);

        if (randomValue <= dangerShotChance && !PlayerHealth.instance.GetProtectedState())
        {
            SpawnDangerShotWarning();
            StartCoroutine(SpawnDangerShotAfterDelay());
        }
        else
        {
            Vector3 normalTarget = GetNormalShotTargetPosition();
            currentWeaponScript.SpawnTracerPosition(normalTarget);
        }
    }

    public void Roll()
    {
        animator.SetTrigger("Roll");
    }

    public void Die()
    {
        isDead = true;
        StopAllCoroutines();
        agent.isStopped = true;
    }
}