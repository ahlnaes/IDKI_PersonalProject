using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    public static PowerUpSpawner Instance { get; private set; }
    [SerializeField] private GameObject powerUpPrefab;
    
    private void Awake()
    {
        // singleton since not many needed
        if (Instance && !Equals(Instance, this))
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    
    
}
