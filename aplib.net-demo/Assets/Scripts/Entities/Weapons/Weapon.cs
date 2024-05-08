using UnityEngine;

namespace Entities.Weapons
{

    /// <summary>
    /// Abstract weapons class as a base for all weapons.
    /// </summary>
    public abstract class Weapon : MonoBehaviour
    {
        // The player sound component used to play weapon sounds.
        [SerializeField] protected EntitySound _entitySound;

        /// <summary>
        /// The tag of the target that the weapon can hit.
        /// </summary>
        public string TargetTag = "Enemy";

        /// <summary>
        /// Use the weapon.
        /// </summary>
        public abstract void UseWeapon();
    }
}
