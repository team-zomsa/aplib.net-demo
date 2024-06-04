using Entities.Weapons;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class RangedWeaponTest
{
    [UnityTest]
    public IEnumerator TestAmmo()
    {
        SceneManager.LoadScene("RangedEnemyTestScene");
        yield return null;

        // Get the players crossbow
        var player = GameObject.Find("Player");
        Equipment crossbow = player.GetComponentInChildren<Entities.Weapons.RangedWeapon>();
        var ammoPouch = player.GetComponentInChildren<AmmoPouch>();

        // Shoot the crossbow 10 times, but miss the shots
        for (int i = 0; i < 10; i++)
        {
            crossbow.UseEquipment();
            yield return null;
        }

        // Try to use the crossbow one more time and check if it fails
        crossbow.UseEquipment();
        yield return null;

        // Check if the player has no ammo left
        bool hasAmmo = ammoPouch.TryUseAmmo();
        Assert.IsFalse(hasAmmo);
    }

    [UnityTest]
    public IEnumerator TestIfDisabledWhenAmmoPouchNotPresent()
    {
        SceneManager.LoadScene("RangedWeaponBroken");
        yield return null;

        LogAssert.Expect(LogType.Error, "AmmoPouch not found in the scene. Defaulting to parent ammo pouch.");
        LogAssert.Expect(LogType.Error, "No parent ammo pouch found. Disabling ranged weapon.");

        // Get the players crossbow
        var player = GameObject.Find("Player Ranged");
        Equipment crossbow = player.GetComponentInChildren<Entities.Weapons.RangedWeapon>();

        yield return null;

        // Assert
        Assert.IsFalse(crossbow.enabled);
    }
}
