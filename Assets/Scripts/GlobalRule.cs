using UnityEngine;
using TMPro;
using System.Collections;

public class GlobalRules : MonoBehaviour
{
    public static GlobalRules Instance;
    
    [Header("References")]
    public TextMeshProUGUI vitalsTextEHR;
    
    [Header("Settings")]
    public int spo2Threshold = 90;
    public float blinkInterval = 0.5f;
    
    private bool isAlarmActive = false;
    private Coroutine blinkCoroutine;
    private GameObject persistentToast;
    
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    void Start()
    {
        if (VitalsData.Instance != null)
        {
            VitalsData.Instance.OnVitalsChanged += CheckGlobalRules;
            CheckGlobalRules();
        }
    }
    
    void OnDestroy()
    {
        if (VitalsData.Instance != null)
        {
            VitalsData.Instance.OnVitalsChanged -= CheckGlobalRules;
        }
    }
    
    void CheckGlobalRules()
    {
        if (VitalsData.Instance == null) return;
        
        bool shouldAlarm = VitalsData.Instance.spo2 < spo2Threshold;
        
        if (shouldAlarm && !isAlarmActive)
        {
            StartAlarm();
        }
        else if (!shouldAlarm && isAlarmActive)
        {
            StopAlarm();
        }
    }
    
    void StartAlarm()
    {
        isAlarmActive = true;
        Debug.Log("🚨 ΣΥΝΑΓΕΡΜΟΣ: SpO2 < " + spo2Threshold + "%");
        
        // Ξεκίνα blinking στο VitalsText
        if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
        blinkCoroutine = StartCoroutine(BlinkRoutine());
        
        // Δείξε μόνιμο toast
        if (ToastManager.Instance != null)
        {
            ToastManager.Instance.ShowPersistentToast(
                "🚨 ΣΥΝΑΓΕΡΜΟΣ: SpO2 < 90% (Υποξαιμία)!",
                "danger"
            );
        }
    }
    
    void StopAlarm()
    {
        isAlarmActive = false;
        Debug.Log("✅ Ο συναγερμός έληξε");
        
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }
        
        if (ToastManager.Instance != null)
        {
            ToastManager.Instance.HidePersistentToast();
        }
        
        // Επαναφορά χρώματος - ΠΑΝΤΑ
        if (vitalsTextEHR != null)
        {
            vitalsTextEHR.color = Color.white;
        }
    }
    
    IEnumerator BlinkRoutine()
    {
        while (isAlarmActive)
        {
            if (vitalsTextEHR != null)
            {
                if (vitalsTextEHR.color == Color.red)
                {
                    vitalsTextEHR.color = Color.white;
                }
                else
                {
                    vitalsTextEHR.color = Color.red;
                }
            }
            
            yield return new WaitForSeconds(blinkInterval);
        }
    }
}