using UnityEngine;
using UnityEngine.InputSystem;

public class HelpManager : MonoBehaviour
{
    public static HelpManager Instance;
    
    [Header("UI References")]
    public GameObject helpPanel;
    
    private bool isHelpOpen = false;
    
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    void Start()
    {
        if (helpPanel != null)
            helpPanel.SetActive(false);
    }
    
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.f1Key.wasPressedThisFrame)
        {
            ToggleHelp();
        }
    }
    
    public void ToggleHelp()
    {
        if (isHelpOpen)
            CloseHelp();
        else
            OpenHelp();
    }
    
    public void OpenHelp()
    {
        if (helpPanel != null)
        {
            helpPanel.SetActive(true);
            isHelpOpen = true;
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            Debug.Log("❓ Help άνοιξε");
        }
    }
    
    public void CloseHelp()
    {
        if (helpPanel != null)
        {
            helpPanel.SetActive(false);
            isHelpOpen = false;
            
            EHRManager ehr = FindObjectOfType<EHRManager>();
            if (ehr != null && ehr.IsEHRPanelOpen())
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            
            Debug.Log("❓ Help έκλεισε");
        }
    }
    
    public bool IsHelpOpen()
    {
        return isHelpOpen;
    }
}