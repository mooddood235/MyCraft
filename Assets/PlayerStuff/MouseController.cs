using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    [SerializeField]
    private Transform cameraTransform;
    [SerializeField]
    private float sensitivity;

    private float mouseX;
    private float mouseY;

    private void Start()
    {
        LockAndHideCursor();
    }

    private void Update()
    {
        GetMouseInput();
        Look();
    }

    private void GetMouseInput()
    {
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
    }
    private void Look()
    {
        transform.Rotate(Vector3.up * mouseX * sensitivity * Time.deltaTime);
        cameraTransform.Rotate(Vector3.right * -mouseY * sensitivity * Time.deltaTime);
    }
    private void LockAndHideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
