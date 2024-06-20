using Entities.Weapons;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class WeaponAnimateTest
{
    [UnityTest]
    public IEnumerator WeaponAnimateTestWithEnumeratorPasses()
    {
        SceneManager.LoadScene("WeaponAnimateTestScene");
        yield return null;

        // Get the players crossbow
        var player = GameObject.Find("PlayerAnimated");
        RangedWeapon crossbow = player.GetComponentInChildren<RangedWeapon>();

        bool canBeAnimated = crossbow.CanFire();

        // Check if the crossbow can be animated
        Assert.IsTrue(canBeAnimated);
    }
}
