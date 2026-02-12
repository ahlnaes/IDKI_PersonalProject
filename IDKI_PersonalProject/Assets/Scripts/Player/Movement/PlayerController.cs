using System;
using System.Collections.Generic;
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

        [Header("Damage Detection")]
        [SerializeField] private float damageRadius = 1f;

        public GameManager gameManager;

        private float baseSpeed;
        private Coroutine speedBuffCo;
        private Coroutine healthBuffCo;
        private readonly HashSet<int> damagedBy = new HashSet<int>();

        private ContinuousMoveProvider moveProvider;
        private Camera vrCamera;

        private void Awake()
        {
            vrCamera = Camera.main;
            currentHealth = maxHealth;
            if (speedfx != null) speedfx.SetActive(false);

            moveProvider = GetComponentInChildren<ContinuousMoveProvider>();
            if (moveProvider != null)
                baseSpeed = moveProvider.moveSpeed;
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
            if (attackAction && aimController && weapon)
            {
                if (attackAction.action.IsPressed())
                    weapon.Fire(aimController.forward);
            }

            CheckEnemyDamage();
        }

        private void CheckEnemyDamage()
        {
            var camPos = vrCamera.transform.position;
            var center = new Vector3(camPos.x, 0.5f, camPos.z);
            var hits = Physics.OverlapSphere(center, damageRadius);
            foreach (var hit in hits)
            {
                if (!hit.CompareTag("Enemy")) continue;
                var id = hit.GetInstanceID();
                if (damagedBy.Contains(id)) continue;

                var enemy = hit.GetComponent<Enemy>();
                if (enemy == null) continue;

                currentHealth -= enemy.Damage;
                enemy.Kill();
                UpdateHealthBar();

                if (currentHealth <= 0 && gameManager != null)
                {
                    gameManager.GameOver();
                    return;
                }
            }
        }

        private void UpdateHealthBar()
        {
            if (healthBar != null)
                healthBar.fillAmount = Mathf.Clamp01(currentHealth / maxHealth);
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
