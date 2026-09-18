using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float mouseSensitivity = 2f;
    public float gravity = -9.81f;
    
    public bool canMove = true;
    public bool canLook = true;
    
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
        EHRManager ehr = FindObjectOfType<EHRManager>();
        bool ehrOpen = ehr != null && ehr.IsEHRPanelOpen();
        
        bool helpOpen = false;
        if (HelpManager.Instance != null)
            helpOpen = HelpManager.Instance.IsHelpOpen();
        
        bool selectionOpen = false;
        if (ScenarioSelectionManager.Instance != null)
            selectionOpen = ScenarioSelectionManager.Instance.IsPanelOpen();
        
        if (ehrOpen || helpOpen || selectionOpen)
        {
            if (controller.isGrounded && velocity.y < 0)
                velocity.y = -2f;
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
            return;
        }
        
        if (canLook)
        {
            float mouseX = Mouse.current.delta.x.ReadValue() * mouseSensitivity * 0.1f;
            float mouseY = Mouse.current.delta.y.ReadValue() * mouseSensitivity * 0.1f;
            
            transform.Rotate(Vector3.up * mouseX);
            
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
        
        if (canMove)
        {
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
        }
        
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        
        var kb = Keyboard.current;
        if (kb != null && kb.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        
        if (!ehrOpen && !helpOpen && !selectionOpen && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame 
            && Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}