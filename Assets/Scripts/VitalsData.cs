using UnityEngine;
using System;

public class VitalsData : MonoBehaviour
{
    public int spo2 = 88;
    public int hr = 110;
    public int bpSystolic = 135;
    public int bpDiastolic = 85;
    public int rr = 22;
    
    public event Action OnVitalsChanged;
    
    public static VitalsData Instance;
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    void Start()
    {
        OnVitalsChanged?.Invoke();
    }
    
    public void SetSpO2(int value)
    {
        spo2 = Mathf.Clamp(value, 0, 100);
        OnVitalsChanged?.Invoke();
        Debug.Log("SpO2: " + spo2);
    }
    
    public void SetHR(int value)
    {
        hr = Mathf.Clamp(value, 0, 250);
        OnVitalsChanged?.Invoke();
        Debug.Log("HR: " + hr);
    }
    
    public void SetBP(int systolic, int diastolic)
    {
        bpSystolic = systolic;
        bpDiastolic = diastolic;
        OnVitalsChanged?.Invoke();
        Debug.Log("BP: " + bpSystolic + "/" + bpDiastolic);
    }
    
    public void SetRR(int value)
    {
        rr = Mathf.Clamp(value, 0, 60);
        OnVitalsChanged?.Invoke();
        Debug.Log("RR: " + rr);
    }
    
    public void ApplyVitalsUpdate(int newSpo2, int newHr, int newBpSys, int newBpDia, int newRr)
    {
        // Μην επιτρέπεις 0 αν η νέα τιμή είναι 0 και η παλιά δεν είναι
        if (newSpo2 == 0 && spo2 > 0)
        {
            Debug.LogWarning("Απόπειρα μηδενισμού SpO2 - Αγνοήθηκε");
            return;
        }
        
        if (newHr == 0 && hr > 0)
        {
            Debug.LogWarning("Απόπειρα μηδενισμού HR - Αγνοήθηκε");
            return;
        }
        
        spo2 = newSpo2;
        hr = newHr;
        bpSystolic = newBpSys;
        bpDiastolic = newBpDia;
        rr = newRr;
        OnVitalsChanged?.Invoke();
        Debug.Log("Vitals Updated: SpO2=" + spo2 + " HR=" + hr + " BP=" + bpSystolic + "/" + bpDiastolic + " RR=" + rr);
        
        if (GameLogger.Instance != null)
            GameLogger.Instance.LogEvent("VITALS_CHANGE", 
                "SpO2: " + spo2 + "%, HR: " + hr + " bpm, BP: " + bpSystolic + "/" + bpDiastolic + ", RR: " + rr);
    }
    
    public string GetVitalsText()
    {
        return $"SpO2: {spo2}%\nHR: {hr} bpm\nBP: {bpSystolic}/{bpDiastolic}\nRR: {rr}";
    }
    
    public Color GetSpO2Color()
    {
        if (spo2 < 90) return Color.red;
        if (spo2 < 94) return Color.yellow;
        return Color.green;
    }
    
    public Color GetHRColor()
    {
        if (hr > 120 || hr < 50) return Color.red;
        if (hr > 100 || hr < 60) return Color.yellow;
        return Color.green;
    }
}