using UnityEngine;

public class DebugVariables : MonoBehaviour
{
    public static DebugVariables Instance { get; private set; }
    
    public bool enemySpawnOn;
    public bool powerUpSpawnOn;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
