using NUnit.Framework;
using UnityEngine;

public class KeyUnitTest
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
}
