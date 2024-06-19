using Entities.Weapons;
using System.Collections;
using UnityEngine;

/// <summary>
/// Ranged enemy that attacks the player when in attack range.
/// Has a separate vision range within which it will move closer to the player.
/// </summary>
[RequireComponent(typeof(Timer))]
[RequireComponent(typeof(Animator))]
public class RangedEnemy : RespawningEnemy
{
    [SerializeField]
    private float _attackChargeUp = 1f;

    [SerializeField]
    private float _attackCooldown = 2f;

    [SerializeField]
    private int _attackRange = 10;

    private Timer _attackTimer;

    private RangedWeapon _rangedWeapon;

    private Animator _animator;

    private const float _arbitrarySmallStoppingDistance = 1f;

    /// <summary>
    /// Initialize the ranged weapon and pathfinding.
    /// Do in start to ensure the weapon/pathfinding is initialized beforehand.
    /// </summary>
    protected override void Start()
    {
        _rangedWeapon = GetComponentInChildren<RangedWeapon>();
        _rangedWeapon.Initialize(_damagePoints, _targetTag, transform, _attackRange);
        _attackTimer = gameObject.AddComponent<Timer>();
        _attackTimer.SetExactTime(_attackCooldown + _attackChargeUp);
        _pathFind.SetStoppingDistance(_attackRange - _arbitrarySmallStoppingDistance);
        _animator = GetComponent<Animator>();

        base.Start();
    }

    /// <summary>
    /// Check if the enemy can see the target and attack if it can.
    /// If the target is not visible but within range, it will move closer to the target.
    /// </summary>
    protected override void Update()
    {
        Debug.DrawRay(transform.position, Vector3.up * 20, Color.green);

        // If the target is not directly visible but within vision range, move closer to the target.
        if (_canMove && !_rangedWeapon.EnemiesInLineOfSight() && _pathFind.GoalWithinRange(_visionRange))
        {
            _pathFind.SetStoppingDistance(_arbitrarySmallStoppingDistance);
            _animator.SetBool("Walking", true);
        }

        if (_attackTimer.IsFinished() && _rangedWeapon.EnemiesInLineOfSight())
        {
            _animator.SetBool("Walking", false);
            _pathFind.SetStoppingDistance(_attackRange - _arbitrarySmallStoppingDistance);
            StartCoroutine(Attack());
        }

        base.Update();
    }

    private IEnumerator Attack()
    {
        _attackTimer.Reset();
        _animator.SetBool("Loading", true);

        yield return new WaitForSeconds(_attackChargeUp);

        _animator.SetBool("Loading", false);
        _rangedWeapon.UseWeapon();
    }
}
