using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ScenarioEngine : MonoBehaviour
{
    public static ScenarioEngine Instance;
    
    [Header("UI References")]
    public TextMeshProUGUI nodeText;
    public GameObject optionsContainer;
    public Button continueButton;
    public GameObject optionButtonPrefab;
    
    [Header("Timer")]
    public TextMeshProUGUI timerText;
    
    [Header("State")]
    public int currentScore = 0;
    public Dictionary<string, bool> flags = new Dictionary<string, bool>();
    
    private NodeData currentNode;
    private string currentNodeId = "n1_start";
    private Coroutine timerCoroutine;
    private bool timeoutActive = false;
    
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    void Start()
    {
        StartCoroutine(StartScenario());
    }
    
    IEnumerator StartScenario()
    {
        Debug.Log("StartScenario ξεκίνησε!");
        
        while (ScenarioLoader.Instance == null || ScenarioLoader.Instance.CurrentScenario == null)
        {
            yield return null;
        }
        
        Debug.Log("ScenarioEngine ξεκίνησε! currentNodeId: " + currentNodeId);
        GoToNode(currentNodeId);
    }
    
    public void RestartScenario()
    {
        Debug.Log("RestartScenario ΚΛΗΘΗΚΕ!");
        StopAllCoroutines();
        currentScore = 0;
        flags.Clear();
        currentNodeId = "n1_start";
        Debug.Log("Ξεκινάω StartScenario με currentNodeId: " + currentNodeId);
        StartCoroutine(StartScenario());
    }
    
    public void GoToNode(string nodeId)
    {
        Debug.Log("GoToNode: " + nodeId);
        
        if (ScenarioLoader.Instance == null || ScenarioLoader.Instance.CurrentScenario == null)
        {
            Debug.LogError("ScenarioLoader ή CurrentScenario είναι NULL!");
            return;
        }
        
        NodeData node = ScenarioLoader.Instance.CurrentScenario.rules.nodes.Find(n => n.id == nodeId);
        
        if (node == null)
        {
            Debug.LogError("Δεν βρέθηκε node: " + nodeId);
            Debug.LogError("Διαθέσιμα nodes: " + string.Join(", ", ScenarioLoader.Instance.CurrentScenario.rules.nodes.Select(n => n.id)));
            return;
        }
        
        StopTimer();
        
        currentNode = node;
        currentNodeId = nodeId;
        
        Debug.Log("Node: " + node.id + " (" + node.type + ")");
        
        if (GameLogger.Instance != null)
            GameLogger.Instance.LogEvent("NODE_ENTER", "Node: " + node.id + " (" + node.type + ")");
        
        if (nodeText != null)
            nodeText.text = node.text;
        
        EHRManager ehr = FindObjectOfType<EHRManager>();
        if (ehr != null)
        {
            if (node.id == "n7_gate_documentation_2")
                ehr.ShowCommunicationFields();
            else if (node.id == "n1_start" || node.id == "n2_initial_decision" || 
                     node.id == "n3_intervention" || node.id == "n4_gate_documentation_1")
                ehr.ShowAssessmentFields();
        }
        
        ClearOptions();
        
        switch (node.type)
        {
            case "message":
                ShowContinueButton();
                break;
            case "decision":
                ShowOptions(node.options);
                if (node.timeout != null && node.timeout.seconds > 0)
                    StartTimer(node);
                break;
            case "gate":
                ShowGate(node);
                break;
            case "end":
                ShowEnd(node);
                break;
        }
    }
    
    void ShowContinueButton()
    {
        if (continueButton != null)
        {
            continueButton.gameObject.SetActive(true);
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(() => {
                GoToNode(currentNode.next_node_id);
            });
        }
        if (optionsContainer != null) optionsContainer.SetActive(false);
    }
    
    void ShowOptions(List<OptionData> options)
    {
        if (continueButton != null) continueButton.gameObject.SetActive(false);
        if (optionsContainer != null) optionsContainer.SetActive(true);
        
        foreach (var option in options)
        {
            Button btn = CreateOptionButton(option.label);
            if (btn != null)
                btn.onClick.AddListener(() => { SelectOption(option); });
        }
    }
    
    void ShowGate(NodeData node)
    {
        if (continueButton != null)
        {
            continueButton.gameObject.SetActive(true);
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(() => { CheckGate(node); });
        }
        if (optionsContainer != null) optionsContainer.SetActive(false);
    }
    
    void CheckGate(NodeData node)
    {
        EHRManager ehr = FindObjectOfType<EHRManager>();
        if (ehr == null) return;
        
        bool allFieldsOk = true;
        string missingFields = "";
        
        foreach (var reqForm in node.gate_requirements.required_forms)
        {
            foreach (var field in reqForm.fields)
            {
                if (!ehr.IsFieldSubmitted(field))
                {
                    allFieldsOk = false;
                    
                    string friendlyName = field;
                    switch (field)
                    {
                        case "observation": friendlyName = "Παρατήρηση"; break;
                        case "fiO2_setting": friendlyName = "FiO2"; break;
                        case "recipient": friendlyName = "Παραλήπτης"; break;
                        case "outcome": friendlyName = "Αποτέλεσμα"; break;
                        case "reason": friendlyName = "Αιτία"; break;
                    }
                    
                    missingFields += "• " + friendlyName + "\n";
                }
            }
        }
        
        if (!allFieldsOk)
        {
            if (GameLogger.Instance != null)
                GameLogger.Instance.LogEvent("GATE_BLOCKED", "Αποτυχία gate: " + node.id);
            
            if (nodeText != null)
                nodeText.text = "Ελλιπής τεκμηρίωση!\n\n" +
                               "Πρέπει να συμπληρώσετε τα εξής πεδία:\n" +
                               missingFields + "\n" + node.feedback_blocked;
            
            if (ToastManager.Instance != null)
                ToastManager.Instance.ShowToastStyled("Ελλιπής τεκμηρίωση!", "warning");
            
            return;
        }
        
        if (GameLogger.Instance != null)
            GameLogger.Instance.LogEvent("GATE_PASSED", "Επιτυχία gate: " + node.id);
        
        if (nodeText != null)
            nodeText.text = node.feedback_success;
        
        if (ToastManager.Instance != null)
            ToastManager.Instance.ShowToastStyled(node.feedback_success, "success");
        
        if (node.effects_on_pass != null)
        {
            if (node.effects_on_pass.score_delta != 0)
                currentScore += node.effects_on_pass.score_delta;
        }
        
        StartCoroutine(GoToNextAfterDelay(node.next_node_id, 1.5f));
    }
    
    IEnumerator GoToNextAfterDelay(string nodeId, float delay)
    {
        yield return new WaitForSeconds(delay);
        GoToNode(nodeId);
    }
    
    void ShowEnd(NodeData node)
    {
        Debug.Log("ShowEnd ΚΛΗΘΗΚΕ! Node: " + node.id);
        
        if (continueButton != null) continueButton.gameObject.SetActive(false);
        if (optionsContainer != null) optionsContainer.SetActive(false);
        
        Debug.Log("Τέλος σεναρίου! Σκορ: " + currentScore);
        
        if (GameLogger.Instance != null)
        {
            GameLogger.Instance.LogEvent("SESSION_END", "Τέλος σεναρίου - Σκορ: " + currentScore);
            GameLogger.Instance.ExportToJSON();
        }
        
        Debug.Log("Ψάχνω DebriefManager...");
        DebriefManager debrief = FindObjectOfType<DebriefManager>(true);
        
        if (debrief != null)
        {
            Debug.Log("DebriefManager βρέθηκε, καλώ ShowDebrief()");
            debrief.ShowDebrief();
        }
        else
        {
            Debug.LogError("DebriefManager δεν βρέθηκε!");
        }
    }
    
    void SelectOption(OptionData option)
    {
        StopTimer();
        
        if (option.effects != null)
        {
            if (option.effects.score_delta != 0)
                currentScore += option.effects.score_delta;
            
            if (!string.IsNullOrEmpty(option.effects.toast))
            {
                if (ToastManager.Instance != null)
                    ToastManager.Instance.ShowToast(option.effects.toast);
            }
            
            if (option.effects.vitals_update != null && VitalsData.Instance != null)
            {
                VitalsData.Instance.ApplyVitalsUpdate(
                    option.effects.vitals_update.spo2,
                    option.effects.vitals_update.hr,
                    VitalsData.Instance.bpSystolic,
                    VitalsData.Instance.bpDiastolic,
                    VitalsData.Instance.rr
                );
            }
            
            if (GameLogger.Instance != null)
                GameLogger.Instance.LogEvent("OPTION_SELECTED", "Επιλογή: " + option.label + " (Score: " + option.effects.score_delta + ")");
        }
        
        GoToNode(option.next_node_id);
    }
    
    void StartTimer(NodeData node)
    {
        timeoutActive = true;
        
        if (timerText != null)
        {
            timerText.gameObject.SetActive(true);
            timerText.color = Color.white;
        }
        
        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);
        
        timerCoroutine = StartCoroutine(TimerRoutine(node));
    }
    
    void StopTimer()
    {
        timeoutActive = false;
        
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }
        
        if (timerText != null)
            timerText.gameObject.SetActive(false);
    }
    
    IEnumerator TimerRoutine(NodeData node)
    {
        int remaining = node.timeout.seconds;
        
        while (remaining > 0 && timeoutActive)
        {
            if (timerText != null)
            {
                timerText.text = remaining + "s";
                timerText.color = remaining <= 10 ? Color.red : Color.white;
            }
            
            yield return new WaitForSeconds(1f);
            remaining--;
        }
        
        if (timeoutActive)
            OnTimeout(node);
    }
    
    void OnTimeout(NodeData node)
    {
        timeoutActive = false;
        
        if (timerText != null)
            timerText.gameObject.SetActive(false);
        
        if (GameLogger.Instance != null)
            GameLogger.Instance.LogEvent("TIMEOUT", "Λήξη χρόνου στο node: " + node.id);
        
        if (node.timeout.on_timeout_effects != null)
        {
            var effects = node.timeout.on_timeout_effects;
            
            if (effects.vitals_update != null && VitalsData.Instance != null)
            {
                VitalsData.Instance.ApplyVitalsUpdate(
                    effects.vitals_update.spo2,
                    effects.vitals_update.hr,
                    VitalsData.Instance.bpSystolic,
                    VitalsData.Instance.bpDiastolic,
                    VitalsData.Instance.rr
                );
            }
            
            if (effects.score_delta != 0)
                currentScore += effects.score_delta;
            
            if (!string.IsNullOrEmpty(effects.toast))
            {
                if (ToastManager.Instance != null)
                    ToastManager.Instance.ShowToastStyled(effects.toast, "danger");
            }
        }
        
        GoToNode(node.timeout.next_node_id);
    }
    
    Button CreateOptionButton(string label)
    {
        if (optionButtonPrefab == null) return null;
        
        GameObject btnObj = Instantiate(optionButtonPrefab, optionsContainer.transform);
        Button btn = btnObj.GetComponent<Button>();
        TextMeshProUGUI btnText = btnObj.GetComponentInChildren<TextMeshProUGUI>();
        
        if (btnText != null) btnText.text = label;
        
        return btn;
    }
    
    void ClearOptions()
    {
        if (optionsContainer == null) return;
        
        foreach (Transform child in optionsContainer.transform)
            Destroy(child.gameObject);
    }
}