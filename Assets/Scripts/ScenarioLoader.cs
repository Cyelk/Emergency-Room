using UnityEngine;
using System.IO;

public class ScenarioLoader : MonoBehaviour
{
    public static ScenarioLoader Instance;
    
    public ScenarioData CurrentScenario { get; private set; }
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        
        // Φόρτωσε το σενάριο ΑΜΕΣΩΣ
        LoadScenario("scenario.json");
    }
    
    public void LoadScenario(string fileName)
    {
        string path = Path.Combine(Application.streamingAssetsPath, fileName);
        
        if (File.Exists(path))
        {
            string jsonContent = File.ReadAllText(path);
            CurrentScenario = JsonUtility.FromJson<ScenarioData>(jsonContent);
            
            Debug.Log("✅ Σενάριο φορτώθηκε: " + CurrentScenario.title);
            Debug.Log("📊 Nodes: " + CurrentScenario.rules.nodes.Count);
        }
        else
        {
            Debug.LogError("❌ Δεν βρέθηκε το αρχείο: " + path);
        }
    }
}