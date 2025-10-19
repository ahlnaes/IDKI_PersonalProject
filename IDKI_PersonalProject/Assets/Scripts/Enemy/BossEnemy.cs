using UnityEngine;
using System.Collections;

public class BossEnemy : Enemy
{
    [Header("Beams (assign in Inspector)")]
    [SerializeField] private LineRenderer[] beams;

    [Header("Pattern")]
    [SerializeField] private float laserLength = 25f;
    [SerializeField] private float telegraphDuration = 0.5f;
    [SerializeField] private float fireDuration = 3f;
    [SerializeField] private float intervalBetweenBursts = 2f;
    [SerializeField] private float spinSpeed = 90f; // degrees/sec while firing
    [SerializeField] private float beamY = 0f;      // keep beams flat

    [Header("Damage")]
    [SerializeField] private float damagePerSecond = 5f;
    [SerializeField] private LayerMask hitMask = ~0; // what lasers can damage

    [Header("Visuals")]
    [SerializeField] private Material telegraphMat;
    [SerializeField] private Material laserMat;

    private bool lasersActive;
    private float baseAngleDeg;

    protected override void Awake()
    {
        base.Awake();
        // Ensure beams are all disabled initially
        if (beams != null)
            foreach (var lr in beams) if (lr) lr.enabled = false;
    }

    private void Start()
    {
        StartCoroutine(PatternLoop());
    }

    protected override void Tick() { } // no movement

    private IEnumerator PatternLoop()
    {
        var waitTelegraph = new WaitForSeconds(telegraphDuration);
        var waitFire      = new WaitForSeconds(fireDuration);
        var waitBetween   = new WaitForSeconds(intervalBetweenBursts);

        while (!IsDead)
        {
            // Telegraph (no spin)
            SetBeamsMaterial(telegraphMat);
            SetBeamsEnabled(true);
            UpdateBeamPositions();            // positions from current baseAngleDeg
            yield return waitTelegraph;

            // Fire (spin active)
            SetBeamsMaterial(laserMat);
            lasersActive = true;
            yield return waitFire;
            lasersActive = false;

            SetBeamsEnabled(false);

            // Optional small base rotation between bursts
            baseAngleDeg += 15f;
            yield return waitBetween;
        }
    }

    private void SetBeamsMaterial(Material mat)
    {
        if (beams == null) return;
        foreach (var lr in beams) if (lr) lr.material = mat;
    }

    private void SetBeamsEnabled(bool enable)
    {
        if (beams == null) return;
        foreach (var lr in beams) if (lr) lr.enabled = enable;
    }

    private void Update()
    {
        if (!lasersActive) return;

        // Spin while firing
        baseAngleDeg += spinSpeed * Time.deltaTime;

        // Move beam endpoints
        UpdateBeamPositions();

        // Apply damage along each beam
        RaycastDamage();
    }

    private void UpdateBeamPositions()
    {
        if (beams == null || beams.Length == 0) return;

        int numBeams = beams.Length;
        float step = 360f / numBeams;

        Vector3 start = transform.position;
        start.y = beamY;

        for (int i = 0; i < numBeams; i++)
        {
            var lr = beams[i];
            if (!lr) continue;

            float ang = baseAngleDeg + i * step;
            Vector3 dir = DirFromAngle(ang);
            Vector3 end = start + dir * laserLength;
            end.y = beamY;

            lr.SetPosition(0, start);
            lr.SetPosition(1, end);
        }
    }

    private void RaycastDamage()
    {
        int numBeams = beams?.Length ?? 0;
        if (numBeams == 0) return;

        float step = 360f / numBeams;
        Vector3 start = transform.position;
        start.y = beamY;

        for (int i = 0; i < numBeams; i++)
        {
            float ang = baseAngleDeg + i * step;
            Vector3 dir = DirFromAngle(ang);

            // small lift to avoid hitting floor at y=0 if needed, set to 0 if no floor collider
            Vector3 origin = start + Vector3.up * 0.05f;

            if (Physics.Raycast(origin, dir, out var hit, laserLength, hitMask, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.TryGetComponent<IDamageable>(out var dmg))
                {
                    dmg.TakeDamage(damagePerSecond * Time.deltaTime);
                }
            }
        }
    }

    private static Vector3 DirFromAngle(float angleDeg)
    {
        float r = angleDeg * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(r), 0f, Mathf.Sin(r)).normalized;
    }
}
