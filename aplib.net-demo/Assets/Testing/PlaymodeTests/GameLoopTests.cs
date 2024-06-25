using NUnit.Framework;
using System.Collections;
using UnityEngine;
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
        Time.timeScale = 1f;
    }

    [TearDown]
    public void TearDown() => Time.timeScale = 1f;
}
