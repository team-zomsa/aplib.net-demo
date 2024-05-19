using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class WeaponInventory : AbstractInventory
{
    public Queue<Weapon> weaponList;

    public Weapon _meleeWeapon;
    public Weapon _rangedWeapon;
    public void Start()
    {

        //todo different button for weapon and item switch
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
    /// <summary>
    /// Picks up a weapon and puts it in the inventory.
    /// </summary>
    /// <param name="weapon"></param>
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
    }/// <summary>
    /// Uses the equipped weapon.
    /// </summary>
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
    /// Fetches the _inventoryIndicator of the first item in the queue and makes it the texture of the displayed image. Also displays the model of the currently equipped weapon.
    /// </summary>
    public override void DisplayItem()
    {
        UnityEngine.Debug.Log("first item in inv is" + weaponList.Peek()) ;

        _inventoryIndicator.texture
        = weaponList.Count
        == 0
        ? emptyInventoryImage
        : weaponList.Peek().iconTexture;
        bool firstWeapon = true;

        foreach (Weapon weapon in weaponList.ToList<Weapon>())
        {
            MeshRenderer mesh = weapon.gameObject.GetComponentInChildren<MeshRenderer>();

            if (firstWeapon)
            {
                UnityEngine.Debug.Log("setting to active " + weapon);
                UnityEngine.Debug.Log("weapon was" +weapon.gameObject.activeSelf);
                weapon.gameObject.SetActive(true);
                UnityEngine.Debug.Log("weapon is now" + weapon.gameObject.activeSelf);
               // mesh.enabled = true;
                firstWeapon = false;
            }
            else if (firstWeapon == false)
            {
                UnityEngine.Debug.Log("setting to inactive " + weapon);

                UnityEngine.Debug.Log("weapon was" + weapon.gameObject.activeSelf);


                weapon.gameObject.SetActive(false); UnityEngine.Debug.Log(weapon.gameObject.activeSelf);
                UnityEngine.Debug.Log("weapon is now" + weapon.gameObject.activeSelf);
               // mesh.enabled = false;
            }
        }
        
    }
}
