using UnityEngine;

/// <summary>
/// A basic melee weapon that deals damage to enemies within a short range.
/// </summary>
public class MeleeWeapon : Weapon
{
    [SerializeField] private int _damage = 25;
    [SerializeField] private int _range = 3;

    private Transform _playerTransform;

    private void Start()
    {
        _playerTransform = transform.parent;
    }

    /// <summary>
    /// Shoots a ray from the players's position in the direction it is facing.
    /// Look for an entity with an Enemy tag within range and deal damage to it.
    /// </summary>
    public override void UseWeapon()
    {
        RaycastHit hit;
        if (Physics.Raycast(_playerTransform.position, transform.forward, out hit, _range))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                // We need to check if the enemy has an BasicEnemy component before dealing damage
                BasicEnemy enemy = hit.collider.GetComponent<BasicEnemy>() ?? hit.collider.GetComponentInParent<DummyEnemy>();
                if (enemy != null) enemy.TakeDamage(_damage);
                else Debug.Log("This enemy does not have a BasicEnemy component!");
            }
        }
    }
}
