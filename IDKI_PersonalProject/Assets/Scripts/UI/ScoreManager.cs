using TMPro;
using UnityEngine;

namespace UI
{
    public class ScoreManager : MonoBehaviour
    {
        private static ScoreManager Instance { get; set; }

        [SerializeField] private TextMeshProUGUI scoreText;

        private int score;

        public int Score => score;

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
}