using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

public class ConfigurationTests
{
    [Test]
    public void GridSizeXZInvalidMinTest()
    {
        // Arrange
        GameObject gameObject = new GameObject();
        Configuration config = gameObject.AddComponent<Configuration>();
        config.TestingSwitch = true;

        // Act
        config.SetGridX("-2");
        config.SetGridZ("-2");

        // Assert
        Assert.AreEqual(config.MinGridSize, config.GridSizeX);
        Assert.AreEqual(config.MinGridSize, config.GridSizeZ);
    }

    [Test]
    public void GridSizeXZInvalidMaxTest()
    {
        // Arrange
        GameObject gameObject = new GameObject();
        Configuration config = gameObject.AddComponent<Configuration>();
        config.TestingSwitch = true;

        // Act
        config.SetGridX("1000");
        config.SetGridZ("1000");

        // Assert
        Assert.AreEqual(config.MaxGridSize, config.GridSizeX);
        Assert.AreEqual(config.MaxGridSize, config.GridSizeZ);
    }

    [Test]
    public void GridSizeXZValidTest()
    {
        // Arrange
        GameObject gameObject = new GameObject();
        Configuration config = gameObject.AddComponent<Configuration>();
        config.TestingSwitch = true;

        // Act
        config.SetGridX("5");
        config.SetGridZ("5");

        // Assert
        Assert.AreEqual(5, config.GridSizeX);
        Assert.AreEqual(5, config.GridSizeZ);
    }

    [Test]
    public void GridSizeXZEmptyStringTest()
    {
        // Arrange
        GameObject gameObject = new GameObject();
        Configuration config = gameObject.AddComponent<Configuration>();
        config.TestingSwitch = true;

        // Act
        config.SetGridX("");
        config.SetGridZ("");

        // Assert
        Assert.AreEqual(config.MinGridSize, config.GridSizeX);
        Assert.AreEqual(config.MinGridSize, config.GridSizeZ);
    }

    [Test]
    public void RoomValueInvalidTest()
    {
        // Arrange
        GameObject gameObject = new GameObject();
        Configuration config = gameObject.AddComponent<Configuration>();
        config.TestingSwitch = true;

        // Act
        config.SetValueRoom("");

        // Assert
        Assert.AreEqual(0, config.RoomAmount);
    }

    [Test]
    public void RoomValueInvalidMinTest()
    {
        // Arrange
        GameObject gameObject = new GameObject();
        Configuration config = gameObject.AddComponent<Configuration>();
        config.TestingSwitch = true;

        // Act
        config.SetValueRoom("-2");

        // Assert
        Assert.AreEqual(2, config.RoomAmount);
    }

    [Test]
    public void RoomValueInvalidMaxTest()
    {
        // Arrange
        GameObject gameObject = new GameObject();
        Configuration config = gameObject.AddComponent<Configuration>();
        config.TestingSwitch = true;

        // Act
        config.SetValueRoom("1000");

        // Assert
        Assert.AreEqual(7, config.RoomAmount);
    }

    [Test]
    public void RoomValueValidTest()
    {
        // Arrange
        GameObject gameObject = new GameObject();
        Configuration config = gameObject.AddComponent<Configuration>();
        config.TestingSwitch = true;

        // Act
        config.SetGridX("5");
        config.SetGridZ("5");
        config.SetValueRoom("5");

        // Assert
        Assert.AreEqual(5, config.RoomAmount);
    }

    [Theory]
    [TestCase("", 0)]
    [TestCase("-2", 2)]
    [TestCase("1000", 3)]
    public void ItemAmmoValueInvalidTest(string number, int expected)
    {
        // Arrange
        GameObject gameObject = new GameObject();
        Configuration config = gameObject.AddComponent<Configuration>();
        config.TestingSwitch = true;

        // Act
        config.SetValueItemAmmo(number);

        // Assert
        Assert.AreEqual(expected, config.ItemAmmoAmount);
    }

    [Test]
    public void ItemAmmoValueValidTest()
    {
        // Arrange
        GameObject gameObject = new GameObject();
        Configuration config = gameObject.AddComponent<Configuration>();
        config.TestingSwitch = true;

        // Act
        config.SetGridX("5");
        config.SetGridZ("5");
        config.SetValueItemAmmo("5");

        // Assert
        Assert.AreEqual(5, config.ItemAmmoAmount);
    }

