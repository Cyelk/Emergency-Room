using UnityEngine;
using TMPro;

public class VitalsDisplayEHR : MonoBehaviour
{
    public TextMeshProUGUI vitalsText;
    
    void Start()
    {
        if (vitalsText == null)
            vitalsText = GetComponent<TextMeshProUGUI>();
        
        if (VitalsData.Instance != null)
        {
            VitalsData.Instance.OnVitalsChanged += UpdateDisplay;
            UpdateDisplay();
        }
    }
    
    void OnDestroy()
    {
        if (VitalsData.Instance != null)
        {
            VitalsData.Instance.OnVitalsChanged -= UpdateDisplay;
        }
    }
    
    void UpdateDisplay()
    {
        if (vitalsText != null && VitalsData.Instance != null)
        {
            vitalsText.text = "📊 ΖΩΤΙΚΑ ΣΗΜΕΙΑ\n" + VitalsData.Instance.GetVitalsText();
            vitalsText.color = VitalsData.Instance.GetSpO2Color();
        }
    }
}