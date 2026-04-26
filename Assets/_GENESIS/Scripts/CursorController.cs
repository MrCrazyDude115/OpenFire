using UnityEngine;

public class CursorController : MonoBehaviour
{
    private void Start()
    {
        //Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }
}