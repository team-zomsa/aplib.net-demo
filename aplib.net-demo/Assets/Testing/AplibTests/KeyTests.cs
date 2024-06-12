using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

public class KeyTests
{
    /// <summary>
    /// Tests start function and variables of the key.
    /// Also tests id the initialize method works correctly.
    /// </summary>
    [Test]
    public void KeyBaseFunctionTest()
    {
        // Arrange
        GameObject gameObject = new GameObject();
        gameObject.AddComponent<Key>();
        gameObject.AddComponent<MeshRenderer>();

        // Act
        gameObject.GetComponent<Key>().Initialize(10, Color.blue);

        // Assert
        Assert.IsFalse(gameObject.GetComponent<Key>().stackable);
        Assert.AreEqual(0, gameObject.GetComponent<Key>().uses);
        Assert.AreEqual(1, gameObject.GetComponent<Key>().usesAddedPerPickup);
        Assert.AreEqual(10, gameObject.GetComponent<Key>().Id);
        Assert.AreEqual(Color.blue, gameObject.GetComponent<MeshRenderer>().material.color);
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator KeyTestsWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
