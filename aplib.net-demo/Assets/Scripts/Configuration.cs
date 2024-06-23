using LevelGeneration;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Plastic.Newtonsoft.Json.Linq;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;
using UnityEngine.UI;

public class Configuration : MonoBehaviour
{
    [SerializeField] private const float _maxPercentage = 0.7f;
    [SerializeField] private const float _minPercentage = 0.2f;

    [SerializeField] private SpawnableEnemies _spawnableEnemies;
    [SerializeField] private SpawnableItems _spawnableItems;
    [SerializeField] private GridConfig _gridConfig;

    // Grid component variables.
    public int MinGridSize => 3;
    public int MaxGridSize => 100;

    public int GridVolume { get; private set; }

    [SerializeField]
    public int GridSizeX { get; private set; } = 3;
    [SerializeField]
    public int GridSizeZ { get; private set; } = 3;

    // Room component.
    [SerializeField]
    public int RoomAmount { get; private set; } = 0;

    // Item ammo.
    [SerializeField]
    public int AmmoItemAmount { get; private set; } = 0;

    // Item health.
    [SerializeField]
    public int HealthItemAmount { get; private set; } = 0;

    // Item rage.
    [SerializeField]
    public int RageItemAmount { get; private set; } = 0;

    // Enemy melee.
    [SerializeField]
    public int MeleeEnemyAmount { get; private set; } = 0;

    // Enemy ranged.
    [SerializeField]
    public int RangedEnemyAmount { get; private set; } = 0;

    /// <summary>
    /// Max total items that can excist in 1 maze.
    /// </summary>
    public int TotalItemCount { get; private set; }
    /// <summary>
    /// Max total enemies that can excist in 1 maze.
    /// </summary>
    public int TotalEnemyCount { get; private set; }

    // Disables TMP fields so unity can run the unit tests.
    public bool TestingSwitch { get; set; }

    // Input fields.
    public TMP_InputField inputFieldXGrid;
    public TMP_InputField inputFieldZGrid;
    public TMP_InputField inputFieldRoom;
    public TMP_InputField inputFieldAmmoItem;
    public TMP_InputField inputFieldHealthItem;
    public TMP_InputField inputFieldRageItem;
    public TMP_InputField inputFieldMeleeEnemy;
    public TMP_InputField inputFieldRangedEnemy;

    private void Start()
    {
        // Initialize values.
        GridVolume = GridSizeZ * GridSizeX;

        TotalItemCount = AmmoItemAmount + HealthItemAmount + RageItemAmount;
        TotalEnemyCount = RangedEnemyAmount + MeleeEnemyAmount;

        CheckAllValuesToBeValid();
    }

    /// <summary>
    /// Check if input does not exceed the max or min boundry.
    /// Set valid input.
    /// </summary>
    /// <param name="value">Input given from the player through the input field</param>
    public void SetGridX(string value)
    {
        GridSizeX = value == string.Empty ? MinGridSize : int.Parse(value);
        CheckAllValuesToBeValid();
    }

    /// <summary>
    /// Check if input does not exceed the max or min boundry.
    /// Set valid input.
    /// </summary>
    /// <param name="value">Input given from the player through the input field</param>
    public void SetGridZ(string value)
    {
        GridSizeZ = value == string.Empty ? MinGridSize : int.Parse(value);
        CheckAllValuesToBeValid();
    }

    /// <summary>
    /// Set inputfield amount to variable.
    /// Then call check to validate value.
    /// </summary>
    /// <param name="amount">Amount of rooms.</param>
    public void SetValueRoom(string amount)
    {
        RoomAmount = amount == string.Empty ? 0 : int.Parse(amount);
        CheckAllValuesToBeValid();
    }

    /// <summary>
    /// Set inputfield amount to variable.
    /// Then call check to validate value.
    /// </summary>
    /// <param name="amount">Amount of ammo.</param>
    public void SetValueItemAmmo(string amount)
    {
        AmmoItemAmount = amount == string.Empty ? 0 : int.Parse(amount);
        CheckAllValuesToBeValid();
    }

    /// <summary>
    /// Set inputfield amount to variable.
    /// Then call check to validate value.
    /// </summary>
    /// <param name="amount">Amount of health potions.</param>
    public void SetValueItemHealth(string amount)
    {
        HealthItemAmount = amount == string.Empty ? 0 : int.Parse(amount);
        CheckAllValuesToBeValid();
    }

    /// <summary>
    /// Set inputfield amount to variable.
    /// Then call check to validate value.
    /// </summary>
    /// <param name="amount">Amount of rage potions.</param>
    public void SetValueItemRage(string amount)
    {
        RageItemAmount = amount == string.Empty ? 0 : int.Parse(amount);
        CheckAllValuesToBeValid();
    }

    /// <summary>
    /// Set inputfield amount to variable.
    /// Then call check to validate value.
    /// </summary>
    /// <param name="amount">Amount of melee enemies.</param>
    public void SetValueEnemyMelee(string amount)
    {
        MeleeEnemyAmount = amount == string.Empty ? 0 : int.Parse(amount);
        CheckAllValuesToBeValid();
    }

