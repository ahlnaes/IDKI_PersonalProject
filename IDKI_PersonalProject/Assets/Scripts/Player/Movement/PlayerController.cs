using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Movement
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour, IDamageable
    {
        private static readonly int DashHash = Animator.StringToHash("Dash");

        [Header("Movement")]
        public float speed = 4f;

        [Header("Dash")]
        public float dashSpeed = 20f;
        public float dashDuration = 0.2f;
        public float dashCooldown = 2f;

        [Header("Input")]
        public InputActionReference moveAction;
        public InputActionReference dashAction;

        [Header("Weapon")]
        [SerializeField] private WeaponController weapon;

        [Header("Health")] 
        public Health health;

        private CharacterController controller;
        private Animator animator;
        private DashAbility dash;
        private CameraRelativeDirection mapper;
        private TrailRenderer trailRenderer;
        private AudioSource audioSource;

        private System.Action<InputAction.CallbackContext> dashHandler;
        private bool dashPressedThisFrame;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            animator   = GetComponent<Animator>();
            trailRenderer = GetComponentInChildren<TrailRenderer>();
            audioSource = GetComponent<AudioSource>();
            mapper = new CameraRelativeDirection(Camera.main);
            health = GetComponent<Health>();

            dash = new DashAbility(dashSpeed, dashDuration, dashCooldown);
            dash.OnDashStarted += () =>
            {
                animator.ResetTrigger(DashHash);
                animator.SetTrigger(DashHash);
                if (audioSource?.clip) AudioSource.PlayClipAtPoint(audioSource.clip, transform.position);
                if (trailRenderer) trailRenderer.enabled = true;
            };
            dash.OnDashEnded += () =>
            {
                if (trailRenderer) trailRenderer.enabled = false;
            };

            if (dashAction != null)
            {
                dashHandler = _ => dashPressedThisFrame = true;
                dashAction.action.performed += dashHandler;
            }
        }

        private void OnEnable()
        {
            moveAction?.action.Enable();
            dashAction?.action.Enable();
        }

        private void OnDisable()
        {
            if (dashAction != null && dashHandler != null)
                dashAction.action.performed -= dashHandler;
            moveAction?.action.Disable();
            dashAction?.action.Disable();
        }

        private void Update()
        {
            var dt = Time.deltaTime;

            // dash 
            dash.Tick(controller, dt);
            if (dash.IsDashing) { dashPressedThisFrame = false; return; }

            // movement
            var input = moveAction ? moveAction.action.ReadValue<Vector2>() : Vector2.zero;
            var dir = mapper.Map(input);
            controller.SimpleMove(dir * speed);

            if (dashPressedThisFrame)
            {
                dash.TryDash(dir, transform.forward);
                dashPressedThisFrame = false;
            }

            // fire 
            if (Mouse.current.leftButton.wasPressedThisFrame || Input.GetMouseButton(0))
            {
                var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                if (Physics.Raycast(ray, out var hit))
                {
                    var direction = (hit.point - weapon.SourcePosition).normalized;
                    weapon.Fire(direction);
                }
            }
        }

        public float GetRemainingCooldown() => dash.RemainingCooldown;
        public bool IsDashing() => dash.IsDashing;
        public bool IsDead => health.IsDead;

        public void TakeDamage(float damage)
        {
            health.TakeDamage(damage);
        }
    }
}
