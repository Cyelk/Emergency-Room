using UnityEngine;
using TMPro;

public class VitalsDisplay3D : MonoBehaviour
{
    public TextMeshPro vitalsText;
    
    void Start()
    {
        if (vitalsText == null)
            vitalsText = GetComponent<TextMeshPro>();
        
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
            vitalsText.text = VitalsData.Instance.GetVitalsText();
        }
    }
}