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

    /// <summary>
    /// A test that checks if the game is paused after the pause method is called.
    /// </summary>
    [UnityTest]
    public IEnumerator WhenGameManagerPauseCalledThenGameIsPaused()
    {
        // Arrange
        GameObject gameObject = new();
        gameObject.AddComponent<GameManager>();
        Time.timeScale = 1f;

        yield return null;

        // Act
        GameManager.Instance.Pause();

        // Assert
        Assert.AreEqual(0f, Time.timeScale);

        yield break;
    }
}
