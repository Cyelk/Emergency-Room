using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

public class ValidationManager : MonoBehaviour
{
    public static ValidationManager Instance;
    
    public int validationScore = 0;
    public List<string> validationLog = new List<string>();
    
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    public int ValidateField(string fieldName, string userInput)
    {
        if (ScenarioLoader.Instance == null || ScenarioLoader.Instance.CurrentScenario == null)
            return 0;
        
        if (string.IsNullOrEmpty(userInput))
            return 0;
        
        FieldValidation rule = GetValidationRule(fieldName);
        
        if (rule == null)
        {
            Debug.Log("Δεν βρέθηκε κανόνας για: " + fieldName);
            return 0;
        }
        
        bool isValid = false;
        
        if (rule.type == "keywords")
        {
            string normalizedInput = NormalizeText(userInput);
            isValid = rule.keywords.Any(k => normalizedInput.Contains(NormalizeText(k)));
        }
        else if (rule.type == "expected")
        {
            string trimmedInput = userInput.Trim();
            isValid = rule.expected.Any(e => 
                NormalizeText(trimmedInput) == NormalizeText(e) || 
                NormalizeText(trimmedInput) == NormalizeText(e + "%"));
        }
        
        int score = isValid ? rule.score_if_valid : rule.score_if_invalid;
        
        string result = isValid ? "ΝΑΙ" : "ΟΧΙ";
        string logEntry = result + " " + fieldName + ": '" + userInput + "' (" + (score >= 0 ? "+" : "") + score + ")";
        validationLog.Add(logEntry);
        
        Debug.Log("Validation: " + logEntry);
        
        validationScore += score;
        
        if (GameLogger.Instance != null)
            GameLogger.Instance.LogEvent("VALIDATION", logEntry);
        
        return score;
    }
    
    string NormalizeText(string text)
    {
        if (string.IsNullOrEmpty(text)) return "";
        
        string decomposed = text.Normalize(NormalizationForm.FormD);
        StringBuilder sb = new StringBuilder();
        
        foreach (char c in decomposed)
        {
            UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(c);
            if (uc != UnicodeCategory.NonSpacingMark)
            {
                sb.Append(c);
            }
        }
        
        return sb.ToString().Normalize(NormalizationForm.FormC).ToLower();
    }
    
    FieldValidation GetValidationRule(string fieldName)
    {
        if (ScenarioLoader.Instance == null || ScenarioLoader.Instance.CurrentScenario == null)
            return null;
        
        var forms = ScenarioLoader.Instance.CurrentScenario.ehr_config.forms;
        
        if (forms.assessment_form != null && forms.assessment_form.validation != null)
        {
            if (fieldName == "observation") return forms.assessment_form.validation.observation;
        }
        
        if (forms.intervention_form != null && forms.intervention_form.validation != null)
        {
            if (fieldName == "fiO2_setting") return forms.intervention_form.validation.fiO2_setting;
        }
        
        if (forms.communication_log != null && forms.communication_log.validation != null)
        {
            if (fieldName == "recipient") return forms.communication_log.validation.recipient;
            if (fieldName == "outcome") return forms.communication_log.validation.outcome;
            if (fieldName == "reason") return forms.communication_log.validation.reason;
        }
        
        return null;
    }
    
    public int GetValidationScore()
    {
        return validationScore;
    }
    
    public List<string> GetValidationLog()
    {
        return validationLog;
    }
    
    public void ResetValidation()
    {
        validationScore = 0;
        validationLog.Clear();
        Debug.Log("Validation reset");
    }
}