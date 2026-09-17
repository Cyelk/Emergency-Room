using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class DebriefManager : MonoBehaviour
{
    public static DebriefManager Instance;
    
    [Header("Debrief Panel")]
    public GameObject debriefPanel;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI decisionPathText;
    public TextMeshProUGUI documentationText;
    
    [Header("Buttons")]
    public Button exportButton;
    public Button restartButton;
    public Button closeButton;
    
    [Header("EHR Panels to Hide")]
    public GameObject ehrPanel;
    
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    void Start()
    {
        if (debriefPanel != null)
            debriefPanel.SetActive(false);
        
        if (exportButton != null)
            exportButton.onClick.AddListener(OnExport);
        
        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestart);
        
        if (closeButton != null)
            closeButton.onClick.AddListener(OnClose);
    }
    
    public void ShowDebrief()
    {
        if (debriefPanel == null) return;
        
        if (ehrPanel != null)
            ehrPanel.SetActive(false);
        
        debriefPanel.SetActive(true);
        
        UpdateScore();
        UpdateDecisionPath();
        UpdateDocumentation();
        
        Debug.Log("📋 Debrief άνοιξε");
    }
    
    void UpdateScore()
    {
        if (scoreText != null && ScenarioEngine.Instance != null)
        {
            int scenarioScore = ScenarioEngine.Instance.currentScore;
            int validationScore = 0;
            
            if (ValidationManager.Instance != null)
                validationScore = ValidationManager.Instance.validationScore;
            
            int totalScore = scenarioScore + validationScore;
            
            scoreText.text = "Σκορ: " + totalScore + " / 100\n" +
                            "(Σενάριο: " + scenarioScore + " | Τεκμηρίωση: " + validationScore + ")";
            
            if (totalScore >= 80) scoreText.color = Color.green;
            else if (totalScore >= 50) scoreText.color = Color.yellow;
            else scoreText.color = Color.red;
        }
    }
    
    void UpdateDecisionPath()
    {
        if (decisionPathText == null || GameLogger.Instance == null) return;
        
        var entries = GameLogger.Instance.GetLogEntries();
        var nodeEntries = entries.Where(e => e.eventType == "NODE_ENTER").ToList();
        
        string path = "📖 Διαδρομή Αποφάσεων:\n";
        foreach (var entry in nodeEntries)
        {
            path += "  → " + entry.details.Replace("Node: ", "") + "\n";
        }
        
        decisionPathText.text = path;
    }
    
    void UpdateDocumentation()
    {
        if (documentationText == null) return;
        
        string doc = "📝 Τεκμηρίωση:\n";
        
        EHRManager ehr = EHRManager.Instance;
        if (ehr != null)
        {
            if (!string.IsNullOrEmpty(ehr.submittedObservation))
                doc += "\n  Παρατήρηση: \"" + ehr.submittedObservation + "\"\n";
            if (!string.IsNullOrEmpty(ehr.submittedFiO2))
                doc += "  FiO2: \"" + ehr.submittedFiO2 + "\"\n";
            if (!string.IsNullOrEmpty(ehr.submittedRecipient))
                doc += "  Παραλήπτης: \"" + ehr.submittedRecipient + "\"\n";
            if (!string.IsNullOrEmpty(ehr.submittedOutcome))
                doc += "  Αποτέλεσμα: \"" + ehr.submittedOutcome + "\"\n";
            if (!string.IsNullOrEmpty(ehr.submittedReason))
                doc += "  Αιτία: \"" + ehr.submittedReason + "\"\n";
        }
        
        if (ValidationManager.Instance != null)
        {
            doc += "\n📊 Έλεγχος Τεκμηρίωσης:\n";
            foreach (var log in ValidationManager.Instance.validationLog)
            {
                doc += "  " + log + "\n";
            }
            doc += "\n  Σύνολο: " + ValidationManager.Instance.validationScore + " πόντοι\n";
        }
        
        documentationText.text = doc;
    }
    
    void OnExport()
    {
        if (GameLogger.Instance != null)
        {
            GameLogger.Instance.ExportToJSON();
            
            if (ToastManager.Instance != null)
                ToastManager.Instance.ShowToastStyled("📥 Το log εξήχθη!", "success");
        }
    }
    
    void OnRestart()
    {
        Debug.Log("🔄 Επανεκκίνηση...");
        
        if (ScenarioEngine.Instance != null)
            ScenarioEngine.Instance.currentScore = 0;
        
        if (GameLogger.Instance != null)
            GameLogger.Instance.ClearLog();
        
        if (VitalsData.Instance != null)
            VitalsData.Instance.ApplyVitalsUpdate(88, 110, 135, 85, 22);
        
        if (EHRManager.Instance != null)
            EHRManager.Instance.ResetFields();
        
        if (ValidationManager.Instance != null)
            ValidationManager.Instance.ResetValidation();
        
        if (debriefPanel != null)
            debriefPanel.SetActive(false);
        
        if (ehrPanel != null)
            ehrPanel.SetActive(true);
        
        if (ScenarioEngine.Instance != null)
            ScenarioEngine.Instance.GoToNode("n1_start");
    }
    
    void OnClose()
    {
        if (debriefPanel != null)
            debriefPanel.SetActive(false);
        
        if (ehrPanel != null)
            ehrPanel.SetActive(true);
    }
}