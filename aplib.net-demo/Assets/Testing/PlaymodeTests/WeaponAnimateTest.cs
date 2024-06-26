// This program has been developed by students from the bachelor Computer Science at Utrecht
// University within the Software Project course.
// Copyright Utrecht University (Department of Information and Computing Sciences)

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
