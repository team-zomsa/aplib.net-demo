using Entities.Weapons;
using System.Collections;
using UnityEngine;

/// <summary>
/// Melee enemy that attacks the player when in range.
/// It inherits from DummyEnemy to add respawn functionality.
/// </summary>
[RequireComponent(typeof(Timer))]
public class MeleeEnemy : RespawningEnemy
{
    [SerializeField]
    private float _attackCooldown = 2f;

    [SerializeField]
    private float _hitDelay = 1f;

    [SerializeField]
    private float _swingWidth = 1.4f;

    [SerializeField]
    private float _swingLength = 4f;

    private readonly float _sizeIncrease = 1.2f;
    private MeleeWeapon _meleeWeapon;
    private Timer _cooldownTimer;
    private bool _isSwinging = false;

    /// <summary>
    /// Sets the target tag for the melee weapon.
    /// Also initializes the timers.
    /// </summary>
    protected override void Awake()
    {
        _cooldownTimer = GetComponent<Timer>();
        _cooldownTimer.SetExactTime(_attackCooldown);

        _meleeWeapon = GetComponentInChildren<MeleeWeapon>();
        _meleeWeapon.Initialize(_damagePoints, _targetTag, _swingLength, _swingWidth);

        base.Awake();
    }

    /// <summary>
    /// Updates the timer and starts the swing if the cooldown is finished and enemies are in sight.
    /// </summary>
    protected override void Update()
    {
        if (_isSwinging)
            return;

        base.Update();

        if (_cooldownTimer.IsFinished() && _meleeWeapon.EnemiesWithinRange())
        {
            _cooldownTimer.Reset();
            StartSwing();
        }
    }

    /// <summary>
    /// Start a weapon swing.
    /// Also makes the enemy slightly bigger during the swing to give a visual cue.
    /// </summary>
    private void StartSwing()
    {
        _isSwinging = true;
        transform.localScale *= _sizeIncrease;
        StartCoroutine(SwingCoroutine());
    }

    /// <summary>
    /// Attacks with the meleeweapon if the hit delay is finished.
    /// </summary>
    /// <returns>Coroutine for the swing.</returns>
    private IEnumerator SwingCoroutine()
    {
        yield return new WaitForSeconds(_hitDelay);
        _meleeWeapon.UseWeapon();
        transform.localScale /= _sizeIncrease;
        _isSwinging = false;
    }
}
