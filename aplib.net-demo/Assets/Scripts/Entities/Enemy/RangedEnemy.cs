using UnityEngine;
using System.Collections;

public class RangedEnemy : DummyEnemy
{
    [SerializeField] private int _attackRange = 10;
    [SerializeField] private int _visionRange = 20;
    [SerializeField] private float _attackCooldown = 2f;
    private bool _movingCloser = false;
    private RangedWeapon _rangedWeapon;
    private Timer _attackTimer;

    /// <summary>
    /// Initialize the ranged weapon and pathfinding.
    /// Do in start to ensure the weapon/pathfinding is initialized beforehand.
    /// </summary>
    protected override void Start()
    {
        base.Start();
        _rangedWeapon = GetComponentInChildren<RangedWeapon>();       
        _rangedWeapon.Initialize(_damagePoints, _targetTag, transform, _attackRange);
        _attackTimer = new Timer(_attackCooldown);
        _pathFind.TagToFind = _targetTag;
        _pathFind.SetStoppingDistance(_attackRange - 1f);
    }

    /// <summary>
    /// Check if the enemy can see the target and attack if it can.
    /// If the target is not visible but within range, it will move closer to the target.
    /// </summary>
    protected override void Update()
    {
        _attackTimer.Update(Time.deltaTime);

        // If the target is not within vision range, do nothing.
        if (!_pathFind.GoalWithinRange(_visionRange)) 
        {
            _pathFind.ToggleAgent(false);
            return;
        }

        if (_movingCloser) 
            return;
        
        // If the target is not directly visible but within vision range, move closer to the target.
        if (!_rangedWeapon.EnemiesInLineOfSight())
        {
            _pathFind.SetStoppingDistance(1f);
            _pathFind.ToggleAgent(true);
            _movingCloser = true;
            StartCoroutine(MoveCloser());
            return;
        }

        if (_attackTimer.IsFinished()) 
        {
            _rangedWeapon.UseWeapon();
            _attackTimer.Reset();
        }

        base.Update();
    }

    /// <summary>
    /// Coroutine to move closer until the enemy is within line of sight.
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveCloser()
    {
        while (_pathFind.GoalWithinRange(_visionRange) && !_rangedWeapon.EnemiesInLineOfSight())
        {
            _pathFind.UpdateAgent();
            yield return null;
        }
        _movingCloser = false;
        _pathFind.SetStoppingDistance(_attackRange - 1f);
    }
}
