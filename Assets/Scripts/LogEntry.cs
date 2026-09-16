using System;

[Serializable]
public class LogEntry
{
    public string timestamp;
    public string eventType;
    public string details;
    public int scoreAtTime;
    
    public LogEntry(string type, string detail, int score)
    {
        timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        eventType = type;
        details = detail;
        scoreAtTime = score;
    }
}