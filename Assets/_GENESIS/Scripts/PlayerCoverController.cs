using UnityEngine;
using Unity.Cinemachine;

public class PlayerCoverController : MonoBehaviour
{
    public static PlayerCoverController instance;

    [Header("Cameras")]
    [SerializeField] private CinemachineCamera inCoverCamera;
    [SerializeField] private CinemachineCamera peekCamera;

    [Header("Camera Priority Settings")]
    [SerializeField] private int activePriority = 20;
    [SerializeField] private int inactivePriority = 10;

    [Header("Player State")]
    [SerializeField] private bool startsInCover = true;

    public bool isPeeking;

    //private bool isProtected;
    private PlayerShooter shooter;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        if (startsInCover)
        {
            SetPeekingState(false);
        }
        else
        {
            SetPeekingState(true);
        }

        shooter = GetComponent<PlayerShooter>();
    }

    private void Update()
    {
        HandleCoverInput();
    }

    private void HandleCoverInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            SetPeekingState(true);
        }

        if (Input.GetMouseButtonUp(1))
        {
            SetPeekingState(false);

            if (shooter.currentAmmo < shooter.maxAmmo)
            {
                shooter.Reload();
            }
        }
    }

    public void SetAreaCameras(CinemachineCamera newInCoverCamera, CinemachineCamera newPeekCamera)
    {
        inCoverCamera = newInCoverCamera;
        peekCamera = newPeekCamera;
    }

    /*I discovered that I can keep variables private even if I might use them elsewhere by then "getting/returning" them with a function for use in another script.
    That way they maintain they private setting AND can still be retrieved. I'll be making use of this more in the future.
    But whatever I set as private/public already previously will remain for now. Not enough time left to clean up.*/

    public Transform GetPeekCameraTransform()
    {
        return peekCamera.transform;
    }

    public bool GetPeekingState()
    {
        return isPeeking;
    }

    public void SetPeekingState(bool peekState)
    {
        isPeeking = peekState;

        if (isPeeking)
        {
            peekCamera.Priority = activePriority;
            inCoverCamera.Priority = inactivePriority;
        }
        else
        {
            inCoverCamera.Priority = activePriority;
            peekCamera.Priority = inactivePriority;
        }
    }

    //public bool GetProtectedState()
    //{
    //    return isProtected;
    //}

    //public void SetProtectedState(bool protectedState)
    //{
    //    isProtected = protectedState;
    //}
}