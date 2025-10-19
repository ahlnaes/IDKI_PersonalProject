using UnityEngine;

// reusable cooldown timer for abilites etc
namespace Player
{
    public sealed class CooldownTimer
    {
        public float Remaining { get; private set; }
        public bool IsActive => Remaining > 0f;
        public void Start(float duration) => Remaining = Mathf.Max(duration, 0f);
        public void Tick(float dt) => Remaining = Mathf.Max(Remaining - dt, 0f);
    }
}
