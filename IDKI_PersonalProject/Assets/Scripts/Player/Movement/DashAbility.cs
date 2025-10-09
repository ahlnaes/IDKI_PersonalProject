using UnityEngine;

namespace Player.Movement
{
    /// <summary>
    /// Dash Ability
    /// - Performed when requested
    /// - moves the character in dir for set duration
    /// - can have a cd
    /// </summary>
    public sealed class DashAbility
    {
        public bool IsDashing { get; private set; }
        public float RemainingCooldown => cooldown.Remaining;

        public event System.Action OnDashStarted;
        public event System.Action OnDashEnded;

        private readonly float speed;
        private readonly float duration;
        private readonly float cooldownSec;
        
        private readonly CooldownTimer cooldown = new CooldownTimer();
        private float timer;
        private Vector3 dir;

        public DashAbility(float speed, float duration, float cooldown)
        {
            this.speed = speed;
            this.duration = duration;
            cooldownSec = cooldown;
        }
        /// <summary>
        /// try to dash, returns false if already dashing or on cd
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="currentDir"></param>
        /// <returns></returns>
        public bool TryDash(Vector3 direction, Vector3 currentDir)
        {
            if (IsDashing ||cooldown.IsActive) return false;
            this.dir = direction.sqrMagnitude > 0.0001f ? direction.normalized : currentDir.normalized;
            IsDashing = true;
            timer = duration;
            cooldown.Start(cooldownSec);
            OnDashStarted?.Invoke();
            return true;
        }

        public void Tick(CharacterController controller, float dt)
        {
            cooldown.Tick(dt);
            if (!IsDashing) return;
            controller.Move(dir * (speed * dt));
            timer -= dt;

            if (!(timer <= 0f)) return;
            IsDashing = false;
            OnDashEnded?.Invoke();
        }
    }
}