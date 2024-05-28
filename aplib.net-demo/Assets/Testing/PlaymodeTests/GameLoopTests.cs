using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class GameLoopTests
{
    /// <summary>
    /// A test that checks if the game is playing after the GameManager is instantiated.
    /// </summary>
    [UnityTest]
    public IEnumerator WhenGameManagerInstantiatedThenGameIsPlaying()
    {
        // Arrange
        GameObject gameObject = new();
        Time.timeScale = 0f;

        // Act
        gameObject.AddComponent<GameManager>();

        yield return null;

        // Assert
        Assert.AreEqual(1f, Time.timeScale);

        yield break;
    }
}
