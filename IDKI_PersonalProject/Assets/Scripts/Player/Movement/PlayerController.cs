using System;
using System.Net;
using UnityEngine;
using UnityEngine.InputSystem;

// playercontroller for moving the character relative to how the camera is facing in the isometric view
namespace Player.Movement
{
    public class PlayerController : MonoBehaviour
    {
        private static readonly int Dash = Animator.StringToHash("Dash");
        [Header("Movement")] public float speed = 4f;

        [Header("Dash")] public float dashSpeed = 20f;
        public float dashDuration = 0.2f;
        public float dashCooldown = 2f;

        [Header("Input")] public InputActionReference moveAction;
        public InputActionReference dashAction;

        [Header("Weapon")] [SerializeField] private WeaponController weapon;
        
        [Header("Health")]
        [SerializeField] private float maxHealth = 10f;
        [SerializeField] private float currentHealth;
        [SerializeField] private RectTransform healthBar;

        private CharacterController controller;
        private Animator animator;
        private DashAbility dash;
        private CameraRelativeDirection mapper;
        private TrailRenderer trailRenderer;
        private AudioSource audioSource;

        // stuff to make dashing work etc
        private System.Action<InputAction.CallbackContext> dashHandler;
        private bool dashPressedThisFrame;
        public System.Action DashStarted;
        public System.Action DashEnded;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            trailRenderer = GetComponentInChildren<TrailRenderer>();
            audioSource = GetComponent<AudioSource>();
            
            currentHealth = maxHealth;
            
            dash = new DashAbility(dashSpeed, dashDuration, dashCooldown);
            dash.OnDashStarted += () =>
            {
                animator.ResetTrigger(Dash);
                animator.SetTrigger(Dash);
                AudioSource.PlayClipAtPoint(audioSource.clip, transform.position);
                trailRenderer.enabled = true;
                DashStarted?.Invoke();
            };
            dash.OnDashEnded += () =>
            {
                trailRenderer.enabled = false;
                DashEnded?.Invoke();
            };

            mapper = new CameraRelativeDirection();

            if (!dashAction) return;
            dashHandler = _ => dashPressedThisFrame = true;
            dashAction.action.performed += dashHandler;
        }

        private void OnEnable()
        {
            moveAction?.action.Enable();
            dashAction?.action.Enable();
        }

        private void OnDisable()
        {
            if (dashAction != null && dashHandler != null)
            {
                dashAction.action.performed -= dashHandler;
            }

            moveAction?.action.Disable();
            dashAction?.action.Disable();
        }

        private void Update()
        {
            var dt = Time.deltaTime;

            // tick dash instead of update so update is handled in one place for the mvmt
            dash.Tick(controller, dt);
            // no movement if were in the middle of dashing
            if (dash.IsDashing)
            {
                dashPressedThisFrame = false;
                return;
            }

            // input mapped to world direction
            var input = !moveAction ? Vector2.zero : moveAction.action.ReadValue<Vector2>();
            var dir = mapper.Map(input);

            // movement
            //if (dir != Vector3.zero) transform.forward = dir;
            controller.SimpleMove(dir * speed);

            if (dashPressedThisFrame)
            {
                dash.TryDash(dir, transform.forward);
                dashPressedThisFrame = false;
            }

            if (Mouse.current.leftButton.wasPressedThisFrame || Input.GetMouseButton(0))
            {
                var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                if (!Physics.Raycast(ray, out var hit)) return;
                var direction = (hit.point - weapon.SourcePosition).normalized;
                weapon.Fire(direction);
            }
        }

        private void UpdateHealthBar()
        {
            var width = Mathf.Clamp01(currentHealth / maxHealth);
            healthBar.localScale = new Vector3(width, 1f, 1f);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                var enemy = other.gameObject.GetComponent<Enemy>();
                currentHealth -= enemy.Damage;
                UpdateHealthBar();
            }
        }

        public float GetRemainingCooldown() => dash.RemainingCooldown;
        public bool IsDashing() => dash.IsDashing;
    }
}