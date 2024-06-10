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
        //      Setup test object to access method switchTile in GridPlacer
        GameObject gameObject = new GameObject();
        gameObject.SetActive(false);
        GridPlacer gridPlacer = gameObject.AddComponent<GridPlacer>();

        //      Define testing cells
        Cell corner = new Cell(10, 10);
        corner.Tile = new Corner(Direction.North);

        Cell crossing = new Cell(10, 10);
        crossing.Tile = new Crossing();

        Cell deadEnd = new Cell(10, 10);
        deadEnd.Tile = new DeadEnd(Direction.North);

        Cell room = new Cell(10, 10);
        room.Tile = new Room(
                        new List<Direction>() { Direction.North, Direction.East, Direction.South, Direction.West },
                        new List<Direction>() { Direction.North, Direction.East, Direction.South, Direction.West });

        Cell straight = new Cell(10, 10);
        straight.Tile = new Straight(Direction.North);

        Cell tSection = new Cell(10, 10);
        tSection.Tile = new TSection(Direction.North);

        // Act
        gridPlacer.SwitchTile(corner);
        gridPlacer.SwitchTile(crossing);
        gridPlacer.SwitchTile(deadEnd);
        gridPlacer.SwitchTile(room);
        gridPlacer.SwitchTile(straight);
        gridPlacer.SwitchTile(tSection);

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
