using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class EHRManager : MonoBehaviour
{
    public static EHRManager Instance;
    
    [Header("Main Panel")]
    public GameObject ehrPanel;
    public Button SubmitButton;
    public TextMeshProUGUI statusText;
    
    [Header("Assessment Fields")]
    public TMP_InputField observationInput;
    public TMP_InputField fiO2Input;
    
    [Header("Communication Fields")]
    public TMP_InputField recipientInput;
    public TMP_InputField outcomeInput;
    public TMP_InputField reasonInput;
    
    [Header("Field Groups")]
    public GameObject assessmentFields;
    public GameObject communicationFields;
    
    [Header("Flags - Assessment")]
    public bool observationSubmitted = false;
    public bool fiO2Submitted = false;
    
    [Header("Flags - Communication")]
    public bool recipientSubmitted = false;
    public bool outcomeSubmitted = false;
    public bool reasonSubmitted = false;
    
    [Header("Submitted Values")]
    public string submittedObservation = "";
    public string submittedFiO2 = "";
    public string submittedRecipient = "";
    public string submittedOutcome = "";
    public string submittedReason = "";
    
    private bool isPanelOpen = false;
    private GameObject currentHotspot = null;
    
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    void Start()
    {
        if (ehrPanel != null)
            ehrPanel.SetActive(false);
        
        ShowAssessmentFields();
    }
    
    public bool IsEHRPanelOpen()
    {
        return isPanelOpen;
    }
    
    public void ToggleEHR(GameObject hotspotObject, string hotspotName)
    {
        if (ehrPanel.activeSelf && currentHotspot == hotspotObject)
        {
            CloseEHR();
            return;
        }
        
        OpenEHR(hotspotObject, hotspotName);
    }
    
    public void OpenEHR(GameObject hotspotObject, string hotspotName)
    {
        if (ehrPanel != null)
        {
            currentHotspot = hotspotObject;
            isPanelOpen = true;
            ehrPanel.SetActive(true);
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            if (EventSystem.current != null)
            {
                var inputModule = EventSystem.current.GetComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
                if (inputModule != null)
                {
                    inputModule.enabled = false;
                }
            }
            
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                player.canLook = false;
                player.canMove = false;
                player.enabled = false;
            }
            
            Debug.Log("📂 EHR άνοιξε από: " + hotspotName);
        }
    }
    
    public void CloseEHR()
    {
        if (ehrPanel != null)
        {
            currentHotspot = null;
            isPanelOpen = false;
            ehrPanel.SetActive(false);
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            if (EventSystem.current != null)
            {
                var inputModule = EventSystem.current.GetComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
                if (inputModule != null)
                {
                    inputModule.enabled = true;
                }
            }
            
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                player.canLook = true;
                player.canMove = true;
                player.enabled = true;
            }
            
            Debug.Log("📂 EHR έκλεισε");
        }
    }
    
    public void ShowAssessmentFields()
    {
        if (assessmentFields != null) assessmentFields.SetActive(true);
        if (communicationFields != null) communicationFields.SetActive(false);
    }
    
    public void ShowCommunicationFields()
    {
        if (assessmentFields != null) assessmentFields.SetActive(false);
        if (communicationFields != null) communicationFields.SetActive(true);
    }
    
    public void OnSubmit()
    {
        string observation = observationInput != null ? observationInput.text : "";
        string fiO2 = fiO2Input != null ? fiO2Input.text : "";
        string recipient = recipientInput != null ? recipientInput.text : "";
        string outcome = outcomeInput != null ? outcomeInput.text : "";
        string reason = reasonInput != null ? reasonInput.text : "";
        
        bool hasError = false;
        string missingFields = "";
        
        bool assessmentActive = assessmentFields != null && assessmentFields.activeSelf;
        bool communicationActive = communicationFields != null && communicationFields.activeSelf;
        
        if (assessmentActive)
        {
            if (string.IsNullOrEmpty(observation))
            {
                missingFields += "• Παρατήρηση\n";
                hasError = true;
            }
            else
            {
                observationSubmitted = true;
                submittedObservation = observation;
                
                if (ValidationManager.Instance != null)
                    ValidationManager.Instance.ValidateField("observation", observation);
            }
            
            if (string.IsNullOrEmpty(fiO2))
            {
                missingFields += "• FiO2\n";
                hasError = true;
            }
            else
            {
                fiO2Submitted = true;
                submittedFiO2 = fiO2;
                
                if (ValidationManager.Instance != null)
                    ValidationManager.Instance.ValidateField("fiO2_setting", fiO2);
            }
        }
        
        if (communicationActive)
        {
            if (string.IsNullOrEmpty(recipient))
            {
                missingFields += "• Παραλήπτης\n";
                hasError = true;
            }
            else
            {
                recipientSubmitted = true;
                submittedRecipient = recipient;
                
                if (ValidationManager.Instance != null)
                    ValidationManager.Instance.ValidateField("recipient", recipient);
            }
            
            if (string.IsNullOrEmpty(outcome))
            {
                missingFields += "• Αποτέλεσμα\n";
                hasError = true;
            }
            else
            {
                outcomeSubmitted = true;
                submittedOutcome = outcome;
                
                if (ValidationManager.Instance != null)
                    ValidationManager.Instance.ValidateField("outcome", outcome);
            }
            
            if (!string.IsNullOrEmpty(reason))
            {
                reasonSubmitted = true;
                submittedReason = reason;
                
                if (ValidationManager.Instance != null)
                    ValidationManager.Instance.ValidateField("reason", reason);
            }
        }
        
        if (!assessmentActive && !communicationActive)
        {
            return;
        }
        
        if (hasError)
        {
            Debug.LogWarning("⚠️ Λείπουν πεδία: " + missingFields);
            
            if (GameLogger.Instance != null)
                GameLogger.Instance.LogEvent("EHR_SUBMIT_FAILED", "Λείπουν: " + missingFields);
            
            if (ToastManager.Instance != null)
                ToastManager.Instance.ShowToastStyled("⚠️ Λείπουν πεδία!", "warning");
            
            return;
        }
        
        Debug.Log("✅ EHR Υποβολή");
        
        if (GameLogger.Instance != null)
            GameLogger.Instance.LogEvent("EHR_SUBMIT", "Επιτυχής υποβολή");
        
        if (ToastManager.Instance != null)
            ToastManager.Instance.ShowToastStyled("✅ Η τεκμηρίωση καταχωρήθηκε!", "success");
    }
    
    public bool IsFieldSubmitted(string fieldName)
    {
        switch (fieldName)
        {
            case "observation": return observationSubmitted;
            case "fiO2_setting": return fiO2Submitted;
            case "recipient": return recipientSubmitted;
            case "outcome": return outcomeSubmitted;
            case "reason": return reasonSubmitted;
            default: return false;
        }
    }
    
    public void ResetFields()
    {
        observationSubmitted = false;
        fiO2Submitted = false;
        recipientSubmitted = false;
        outcomeSubmitted = false;
        reasonSubmitted = false;
        
        submittedObservation = "";
        submittedFiO2 = "";
        submittedRecipient = "";
        submittedOutcome = "";
        submittedReason = "";
        
        if (observationInput != null) observationInput.text = "";
        if (fiO2Input != null) fiO2Input.text = "";
        if (recipientInput != null) recipientInput.text = "";
        if (outcomeInput != null) outcomeInput.text = "";
        if (reasonInput != null) reasonInput.text = "";
        
        if (ValidationManager.Instance != null)
            ValidationManager.Instance.ResetValidation();
    }

}