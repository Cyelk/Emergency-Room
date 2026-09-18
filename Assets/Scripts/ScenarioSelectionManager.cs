using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.IO;

public class ScenarioSelectionManager : MonoBehaviour
{
    public static ScenarioSelectionManager Instance;
    
    [Header("UI")]
    public GameObject selectionPanel;
    public Button openSelectionButton;
    public Button openSelectionButtonDebrief;
    public Button closeSelectionButton;
    public Button loadButton;
    public TMP_Dropdown scenarioDropdown;
    
    [Header("Panels")]
    public GameObject ehrPanel;
    public GameObject debriefPanel;
    
    private bool isPanelOpen = false;
    private List<string> scenarioFiles = new List<string>();
    
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    void Start()
    {
        if (selectionPanel != null)
            selectionPanel.SetActive(false);
        
        if (openSelectionButton != null)
            openSelectionButton.onClick.AddListener(OpenSelection);
        
        if (openSelectionButtonDebrief != null)
            openSelectionButtonDebrief.onClick.AddListener(OpenSelection);
        
        if (closeSelectionButton != null)
            closeSelectionButton.onClick.AddListener(CloseSelection);
        
        if (loadButton != null)
            loadButton.onClick.AddListener(OnLoadClicked);
        
        ScanForScenarios();
        SetupDropdown();
    }
    
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.f2Key.wasPressedThisFrame)
        {
            if (isPanelOpen) CloseSelection();
            else OpenSelection();
        }
    }
    
    void ScanForScenarios()
    {
        scenarioFiles.Clear();
        
        string path = Application.streamingAssetsPath;
        
        if (Directory.Exists(path))
        {
            string[] files = Directory.GetFiles(path, "*.json");
            
            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);
                scenarioFiles.Add(fileName);
                Debug.Log("📄 Βρέθηκε σενάριο: " + fileName);
            }
        }
    }
    
    void SetupDropdown()
    {
        if (scenarioDropdown == null) return;
        
        scenarioDropdown.ClearOptions();
        
        List<string> options = new List<string>();
        
        foreach (string file in scenarioFiles)
        {
            string title = GetScenarioTitle(file);
            options.Add(title);
        }
        
        scenarioDropdown.AddOptions(options);
    }
    
    string GetScenarioTitle(string fileName)
    {
        string path = Path.Combine(Application.streamingAssetsPath, fileName);
        
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            int titleStart = json.IndexOf("\"title\":");
            if (titleStart >= 0)
            {
                titleStart = json.IndexOf("\"", titleStart + 8) + 1;
                int titleEnd = json.IndexOf("\"", titleStart);
                if (titleEnd > titleStart)
                    return json.Substring(titleStart, titleEnd - titleStart);
            }
        }
        
        return fileName.Replace(".json", "");
    }
    
    public void OpenSelection()
    {
        Debug.Log("🔥 OpenSelection ΚΛΗΘΗΚΕ!");
        
        if (selectionPanel == null)
        {
            Debug.LogError("❌ selectionPanel είναι NULL!");
            return;
        }
        
        HidePanel(ehrPanel);
        HidePanel(debriefPanel);
        
        selectionPanel.SetActive(true);
        selectionPanel.transform.SetAsLastSibling();
        
        isPanelOpen = true;
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        Debug.Log("📋 Selection άνοιξε");
    }
    
    public void CloseSelection()
    {
        if (selectionPanel == null) return;
        
        selectionPanel.SetActive(false);
        isPanelOpen = false;
        
        ShowPanel(ehrPanel);
        ShowPanel(debriefPanel);
        
        Debug.Log("📋 Selection έκλεισε");
    }
    
    void HidePanel(GameObject panel)
    {
        if (panel == null) return;
        
        CanvasGroup cg = panel.GetComponent<CanvasGroup>();
        if (cg == null) cg = panel.AddComponent<CanvasGroup>();
        
        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }
    
    void ShowPanel(GameObject panel)
    {
        if (panel == null) return;
        
        CanvasGroup cg = panel.GetComponent<CanvasGroup>();
        if (cg == null) return;
        
        cg.alpha = 1f;
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }
    
    public void OnLoadClicked()
    {
        if (scenarioDropdown == null) return;
        
        int index = scenarioDropdown.value;
        if (index < 0 || index >= scenarioFiles.Count) return;
        
        string fileName = scenarioFiles[index];
        LoadScenario(fileName);
    }
    
    public void LoadScenario(string fileName)
    {
        Debug.Log("🔄 Φόρτωση σεναρίου: " + fileName);
        
        if (selectionPanel != null)
            selectionPanel.SetActive(false);
        isPanelOpen = false;
        
        if (debriefPanel != null)
            debriefPanel.SetActive(false);
        
        ResetGameState();
        
        if (ScenarioLoader.Instance != null)
            ScenarioLoader.Instance.LoadScenario(fileName);
        
        if (ScenarioEngine.Instance != null)
            ScenarioEngine.Instance.RestartScenario();
        
        if (GameLogger.Instance != null)
        {
            GameLogger.Instance.ClearLog();
            GameLogger.Instance.LogEvent("SESSION_START", "Έναρξη σεναρίου: " + fileName);
        }
        
        ShowPanel(ehrPanel);
        if (ehrPanel != null)
            ehrPanel.SetActive(true);
        
        Debug.Log("✅ Σενάριο φορτώθηκε: " + fileName);
    }
    
    void ResetGameState()
    {
        Debug.Log("🔄 Reset κατάστασης...");
        
        if (VitalsData.Instance != null)
            VitalsData.Instance.ApplyVitalsUpdate(88, 110, 135, 85, 22);
        
        if (EHRManager.Instance != null)
        {
            EHRManager.Instance.ResetFields();
            EHRManager.Instance.ShowAssessmentFields();
        }
        
        if (ValidationManager.Instance != null)
            ValidationManager.Instance.ResetValidation();
        
        if (HelpManager.Instance != null && HelpManager.Instance.IsHelpOpen())
            HelpManager.Instance.CloseHelp();
        
        GameObject debriefObj = GameObject.Find("DebriefPanel");
        if (debriefObj != null)
        {
            debriefObj.SetActive(false);
            Debug.Log("🔄 DebriefPanel έκλεισε");
        }
        
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.gameObject.SetActive(true);
            player.canLook = true;
            player.canMove = true;
            player.enabled = true;
        }
        
        Debug.Log("✅ Reset ολοκληρώθηκε");
    }
    
    public bool IsPanelOpen()
    {
        return isPanelOpen;
    }
}