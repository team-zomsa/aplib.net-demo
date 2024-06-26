// This program has been developed by students from the bachelor Computer Science at Utrecht
// University within the Software Project course.
// Copyright Utrecht University (Department of Information and Computing Sciences)

using Assets.Scripts.Tiles;
using LevelGeneration;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using WFC;

public class SetStartTilePropertyTrueTests
{
    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator SetStartTilePropertyTrueTest()
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
        LevelSpawner.UpdateTileModel(corner);
        LevelSpawner.UpdateTileModel(crossing);
        LevelSpawner.UpdateTileModel(deadEnd);
        LevelSpawner.UpdateTileModel(room);
        LevelSpawner.UpdateTileModel(straight);
        LevelSpawner.UpdateTileModel(tSection);

        // Assert
        Assert.IsTrue(corner.Tile.IsStart);
        Assert.IsTrue(crossing.Tile.IsStart);
        Assert.IsTrue(deadEnd.Tile.IsStart);
        Assert.IsTrue(room.Tile.IsStart);
        Assert.IsTrue(straight.Tile.IsStart);
        Assert.IsTrue(tSection.Tile.IsStart);

        yield return null;
    }
}
