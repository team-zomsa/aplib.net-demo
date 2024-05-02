using UnityEngine;

public class RangedEnemy : DummyEnemy
{
    [SerializeField] private float _attackRange = 25f;
    [SerializeField] private float _attackCooldown = 2f;
    [SerializeField] private string _targetTag = "Player";

    private RangedWeapon _rangedWeapon;
    private Timer _attackTimer;

    protected override void Awake()
    {
        base.Awake();
        _rangedWeapon = GetComponentInChildren<RangedWeapon>();
        _rangedWeapon.TargetTag = _targetTag;
        _rangedWeapon.FirePoint = transform;
        _attackTimer = new Timer(_attackCooldown);
        _pathFind.TagToFind = _targetTag;
    }

    protected override void Update()
    {
        base.Update();
        _attackTimer.Update(Time.deltaTime);

        if (_rangedWeapon.EnemiesInRange() && _attackTimer.IsFinished()) 
        {
            _rangedWeapon.UseWeapon();
            _attackTimer.Reset();
        }
    }
}
