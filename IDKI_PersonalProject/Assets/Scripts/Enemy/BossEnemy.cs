using UnityEngine;
using System.Collections;

public class BossEnemy : Enemy
{
    [Header("Pattern")]
    [SerializeField] private int   numBeams = 8;
    [SerializeField] private float laserLength = 25f;
    [SerializeField] private float telegraphDuration = 0.5f;
    [SerializeField] private float fireDuration = 3f;
    [SerializeField] private float intervalBetweenBursts = 2f;
    [SerializeField] private float spinSpeed = 90f; // degrees/sec while firing

    [Header("Damage")]
    [SerializeField] private float damagePerSecond = 5f;
    [SerializeField] private LayerMask hitMask = ~0;

    [Header("Visuals")]
    [SerializeField] private Material telegraphMat;
    [SerializeField] private Material laserMat;
    [SerializeField] private float   lineWidth = 0.05f;

    private LineRenderer[] lines;
    private bool lasersActive;
    private float baseAngleDeg;

    protected override void Awake()
    {
        base.Awake();
        // dynamic spawn of beams (kept)
        lines = new LineRenderer[numBeams];
        for (int i = 0; i < numBeams; i++)
        {
            var go = new GameObject($"Laser_{i}");
            go.transform.SetParent(transform, false);

            var lr = go.AddComponent<LineRenderer>();
            lr.positionCount = 2;
            lr.material = telegraphMat;
            lr.startWidth = lr.endWidth = lineWidth;
            lr.useWorldSpace = true;
            lr.enabled = false;
            lines[i] = lr;
        }
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
            SetMaterial(telegraphMat);
            SetEnabled(true);
            UpdateLines();               // place telegraph lines
            yield return waitTelegraph;

            // Fire (spin active)
            SetMaterial(laserMat);
            lasersActive = true;
            yield return waitFire;

            // Stop firing
            lasersActive = false;
            SetEnabled(false);

            yield return waitBetween;
        }
    }

    private void SetMaterial(Material m)
    {
        foreach (var lr in lines) lr.material = m;
    }

    private void SetEnabled(bool e)
    {
        foreach (var lr in lines) lr.enabled = e;
    }

    // Update runs ONLY the firing spin + damage
    protected override void Update()
    {
        if (!lasersActive) return;

        // spin while firing
        baseAngleDeg += spinSpeed * Time.deltaTime;

        // keep lines updated to new angles
        UpdateLines();

        // damage along each beam
        DoRaycastDamage();
    }

    private void UpdateLines()
    {
        var start = transform.position; start.y = 0f;
        float step = 360f / numBeams;

        for (int i = 0; i < numBeams; i++)
        {
            float ang = baseAngleDeg + i * step;
            Vector3 dir = DirFromAngle(ang);
            Vector3 end = start + dir * laserLength; end.y = 0f;

            var lr = lines[i];
            lr.SetPosition(0, start);
            lr.SetPosition(1, end);
        }
    }

    private void DoRaycastDamage()
    {
        var start = transform.position; start.y = 0f;
        float step = 360f / numBeams;

        // small lift avoids self/floor hit if needed; set to 0 if your floor isn't in the mask
        Vector3 origin = start + Vector3.up * 0.05f;

        for (int i = 0; i < numBeams; i++)
        {
            float ang = baseAngleDeg + i * step;
            Vector3 dir = DirFromAngle(ang);

            if (Physics.Raycast(origin, dir, out var hit, laserLength, hitMask, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.TryGetComponent<IDamageable>(out var dmg))
                    dmg.TakeDamage(damagePerSecond * Time.deltaTime);
            }
        }
    }

    private static Vector3 DirFromAngle(float angleDeg)
    {
        float r = angleDeg * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(r), 0f, Mathf.Sin(r)).normalized;
    }
}
