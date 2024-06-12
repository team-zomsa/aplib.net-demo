using Assets.Scripts.Items;
using Entities.Weapons;
using NUnit.Framework;
using System;
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

        // Calculate the expected damage increase
        int damageIncrease = activeWeapon.Damage * ragePotion.DamageIncreasePercentage / 100;
        int expectedDamage = activeWeapon.Damage + damageIncrease;

        // Use the rage potion
        ragePotion.UseItem();

        // Check if the player's damage has increased
        Assert.AreEqual(activeWeapon.Damage, expectedDamage);
    }

    /// <summary>
    /// Test if the player's damage returns to normal after the rage potion effect is over.
    /// </summary>
    [UnityTest]
    public IEnumerator ReturnsToNormalAfterTimer()
    {
        SceneManager.LoadScene("RagePotionTestScene");
        yield return null;

        // Get the player object
        var player = GameObject.Find("Player");
        EquipmentInventory equipmentInventory = player.GetComponentInChildren<EquipmentInventory>();
        Weapon activeWeapon = equipmentInventory.CurrentEquipment as Weapon;
        int playerDamageBefore = activeWeapon.Damage;

        // Get the rage potion
        GameObject ragePotionObj = GameObject.Find("RagePotion");
        RagePotion ragePotion = ragePotionObj.GetComponent<RagePotion>();

        // Use the rage potion
        ragePotion.GetComponent<RagePotion>().UseItem();

        // Wait for the timer to finish
        yield return new WaitForSeconds(ragePotion.Duration);

        // Check if the player's damage has returned to normal
        Assert.AreEqual(activeWeapon.Damage, playerDamageBefore);
    }
}
