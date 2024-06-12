using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

public class KeyTests
{
    // A Test behaves as an ordinary method
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
        Assert.AreEqual(gameObject.GetComponent<Key>().Id, 10);
        Assert.AreEqual(gameObject.GetComponent<MeshRenderer>().material.color, Color.blue);
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
