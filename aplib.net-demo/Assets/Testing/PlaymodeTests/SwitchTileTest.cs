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
    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator SwitchTileTest()
    {
        // Arrange
        // Setup test object to access method switchTile in GridPlacer
        GameObject gameObject = new GameObject();
        gameObject.SetActive(false);
        
        // Define testing cells
        Cell corner = new(10, 10) { Tile = new Corner() };

        Cell crossing = new(10, 10) { Tile = new Crossing() };

        Cell deadEnd = new(10, 10) { Tile = new DeadEnd() };

        Cell room = new(10, 10)
        {
            Tile = new Room(
                new List<Direction> { Direction.North, Direction.East, Direction.South, Direction.West },
                new List<Direction> { Direction.North, Direction.East, Direction.South, Direction.West })
        };

        Cell straight = new(10, 10) { Tile = new Straight() };

        Cell tSection = new(10, 10) { Tile = new TSection() };

        // Act
        GridPlacer.SwitchTile(corner);
        GridPlacer.SwitchTile(crossing);
        GridPlacer.SwitchTile(deadEnd);
        GridPlacer.SwitchTile(room);
        GridPlacer.SwitchTile(straight);
        GridPlacer.SwitchTile(tSection);

        // Assert
        Assert.IsTrue(corner.Tile is StartCorner);
        Assert.IsTrue(crossing.Tile is StartCrossing);
        Assert.IsTrue(deadEnd.Tile is StartDeadEnd);
        Assert.IsTrue(room.Tile is StartRoom);
        Assert.IsTrue(straight.Tile is StartStraight);
        Assert.IsTrue(tSection.Tile is StartTSection);

        yield return null;
    }
}
