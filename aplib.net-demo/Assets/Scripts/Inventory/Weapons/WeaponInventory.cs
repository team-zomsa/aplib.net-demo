using System.Collections.Generic;

public class WeaponInventory : AbstractInventory
{
    protected Queue<Item> weaponList;

    private Weapon _meleeWeapon;
    private Weapon _rangedWeapon;
    public void Start()
    {
        _meleeWeapon = GetComponent<MeleeWeapon>();
        _rangedWeapon = GetComponent<RangedWeapon>();

        //once we get another way to acquire weapons, this can be removed

        weaponList.Enqueue(_meleeWeapon);
        weaponList.Enqueue(_rangedWeapon);
    }
}
