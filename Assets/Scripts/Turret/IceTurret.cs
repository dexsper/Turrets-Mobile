using UnityEngine;
using UnityEngine.VFX;

public class IceTurret : BaseTurret
{
    [Label("Attack Settings", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField, Range(1, 180)] protected float _damageAngle = 20f;
    [SerializeField, Range(.001f, 1f)] protected float _attackRate = .1f;
    [SerializeField, Range(.1f, 1.5f)] protected float _decelerationPerAttack = 1f;

    [Label("Visual Settings", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField, NotNull] protected VisualEffect _throwEffect;

    private float _fireTime, _attack = 0f;

    public bool IsFire { get; private set; }

    protected override void Start()
    {
        base.Start();

        if (_throwEffect != null)
        {
            _throwEffect.SetFloat("Angle", _damageAngle);
        }
    }

    protected override void Fire()
    {
        if (!IsFire)
        {
            IsFire = true;
            _throwEffect.Play();
        }

        _fireTime += Time.deltaTime;
        _attack += Time.deltaTime;

        if (_attack >= _attackRate)
        {
            _attack = 0f;
            DamageArea();
        }

        if (_fireTime >= 1f)
        {
            _fireTime = 0f;

            base.Fire();
        }
    }
    private void DamageArea()
    {
        if (TargetPoint.FillBuffer(_shootPivot[_currentShootPivot].position, Aim.AimDistance))
        {
            for (int i = 0; i < TargetPoint.BufferedCount; i++)
            {
                Enemy enemy = TargetPoint.GetBuffered(i);

                if (enemy == null)
                    break;

                Vector3 targetDirection = enemy.transform.position - Aim.ArcRoot.transform.position;

                float angleBetween = Vector3.Angle(Aim.ArcRoot.transform.forward, targetDirection);

                if (angleBetween <= _damageAngle)
                {
                    enemy.ApplyDamage(_damage);
                    enemy.AddDeceleration(_decelerationPerAttack);
                }
            }
        }
    }
    protected override void StopFire()
    {
        if (IsFire)
        {
            IsFire = false;
            _throwEffect.Stop();
        }
    }


#if UNITY_EDITOR
    // This should probably go in an Editor script, but dealing with Editor scripts
    // is a pain in the butt so I'd rather not.
    private void OnDrawGizmosSelected()
    {
        if (Aim == null)
        {
            Aim = GetComponent<TurretAim>();
        }

        if (!Aim.DrawDebugArcs)
            return;

        if (Aim.TurretBase != null)
        {
            float kArcSize = Aim.AimDistance;

            Color colorDamage = new Color(1f, .5f, .5f, .1f);

            UnityEditor.Handles.color = colorDamage;

            UnityEditor.Handles.DrawSolidArc(
                Aim.ArcRoot.position, Aim.TurretBase.up,
                Aim.ArcRoot.transform.forward, _damageAngle,
                kArcSize);
            UnityEditor.Handles.DrawSolidArc(
                Aim.ArcRoot.position, Aim.TurretBase.up,
                Aim.ArcRoot.transform.forward, -_damageAngle,
                kArcSize);
        }
    }
#endif

}
