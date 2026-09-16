using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class GameLogger : MonoBehaviour
{
    public static GameLogger Instance;
    
    private List<LogEntry> logEntries = new List<LogEntry>();
    
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    void Start()
    {
        LogEvent("SESSION_START", "Έναρξη σεναρίου");
    }
    
    public void LogEvent(string eventType, string details)
    {
        int currentScore = 0;
        if (ScenarioEngine.Instance != null)
            currentScore = ScenarioEngine.Instance.currentScore;
        
        LogEntry entry = new LogEntry(eventType, details, currentScore);
        logEntries.Add(entry);
        
        Debug.Log("📝 [" + eventType + "] " + details);
    }
    
    public void ExportToJSON()
    {
        string path = Path.Combine(Application.dataPath, "Logs/gamelog.json");
        
        LogWrapper wrapper = new LogWrapper();
        wrapper.entries = logEntries;
        
        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(path, json);
        
        Debug.Log("💾 Log αποθηκεύτηκε: " + path);
    }
    
    public List<LogEntry> GetLogEntries()
    {
        return logEntries;
    }
    
    public void ClearLog()
    {
        logEntries.Clear();
        Debug.Log("🗑️ Log καθαρίστηκε");
    }
    
    [System.Serializable]
    private class LogWrapper
    {
        public List<LogEntry> entries;
    }
}