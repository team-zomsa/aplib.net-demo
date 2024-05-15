using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class WeaponInventory : AbstractInventory
{
    public Queue<Weapon> weaponList;

    public Weapon _meleeWeapon;
    public Weapon _rangedWeapon;
    public void Start()
    {

        //todo different button for weapon and item switch
        //todo weapon visuals
        //todo indicator location
        _inventoryIndicator = GetComponent<RawImage>();

        weaponList = new Queue<Weapon>();
        //once we get another way to acquire weapons, this can be removed

        PickUpWeapon(_meleeWeapon);
        PickUpWeapon(_rangedWeapon);
        // UnityEngine.Debug.Log(weaponList.Peek().iconTexture);
        // UnityEngine.Debug.Log(weaponList.Peek());
        DisplayItem();
    }
    public override void PickUpWeapon(Weapon weapon)
    {
        bool alreadyInInventory = false;
        List<Weapon> _tempWeaponList = weaponList.ToList();
        for (int i = 0; i < weaponList.Count; i++)
        {
            if (_tempWeaponList[i].name == weapon.name)
            {
                if (!weapon.stackable)
                    return;

                _tempWeaponList[i].uses += weapon.startUses;
                alreadyInInventory = true;
                break;
            }
        }

        if (!alreadyInInventory)
        {
            weaponList.Enqueue(weapon);
            UnityEngine.Debug.Log("weapon should be added");
            DisplayItem();
        }
    }
    public override void ActivateItem()
    {
        if (weaponList.Any())
        {
            weaponList.Peek().UseWeapon();
        }

        DisplayItem();
    }
    /// <summary>
    /// Puts the first item you have in the last slot.
    /// </summary>
    public override void SwitchItem()
    {
        if (weaponList.Any())
        {
            weaponList.Enqueue(weaponList.Dequeue());
            DisplayItem();
        }
    }

    /// <summary>
    /// Fetches the _inventoryIndicator of the first item in the queue and makes it the texture of the displayed image.
    /// </summary>
    public override void DisplayItem() =>
        _inventoryIndicator.texture
        = weaponList.Count
        == 0
        ? emptyInventoryImage
        : weaponList.Peek().iconTexture;
}