    /// <summary>
    /// Set inputfield amount to variable.
    /// Then call check to validate value.
    /// </summary>
    /// <param name="amount">Amount of ranged enemies.</param>
    public void SetValueEnemyRanged(string amount)
    {
        RangedEnemyAmount = amount == string.Empty ? 0 : int.Parse(amount);
        CheckAllValuesToBeValid();
    }

    /// <summary>
    /// This is called when the grid values are changed.
    /// This is also called when an individual component has changed.
    /// It updates all other conponents.
    /// </summary>
    private void CheckAllValuesToBeValid()
    {
        // Check gridW and gridH
        if (GridSizeX > MaxGridSize) GridSizeX = MaxGridSize;
        else if (GridSizeX < MinGridSize) GridSizeX = MinGridSize;

        if (GridSizeZ > MaxGridSize) GridSizeZ = MaxGridSize;
        else if (GridSizeZ < MinGridSize) GridSizeZ = MinGridSize;

        // Update value.
        GridVolume = GridSizeX * GridSizeZ;

        int minValue = Mathf.CeilToInt(GridVolume * _minPercentage);
        int maxValue = Mathf.CeilToInt(GridVolume * _maxPercentage);

        // Check room value.
        RoomAmount = Mathf.Max(minValue, Mathf.Min(maxValue, RoomAmount));

        // Check ItemAmmoAmount value.
        AmmoItemAmount = Mathf.Max(minValue, Mathf.Min(maxValue, AmmoItemAmount));

        // Check ItemHealthAmount value.
        HealthItemAmount = Mathf.Max(minValue, Mathf.Min(maxValue, HealthItemAmount));

        // Check ItemRageAmount value.
        RageItemAmount = Mathf.Max(minValue, Mathf.Min(maxValue, RageItemAmount));

        // Check EnemyMeleeAmount value.
        MeleeEnemyAmount = Mathf.Max(minValue, Mathf.Min(maxValue, MeleeEnemyAmount));

        // Check EnemyRangedAmount value.
        RangedEnemyAmount = Mathf.Max(minValue, Mathf.Min(maxValue, RangedEnemyAmount));

        // Check if items and enemies don't go over grid vol * _maxPrecentage(0.7f)
        CheckItemOverflow();
        CheckEnemyOverflow();

        // Print correct number in text field. Skip this when testing.
        if (!TestingSwitch)
        {
            inputFieldXGrid.text = GridSizeX.ToString();
            inputFieldZGrid.text = GridSizeZ.ToString();
            inputFieldRoom.text = RoomAmount.ToString();
            inputFieldAmmoItem.text = AmmoItemAmount.ToString();
            inputFieldHealthItem.text = HealthItemAmount.ToString();
            inputFieldRageItem.text = RageItemAmount.ToString();
            inputFieldMeleeEnemy.text = MeleeEnemyAmount.ToString();
            inputFieldRangedEnemy.text = RangedEnemyAmount.ToString();

            UpdateConfigValuesInScriptableScript();
        }
    }

    public void CheckItemOverflow()
    {
        TotalItemCount = AmmoItemAmount + HealthItemAmount + RageItemAmount;
        if (TotalItemCount > GridVolume * _maxPercentage)
        {
            int maxCount = Mathf.CeilToInt(GridVolume * _maxPercentage / 3);

            TotalItemCount = Mathf.CeilToInt(GridVolume * _maxPercentage);
            AmmoItemAmount = maxCount;
            HealthItemAmount = maxCount;
            RageItemAmount = maxCount;
        }
    }

    public void CheckEnemyOverflow()
    {
        TotalEnemyCount = RangedEnemyAmount + MeleeEnemyAmount;
        if (TotalEnemyCount > GridVolume * _maxPercentage)
        {
            int maxCount = Mathf.CeilToInt(GridVolume * _maxPercentage / 3);

            TotalItemCount = Mathf.CeilToInt(GridVolume * _maxPercentage);
            MeleeEnemyAmount = maxCount;
            RangedEnemyAmount = maxCount;
        }
    }

    /// <summary>
    /// This method updates all 3 scriptable scripts for the grid, items and enemies values.
    /// Doing this the level generation can use these value to generate.
    /// </summary>
    private void UpdateConfigValuesInScriptableScript()
    {
        // Set size and rooms.
        _gridConfig.GridWidthX = GridSizeX;
        _gridConfig.GridWidthZ = GridSizeZ;
        _gridConfig.AmountOfRooms = RoomAmount;

        // Set enemies.
        _spawnableEnemies.Enemies[0].Count = MeleeEnemyAmount;
        _spawnableEnemies.Enemies[1].Count = RangedEnemyAmount;

        // Set items.
        _spawnableItems.Items[0].Count = AmmoItemAmount;
        _spawnableItems.Items[1].Count = HealthItemAmount;
        _spawnableItems.Items[2].Count = RageItemAmount;
    }
}
