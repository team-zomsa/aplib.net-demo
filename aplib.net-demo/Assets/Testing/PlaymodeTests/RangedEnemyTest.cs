using Entities.Weapons;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
public class RangedEnemyTest
{
    [SetUp]
    public void SetUp()
    {
        SceneManager.LoadScene("RangedEnemyTestScene");
    }

    [UnityTest]
    public IEnumerator TestAmmo()
    {
        // Get the players crossbow
        var player = GameObject.Find("Player");
        Equipment crossbow = player.GetComponentInChildren<RangedWeapon>();
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
}
