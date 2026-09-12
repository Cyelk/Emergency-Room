using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 150f;
    public float mouseSensitivity = 2f;
    public float gravity = -9.81f;
    
    private CharacterController controller;
    private Transform cameraTransform;
    private float xRotation = 0f;
    private Vector3 velocity;
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
        cameraTransform = GetComponentInChildren<Camera>().transform;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void Update()
    {
        // === ΠΕΡΙΣΤΡΟΦΗ ΜΕ ΠΟΝΤΙΚΙ (ΝΕΟ INPUT SYSTEM) ===
        float mouseX = Mouse.current.delta.x.ReadValue() * mouseSensitivity * 0.1f;
        float mouseY = Mouse.current.delta.y.ReadValue() * mouseSensitivity * 0.1f;
        
        transform.Rotate(Vector3.up * mouseX);
        
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        
        // === ΚΙΝΗΣΗ ΜΕ WASD (ΝΕΟ INPUT SYSTEM) ===
        float x = 0f;
        float z = 0f;
        
        var keyboard = Keyboard.current;
        if (keyboard != null)
        {
            if (keyboard.wKey.isPressed) z += 1f;
            if (keyboard.sKey.isPressed) z -= 1f;
            if (keyboard.aKey.isPressed) x -= 1f;
            if (keyboard.dKey.isPressed) x += 1f;
        }
        
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * moveSpeed * Time.deltaTime);
        
        // === ΒΑΡΥΤΗΤΑ ===
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        
        // === ΞΕΚΛΕΙΔΩΜΑ ΚΕΡΣΟΡΑ ΜΕ ESC ===
        if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        
        // === ΞΑΝΑΚΛΕΙΔΩΜΑ ΜΕ ΚΛΙΚ ===
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame 
            && Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}