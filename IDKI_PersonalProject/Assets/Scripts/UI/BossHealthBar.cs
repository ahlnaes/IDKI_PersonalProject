using UnityEngine;

namespace UI
{
    public class BossHealthBar : MonoBehaviour
    {
        [SerializeField] private RectTransform fill; 
        [SerializeField] private float maxWidth = 400f;

        private Health target;

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        public void Bind(Health bossHealth)
        {
            if (bossHealth == null) return;

            target = bossHealth;
            target.OnChanged += OnHealthChanged;
            target.OnDied    += OnBossDied;

            gameObject.SetActive(true);
            OnHealthChanged(target.Current, target.Max); // init
        }

        private void OnDestroy()
        {
            if (target != null)
            {
                target.OnChanged -= OnHealthChanged;
                target.OnDied    -= OnBossDied;
            }
        }

        private void OnHealthChanged(float current, float max)
        {
            float t = (max > 0f) ? Mathf.Clamp01(current / max) : 0f;
            if (fill)
            {
                var size = fill.sizeDelta;
                size.x = maxWidth * t;
                fill.sizeDelta = size;
            }
        }

        private void OnBossDied()
        {
            gameObject.SetActive(false);
        }
    }
}