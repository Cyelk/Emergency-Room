using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class EHRManager : MonoBehaviour
{
    public GameObject ehrPanel;
    public TMP_InputField observationInput;
    public Button submitButton;
    public TextMeshProUGUI statusText;
    
    // Αποθηκεύουμε ποιο hotspot είναι ανοιχτό
    private GameObject currentHotspot = null;
    
    void Start()
    {
        if (ehrPanel != null)
            ehrPanel.SetActive(false);
        
        if (submitButton != null)
            submitButton.onClick.AddListener(OnSubmit);
    }
    
    // Συνάρτηση για εναλλαγή (toggle) του EHR από ένα hotspot
    public void ToggleEHR(GameObject hotspotObject, string hotspotName)
    {
        // Αν το EHR είναι ήδη ανοιχτό από αυτό το hotspot -> ΚΛΕΙΣΕ
        if (ehrPanel.activeSelf && currentHotspot == hotspotObject)
        {
            CloseEHR();
            return;
        }
        
        // Αλλιώς ΑΝΟΙΞΕ το EHR
        OpenEHR(hotspotObject, hotspotName);
    }
    
    // Συνάρτηση για άνοιγμα του EHR
    public void OpenEHR(GameObject hotspotObject, string hotspotName)
    {
        if (ehrPanel != null)
        {
            currentHotspot = hotspotObject;
            ehrPanel.SetActive(true);
            Debug.Log("📂 EHR άνοιξε από: " + hotspotName);
            UpdateStatus("EHR άνοιξε από: " + hotspotName);
        }
    }
    
    // Συνάρτηση για κλείσιμο του EHR
    public void CloseEHR()
    {
        if (ehrPanel != null)
        {
            currentHotspot = null;
            ehrPanel.SetActive(false);
            Debug.Log("📂 EHR έκλεισε");
            UpdateStatus("EHR έκλεισε");
        }
    }
    
    public void OnSubmit()
    {
        string observation = observationInput != null ? observationInput.text : "";
        
        if (string.IsNullOrEmpty(observation))
        {
            Debug.LogWarning("⚠️ Το πεδίο παρατήρησης είναι άδειο!");
            UpdateStatus("⚠️ Συμπληρώστε την παρατήρηση!");
            return;
        }
        
        Debug.Log("✅ EHR Υποβολή: " + observation);
        UpdateStatus("✅ Η παρατήρηση καταχωρήθηκε: " + observation);
        CloseEHR();
    }
    
    void UpdateStatus(string message)
    {
        if (statusText != null)
            statusText.text = message;
    }
}