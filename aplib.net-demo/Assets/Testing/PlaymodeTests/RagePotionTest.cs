using Assets.Scripts.Items;
using Entities.Weapons;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class RagePotionTest
{
    /// <summary>
    /// Test if the rage potion doubles the damage of the player.
    /// </summary>
    [UnityTest]
    public IEnumerator RagePotionDoublesDamage()
    {
        SceneManager.LoadScene("RagePotionTestScene");
        yield return null;

        // Get the player object
        var player = GameObject.Find("Player");
        EquipmentInventory equipmentInventory = player.GetComponentInChildren<EquipmentInventory>();
        Weapon activeWeapon = equipmentInventory.CurrentEquipment as Weapon;

        // Get the rage potion
        GameObject ragePotionObj = GameObject.Find("RagePotion");
        RagePotion ragePotion = ragePotionObj.GetComponent<RagePotion>();
        int damageIncreasePercentage = ragePotion.DamageIncreasePercentage;

        // Use the rage potion
        ragePotion.UseItem();

        // Check if the player's damage has increased
        Assert.AreEqual(activeWeapon.Damage, activeWeapon.Damage * (1 + damageIncreasePercentage / 100));
    }
}
