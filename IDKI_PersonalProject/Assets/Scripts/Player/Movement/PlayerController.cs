using UnityEngine;
using UnityEngine.InputSystem;

// playercontroller for moving the character relative to how the camera is facing in the isometric view
namespace Player.Movement
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        public float speed = 4f;
    
        [Header("Dash")]
        public float dashSpeed = 20f;
        public float dashDuration = 0.2f;
        public float dashCooldown = 2f;

        [Header("Input")] 
        public InputActionReference moveAction;
        public InputActionReference dashAction;
    
        private CharacterController controller;
        private DashAbility dash;
        private CameraRelativeDirection mapper;

        // stuff to make dashing work etc
        private System.Action<InputAction.CallbackContext> dashHandler;
        private bool dashPressedThisFrame;
        public System.Action DashStarted;
        public System.Action DashEnded;

        private void Awake()
        { 
            controller = GetComponent<CharacterController>();
            dash = new DashAbility(dashSpeed, dashDuration, dashCooldown);
            dash.OnDashStarted += () => DashStarted?.Invoke();
            dash.OnDashEnded += () => DashEnded?.Invoke();

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
            var input = !moveAction ?  Vector2.zero : moveAction.action.ReadValue<Vector2>();
            var dir = mapper.Map(input);
        
            // movement
            //if (dir != Vector3.zero) transform.forward = dir;
            controller.SimpleMove(dir * speed);

            if (!dashPressedThisFrame) return;
            dash.TryDash(dir, transform.forward);
            dashPressedThisFrame = false;
        }

        public float GetRemainingCooldown() => dash.RemainingCooldown;
        public bool IsDashing() => dash.IsDashing;


    }
}