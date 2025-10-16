using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI scoreText;

    private int score;

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

    private void Start()
    {
        UpdateScoreUI();
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText)
            scoreText.text = $"score: {score}"; //string interpolation, very nice
    }
}