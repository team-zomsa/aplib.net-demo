using Assets.Scripts.Tiles;
using Assets.Scripts.Wfc;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class SwitchTileTests
{
    [SetUp]
    public void Setup()
    {
        SceneManager.LoadScene("SwitchTileTest");
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator SwitchTileTest()
    {
        // Arrange
        GameObject gameObject = new GameObject();
        GridPlacer grid = gameObject.GetComponent<GridPlacer>();


        Cell randomTile = new Cell(10, 10);
        randomTile.Tile = new Corner(Direction.North);

        // Act
        grid.SwitchTile(randomTile);

        // Assert
        Assert.IsTrue(randomTile.Tile is StartCorner);

        yield return null;
    }
}