    [Theory]
    [TestCase("", 0)]
    [TestCase("-2", 2)]
    [TestCase("1000", 3)]
    public void ItemHealthValueInvalidTest(string number, int expected)
    {
        // Arrange
        GameObject gameObject = new GameObject();
        Configuration config = gameObject.AddComponent<Configuration>();
        config.TestingSwitch = true;

        // Act
        config.SetValueItemHealth(number);

        // Assert
        Assert.AreEqual(expected, config.ItemHealthAmount);
    }

    [Test]
    public void ItemHealthValueValidTest()
    {
        // Arrange
        GameObject gameObject = new GameObject();
        Configuration config = gameObject.AddComponent<Configuration>();
        config.TestingSwitch = true;

        // Act
        config.SetGridX("5");
        config.SetGridZ("5");
        config.SetValueItemHealth("5");

        // Assert
        Assert.AreEqual(5, config.ItemHealthAmount);
    }

    [Theory]
    [TestCase("", 0)]
    [TestCase("-2", 2)]
    [TestCase("1000", 3)]
    public void ItemRageValueInvalidTest(string number, int expected)
    {
        // Arrange
        GameObject gameObject = new GameObject();
        Configuration config = gameObject.AddComponent<Configuration>();
        config.TestingSwitch = true;

        // Act
        config.SetValueItemRage(number);

        // Assert
        Assert.AreEqual(expected, config.ItemRageAmount);
    }

    [Test]
    public void ItemRageValueValidTest()
    {
        // Arrange
        GameObject gameObject = new GameObject();
        Configuration config = gameObject.AddComponent<Configuration>();
        config.TestingSwitch = true;

        // Act
        config.SetGridX("5");
        config.SetGridZ("5");
        config.SetValueItemRage("5");

        // Assert
        Assert.AreEqual(5, config.ItemRageAmount);
    }

    [Theory]
    [TestCase("", 0)]
    [TestCase("-2", 2)]
    [TestCase("1000", 3)]
    public void EnemyMeleeValueInvalidTest(string number, int expected)
    {
        // Arrange
        GameObject gameObject = new GameObject();
        Configuration config = gameObject.AddComponent<Configuration>();
        config.TestingSwitch = true;

        // Act
        config.SetValueEnemyMelee(number);

        // Assert
        Assert.AreEqual(expected, config.EnemyMeleeAmount);
    }

    [Test]
    public void EnemyMeleeValueValidTest()
    {
        // Arrange
        GameObject gameObject = new GameObject();
        Configuration config = gameObject.AddComponent<Configuration>();
        config.TestingSwitch = true;

        // Act
        config.SetGridX("5");
        config.SetGridZ("5");
        config.SetValueEnemyMelee("5");

        // Assert
        Assert.AreEqual(5, config.EnemyMeleeAmount);
    }

    [Theory]
    [TestCase("", 0)]
    [TestCase("-2", 2)]
    [TestCase("1000", 3)]
    public void EnemyRangedValueInvalidTest(string number, int expected)
    {
        // Arrange
        GameObject gameObject = new GameObject();
        Configuration config = gameObject.AddComponent<Configuration>();
        config.TestingSwitch = true;

        // Act
        config.SetValueEnemyRanged(number);

        // Assert
        Assert.AreEqual(expected, config.EnemyRangedAmount);
    }

    [Test]
    public void EnemyRangedValueValidTest()
    {
        // Arrange
        GameObject gameObject = new GameObject();
        Configuration config = gameObject.AddComponent<Configuration>();
        config.TestingSwitch = true;

        // Act
        config.SetGridX("5");
        config.SetGridZ("5");
        config.SetValueEnemyRanged("5");

        // Assert
        Assert.AreEqual(5, config.EnemyRangedAmount);
    }

    [Test]
    public void ItemOverflowInvalidTest()
    {
        // Arrange
        GameObject gameObject = new GameObject();
        Configuration config = gameObject.AddComponent<Configuration>();
        config.TestingSwitch = true;

        // Act
        config.SetGridX("5");
        config.SetGridZ("5");
        config.SetValueItemAmmo("25");

        // Assert
        Assert.AreEqual(6, config.ItemAmmoAmount);
        Assert.AreEqual(6, config.ItemHealthAmount);
        Assert.AreEqual(6, config.ItemRageAmount);
    }

    [Test]
    public void EnemyOverflowInvalidTest()
    {
        // Arrange
        GameObject gameObject = new GameObject();
        Configuration config = gameObject.AddComponent<Configuration>();
        config.TestingSwitch = true;

        // Act
        config.SetGridX("5");
        config.SetGridZ("5");
        config.SetValueEnemyMelee("25");

        // Assert
        Assert.AreEqual(6, config.EnemyMeleeAmount);
        Assert.AreEqual(6, config.EnemyRangedAmount);
    }
}
