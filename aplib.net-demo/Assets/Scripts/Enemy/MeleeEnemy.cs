using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeleeEnemy : DummyEnemy
{
    [SerializeField] private float _attackCooldown = 1.5f;
    MeleeWeapon _meleeWeapon;
    Timer _timer;

    protected override void Awake()
    {
        base.Awake();
        _meleeWeapon = GetComponentInChildren<MeleeWeapon>();
        _meleeWeapon.TargetTag = _targetTag;
        _timer = new(_attackCooldown);
    }

    protected override void Update()
    {
        // Check the hitzone
        IEnumerable<Collider> targets = _meleeWeapon.GetHitZone();
        if (targets.Any() && _timer.IsFinished())
        {
            _timer.Reset();
            _meleeWeapon.UseWeapon();
        }
        _timer.Update(Time.deltaTime);
    }
}
