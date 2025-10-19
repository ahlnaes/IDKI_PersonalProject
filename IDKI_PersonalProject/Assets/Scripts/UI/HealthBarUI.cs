using UnityEngine;

namespace UI
{
    public class HealthBarUI : MonoBehaviour
    {
        [SerializeField] private Health player;
        [SerializeField] private RectTransform bar;
        private float baseWidth;

        private void Awake()
        {
            bar = GetComponent<RectTransform>();
            baseWidth = bar.sizeDelta.x;
        }

        private void OnEnable()
        {
            if (player) player.OnChanged += UpdateBar;
        }

        private void OnDisable()
        {
            if (player) player.OnChanged -= UpdateBar;
        }

        private void UpdateBar(float current, float max)
        {
            var frac = max > 0 ? Mathf.Clamp01(current / max) : 0f;

            var size = bar.sizeDelta;
            size.x = baseWidth * frac;
            bar.sizeDelta = size;
            bar.anchoredPosition = new Vector2(0f, bar.anchoredPosition.y);
        }
    }
}