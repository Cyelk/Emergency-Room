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
        
        // Κρύψε το EHR Panel
        if (ehrPanel != null)
            ehrPanel.SetActive(false);
        
        // Εμφάνισε το Debrief
        debriefPanel.SetActive(true);
        
        // Ενημέρωσε τα δεδομένα
        UpdateScore();
        UpdateDecisionPath();
        UpdateDocumentation();
        
        Debug.Log("📋 Debrief άνοιξε");
    }
    
    void UpdateScore()
    {
        if (scoreText != null && ScenarioEngine.Instance != null)
        {
            int score = ScenarioEngine.Instance.currentScore;
            scoreText.text = "Σκορ: " + score + " / 100";
            
            // Χρώμα ανάλογα με το σκορ
            if (score >= 80) scoreText.color = Color.green;
            else if (score >= 50) scoreText.color = Color.yellow;
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
        if (documentationText == null || GameLogger.Instance == null) return;
        
        var entries = GameLogger.Instance.GetLogEntries();
        
        string doc = "📝 Τεκμηρίωση:\n";
        
        // Έλεγξε αν έγιναν submit
        bool assessmentSubmitted = entries.Any(e => e.eventType == "EHR_SUBMIT" && e.details.Contains("Επιτυχής"));
        bool gate1Passed = entries.Any(e => e.eventType == "GATE_PASSED" && e.details.Contains("n4"));
        bool gate2Passed = entries.Any(e => e.eventType == "GATE_PASSED" && e.details.Contains("n7"));
        
        doc += gate1Passed ? "  ✅ Assessment Form (Gate 1)\n" : "  ❌ Assessment Form (Gate 1)\n";
        doc += gate2Passed ? "  ✅ Communication Log (Gate 2)\n" : "  ❌ Communication Log (Gate 2)\n";
        
        // Λάθη
        var errors = entries.Where(e => 
            e.eventType == "TIMEOUT" || 
            e.eventType == "GATE_BLOCKED" ||
            (e.eventType == "OPTION_SELECTED" && e.details.Contains("-"))).ToList();
        
        if (errors.Count > 0)
        {
            doc += "\n⚠️ Λάθη:\n";
            foreach (var error in errors)
            {
                doc += "  • " + error.details + "\n";
            }
        }
        else
        {
            doc += "\n✅ Δεν υπάρχουν λάθη!\n";
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
        
        // Επαναφορά όλων
        if (ScenarioEngine.Instance != null)
            ScenarioEngine.Instance.currentScore = 0;
        
        if (GameLogger.Instance != null)
            GameLogger.Instance.ClearLog();
        
        if (VitalsData.Instance != null)
            VitalsData.Instance.ApplyVitalsUpdate(88, 110, 135, 85, 22);
        
        if (EHRManager.Instance != null)
            EHRManager.Instance.ResetFields();
        
        // Κρύψε το debrief
        if (debriefPanel != null)
            debriefPanel.SetActive(false);
        
        // Ξανάνοιξε το EHR
        if (ehrPanel != null)
            ehrPanel.SetActive(true);
        
        // Ξεκίνα το σενάριο από την αρχή
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