using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;

namespace Player.Movement
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Input")]
        public InputActionReference attackAction;

        [Header("VR Aiming")]
        [SerializeField] private Transform aimController;

        [Header("Weapon")]
        [SerializeField] private WeaponController weapon;

        [Header("Health")]
        [SerializeField] private float maxHealth = 10f;
        [SerializeField] private float currentHealth;
        [SerializeField] private Image healthBar;

        [Header("VFx")]
        [SerializeField] private GameObject healfx;
        [SerializeField] private GameObject speedfx;

        public GameManager gameManager;

        private float baseSpeed;
        private Coroutine speedBuffCo;
        private Coroutine healthBuffCo;

        private ContinuousMoveProvider moveProvider;

        private void Awake()
        {
            currentHealth = maxHealth;
            if (speedfx != null) speedfx.SetActive(false);

            moveProvider = GetComponentInChildren<ContinuousMoveProvider>();
            if (moveProvider != null)
                baseSpeed = moveProvider.moveSpeed;

            if (attackAction == null)
                Debug.LogWarning("[PlayerController] attackAction is not assigned!");
            if (aimController == null)
                Debug.LogWarning("[PlayerController] aimController is not assigned!");
            if (weapon == null)
                Debug.LogWarning("[PlayerController] weapon is not assigned!");
        }

        private void OnEnable()
        {
            if (attackAction != null)
                attackAction.action.Enable();
        }

        private void OnDisable()
        {
            if (attackAction != null)
                attackAction.action.Disable();
        }

        private void Update()
        {
            if (attackAction == null || aimController == null || weapon == null) return;

            if (attackAction.action.IsPressed())
            {
                weapon.Fire(aimController.forward);
            }
        }

        private void UpdateHealthBar()
        {
            if (healthBar != null)
                healthBar.fillAmount = Mathf.Clamp01(currentHealth / maxHealth);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                var enemy = other.gameObject.GetComponent<Enemy>();
                currentHealth -= enemy.Damage;
                UpdateHealthBar();
                if (currentHealth <= 0)
                {
                    gameManager.GameOver();
                }
            }
        }

        public float GetRemainingCooldown() => 0f;
        public bool IsDashing() => false;

        public float MaxHealth => maxHealth;

        public void Heal(float amount)
        {
            currentHealth = Mathf.Min(maxHealth, currentHealth + Mathf.Abs(amount));
            if (healfx != null) healfx.SetActive(true);
            healthBuffCo = StartCoroutine(HealFX(1f));
        }

        public void ApplySpeedBuff(float multiplier, float duration)
        {
            if (speedBuffCo != null) StopCoroutine(speedBuffCo);
            speedBuffCo = StartCoroutine(SpeedBuffCR(multiplier, duration));
        }

        private System.Collections.IEnumerator SpeedBuffCR(float mult, float dur)
        {
            if (moveProvider != null)
                moveProvider.moveSpeed = baseSpeed * mult;
            if (speedfx != null) speedfx.SetActive(true);
            var t = dur;
            while (t > 0f)
            {
                t -= Time.deltaTime;
                yield return null;
            }
            if (moveProvider != null)
                moveProvider.moveSpeed = baseSpeed;
            if (speedfx != null) speedfx.SetActive(false);
            speedBuffCo = null;
        }

        private System.Collections.IEnumerator HealFX(float dur)
        {
            if (healfx != null) healfx.SetActive(true);
            var t = dur;
            while (t > 0f)
            {
                t -= Time.deltaTime;
                yield return null;
            }
            if (healfx != null) healfx.SetActive(false);
        }
    }
}
