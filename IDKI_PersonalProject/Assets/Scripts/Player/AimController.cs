using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class AimController : MonoBehaviour
    {
        [Header("Input")] 
        [SerializeField] private InputActionReference lookStick;
        [SerializeField] private InputActionReference pointerPos;

        [Header("Aim")] 
        [SerializeField] private LayerMask aimMask = ~0; //bitwise not operation
        [SerializeField] private float rotateSpeedDegPerSec = 720;
        [SerializeField] private float maxRayDistance = 200f;
    
        private Camera cam;

        private void Awake()
        {
            cam = Camera.main;
        }

        private void OnEnable()
        {
            lookStick?.action.Enable();
            pointerPos?.action.Enable();
        }

        private void OnDisable()
        {
            lookStick?.action.Disable();
            pointerPos?.action.Disable();
        }

        private void LateUpdate()
        {
            UsePointerAim();
        }

        private void UsePointerAim()
        {
            if (!pointerPos) return;

            if (!cam) return;

            var screen = pointerPos.action.ReadValue<Vector2>(); // 2d vector screen coords (mouse pos)
            var ray = cam.ScreenPointToRay(screen); // docs: Returns a ray going from camera through a screen point. makes the screen pos world space
            Debug.DrawRay(ray.origin, ray.direction, Color.red);

            if (!Physics.Raycast(ray, out var hit, maxRayDistance, aimMask, QueryTriggerInteraction.Ignore)) return;
            var to = hit.point - transform.position;
            to.y = 0;
            RotateTowards(to);
        }

        private void RotateTowards(Vector3 direction)
        {
            if (direction == Vector3.zero) return;
            var target = Quaternion.LookRotation(direction.normalized, Vector3.up);
            transform.rotation = rotateSpeedDegPerSec <= 0f ? target : Quaternion.Lerp(transform.rotation, target, rotateSpeedDegPerSec * Time.deltaTime);
        }

        public Vector3 GetAimDirection()
        {
            return cam.ScreenToWorldPoint(pointerPos.action.ReadValue<Vector2>()).normalized;
        }
    }
}