using Assets.Scripts.Items;
using Entities.Weapons;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class RagePotionTest
{
    [SetUp]
    public void SetUp()
    {
        Debug.Log($"Starting test {nameof(RagePotionTest)}");
        SceneManager.LoadScene("RagePotionTestScene");
    }

    /// <summary>
    /// Test if the rage potion doubles the damage of the player.
    /// </summary>
    [UnityTest]
    public IEnumerator RagePotionDoublesDamage()
    {
        // Get the player object
        GameObject player = GameObject.Find("Player");
        EquipmentInventory equipmentInventory = player.GetComponentInChildren<EquipmentInventory>();
        Weapon activeWeapon = (equipmentInventory.CurrentEquipment as Weapon)!;

        // Get the rage potion
        GameObject ragePotionObj = GameObject.Find("RagePotion");
        RagePotion ragePotion = ragePotionObj.GetComponent<RagePotion>();

        // Calculate the expected damage increase
        int damageIncrease = activeWeapon.Damage * ragePotion.DamageIncreasePercentage / 100;
        int expectedDamage = activeWeapon.Damage + damageIncrease;

        // Use the rage potion
        ragePotion.UseItem();

        yield return null;

        // Check if the player's damage has increased
        Assert.AreEqual(expectedDamage, activeWeapon.Damage);
    }

    /// <summary>
    /// Test if the player's damage returns to normal after the rage potion effect is over.
    /// </summary>
    [UnityTest]
    public IEnumerator ReturnsToNormalAfterTimer()
    {
        // Get the player object
        GameObject player = GameObject.Find("Player");
        EquipmentInventory equipmentInventory = player.GetComponentInChildren<EquipmentInventory>();
        Weapon activeWeapon = (equipmentInventory.CurrentEquipment as Weapon)!;
        int playerDamageBefore = activeWeapon.Damage;

        // Get the rage potion
        GameObject ragePotionObj = GameObject.Find("RagePotion");
        RagePotion ragePotion = ragePotionObj.GetComponent<RagePotion>();

        // Use the rage potion
        ragePotion.GetComponent<RagePotion>().UseItem();
        Debug.Log("Rage potion duration: " + ragePotion.Duration);

        // Wait for the timer to finish
        yield return new WaitForSeconds(ragePotion.Duration + 0.01f);

        // Check if the player's damage has returned to normal
        Assert.AreEqual(playerDamageBefore, activeWeapon.Damage);
    }
}
