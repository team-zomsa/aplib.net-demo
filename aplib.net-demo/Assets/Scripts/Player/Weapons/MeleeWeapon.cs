using UnityEngine;

public class MeleeWeapon : Weapon
{
    [SerializeField] private float _damage = 25;
    [SerializeField] private float _range = 3;

    private Transform _playerTransform;

    private void Start()
    {
        _playerTransform = transform.parent;
    }

    /// <summary>
    /// Shoot a ray from the weapon's position in the direction it is facing.
    /// Look for an entity with an Enemy tag and deal damage to it.
    /// </summary>
    public override void UseWeapon()
    {
        RaycastHit hit;
        if (Physics.Raycast(_playerTransform.position, transform.forward, out hit, _range))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                hit.collider.GetComponent<BasicEnemy>().TakeDamage(_damage);
                Debug.Log("Swinging sword at " + hit.collider.name);
            }
        }
        else 
        {
            Debug.Log("Swinging sword at nothing");
        }
    }
}
