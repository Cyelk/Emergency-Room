using UnityEngine;
using TMPro;
using System.Collections;

public class ToastManager : MonoBehaviour
{
    public static ToastManager Instance;
    
    public GameObject toastPanel;
    public TextMeshProUGUI toastText;
    public float defaultDuration = 2.5f;
    
    private Coroutine currentToast;
    private bool isPersistent = false;
    
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    void Start()
    {
        if (toastPanel != null)
            toastPanel.SetActive(false);
    }
    
    public void ShowToast(string message, float duration = -1f)
    {
        if (isPersistent) return;
        
        if (duration < 0) duration = defaultDuration;
        
        if (currentToast != null)
            StopCoroutine(currentToast);
        
        currentToast = StartCoroutine(ShowToastRoutine(message, duration));
    }
    
    IEnumerator ShowToastRoutine(string message, float duration)
    {
        if (toastPanel == null || toastText == null) yield break;
        
        toastText.text = message;
        toastPanel.SetActive(true);
        
        Debug.Log("💬 Toast: " + message);
        
        yield return new WaitForSeconds(duration);
        
        toastPanel.SetActive(false);
        currentToast = null;
    }
    
    public void ShowToastStyled(string message, string style)
    {
        if (isPersistent) return;
        
        Color color = Color.white;
        
        switch (style)
        {
            case "danger": color = Color.red; break;
            case "warning": color = Color.yellow; break;
            case "success": color = Color.green; break;
            case "info": color = Color.cyan; break;
            default: color = Color.white; break;
        }
        
        if (toastText != null)
            toastText.color = color;
        
        ShowToast(message);
    }
    
    public void ShowPersistentToast(string message, string style)
    {
        isPersistent = true;
        
        if (currentToast != null)
            StopCoroutine(currentToast);
        
        Color color = Color.white;
        
        switch (style)
        {
            case "danger": color = Color.red; break;
            case "warning": color = Color.yellow; break;
            case "success": color = Color.green; break;
            case "info": color = Color.cyan; break;
            default: color = Color.white; break;
        }
        
        if (toastPanel != null && toastText != null)
        {
            toastText.text = message;
            toastText.color = color;
            toastPanel.SetActive(true);
        }
        
        Debug.Log("🚨 Persistent Toast: " + message);
    }
    
    public void HidePersistentToast()
    {
        isPersistent = false;
        
        if (toastPanel != null)
            toastPanel.SetActive(false);
    }
}