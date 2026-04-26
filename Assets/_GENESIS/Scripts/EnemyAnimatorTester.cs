using UnityEngine;

public class EnemyAnimatorTester : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeedValue = 0.5f;
    [SerializeField] private float runSpeedValue = 1f;

    private bool isPeeking = false;
    private bool isCrouching = true;

    private void Start()
    {
        animator.SetBool("isCrouching", true);
        animator.SetBool("isPeeking", false);
        animator.SetBool("isMoving", false);
        animator.SetFloat("MoveX", 0f);
        animator.SetFloat("MoveY", 0f);
        animator.SetFloat("MoveSpeed", 0f);
    }

    private void Update()
    {
        HandlePeekTest();
        HandleShootTest();
        HandleMovementTest();
    }

    private void HandlePeekTest()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            isPeeking = !isPeeking;
            isCrouching = !isPeeking;

            animator.SetBool("isPeeking", isPeeking);
            animator.SetBool("isCrouching", isCrouching);
        }
    }

    private void HandleShootTest()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("Shoot");
        }
    }

    private void HandleMovementTest()
    {
        float targetMoveX = 0f;
        float targetMoveY = 0f;
        float targetMoveSpeed = 0f;
        bool isMoving = false;

        if (isPeeking)
        {
            if (Input.GetKey(KeyCode.W))
            {
                targetMoveY = 1f;
                isMoving = true;
            }

            if (Input.GetKey(KeyCode.S))
            {
                targetMoveY = -1f;
                isMoving = true;
            }

            if (Input.GetKey(KeyCode.A))
            {
                targetMoveX = -1f;
                isMoving = true;
            }

            if (Input.GetKey(KeyCode.D))
            {
                targetMoveX = 1f;
                isMoving = true;
            }

            if (isMoving)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    targetMoveSpeed = runSpeedValue;
                }
                else
                {
                    targetMoveSpeed = walkSpeedValue;
                }
            }
        }

        animator.SetBool("isMoving", isMoving);

        animator.SetFloat("MoveX", targetMoveX, 0.1f, Time.deltaTime);
        animator.SetFloat("MoveY", targetMoveY, 0.1f, Time.deltaTime);
        animator.SetFloat("MoveSpeed", targetMoveSpeed, 0.1f, Time.deltaTime);
    }
}