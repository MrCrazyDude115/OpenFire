using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Unity.Cinemachine;

public class AreaTransitionController : MonoBehaviour
{
    [Header("Transition Cameras")]
    [SerializeField] private CinemachineCamera[] transitionCameras;
    [SerializeField] private float[] stepDurations;

    [Header("Final Area Cameras")]
    [SerializeField] private CinemachineCamera finalPeekCamera;
    [SerializeField] private CinemachineCamera finalCoverCamera;

    [Header("Timing")]
    [SerializeField] private float forcePeekDelay = 0.5f;
    //[SerializeField] private float finalCameraDelay = 0.15f;
    [SerializeField] private float initialProtectionPeriod = 0.75f;

    [Header("Events")]
    [SerializeField] private UnityEvent onTransitionStart;
    [SerializeField] private UnityEvent onTransitionFinished;

    public void StartTransition()
    {
        StartCoroutine(TransitionRoutine());
    }

    private IEnumerator TransitionRoutine()
    {
        onTransitionStart.Invoke();

        PlayerHealth.instance.SetProtectedState(true);
        DisablePlayerControls();
        ForcePeekView();
        UIAnnouncerText.instance.ShowWaitText();

        if (UIAnnouncerText.instance.GetReloadTextStatus())
        {
            UIAnnouncerText.instance.HideReloadText();
        }

        yield return new WaitForSeconds(forcePeekDelay);

        for (int i = 0; i < transitionCameras.Length; i++)
        {
            SetOnlyActiveCamera(transitionCameras[i]);

            if (i < stepDurations.Length)
            {
                yield return new WaitForSeconds(stepDurations[i]);
            }
            else
            {
                yield return new WaitForSeconds(1f);
            }
        }

        SetOnlyActiveCamera(finalCoverCamera);
        PlayerCoverController.instance.SetAreaCameras(finalCoverCamera, finalPeekCamera);

        PlayerCoverController.instance.SetPeekingState(false);
        PlayerShooter.instance.Reload();
        EnablePlayerControls();
        UIAnnouncerText.instance.ShowOpenFireText();

        onTransitionFinished.Invoke();

        yield return new WaitForSeconds(initialProtectionPeriod);

        PlayerHealth.instance.SetProtectedState(false);
    }

    private void ForcePeekView()
    {
        PlayerCoverController.instance.SetPeekingState(true);
    }

    private void DisablePlayerControls()
    {
        PlayerShooter.instance.enabled = false;
        PlayerCoverController.instance.enabled = false;
    }

    private void EnablePlayerControls()
    {
        PlayerCoverController.instance.enabled = true;
        PlayerShooter.instance.enabled = true;
    }

    private void SetOnlyActiveCamera(CinemachineCamera activeCamera)
    {
        for (int i = 0; i < transitionCameras.Length; i++)
        {
            if (transitionCameras[i] != null)
            {
                transitionCameras[i].Priority = 0;
            }
        }

        if (finalPeekCamera != null)
        {
            finalPeekCamera.Priority = 0;
        }

        if (finalCoverCamera != null)
        {
            finalCoverCamera.Priority = 0;
        }

        if (activeCamera != null)
        {
            activeCamera.Priority = 100;
        }
    }
}