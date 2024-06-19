using LevelGeneration;
using TMPro;
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
    public int MinGridSize { get { return 3; } }
    public int MaxGridSize { get { return 100; } }

    public int GridVolume { get; private set; }
    public int GridSizeX { get; private set; }
    public int GridSizeZ { get; private set; }

    // Room component.
    public int RoomAmount { get; private set; }

    // Item ammo.
    public int ItemAmmoAmount { get; private set; }

    // Item health.
    public int ItemHealthAmount { get; private set; }

    // Item rage.
    public int ItemRageAmount { get; private set; }

    // Enemy melee.
    public int EnemyMeleeAmount { get; private set; }

    // Enemy ranged.
    public int EnemyRangedAmount { get; private set; }

    // Total items.
    public int TotalItemCount { get; private set; }
    // Total enemies.
    public int TotalEnemyCount { get; private set; }

    // Enemy component variables.
    public bool TestingSwitch { get { return _test; } set { _test = value; } }
    private bool _test = false;

    // Input fields.
    public TMP_InputField inputFieldGridW;
    public TMP_InputField inputFieldGridH;
    public TMP_InputField inputFieldRoom;
    public TMP_InputField inputFieldItemAmmo;
    public TMP_InputField inputFieldItemHealth;
    public TMP_InputField inputFieldItemRage;
    public TMP_InputField inputFieldEnemyMelee;
    public TMP_InputField inputFieldEnemyRanged;

    private void Start()
    {
        // Initialize values.
        GridSizeX = 3;
        GridSizeZ = 3;
        GridVolume = GridSizeZ * GridSizeX;
        RoomAmount = 0;
        ItemAmmoAmount = 0;
        ItemHealthAmount = 0;
        ItemRageAmount = 0;
        EnemyMeleeAmount = 0;
        EnemyRangedAmount = 0;

        TotalItemCount = ItemAmmoAmount + ItemHealthAmount + ItemRageAmount;
        TotalEnemyCount = EnemyRangedAmount + EnemyMeleeAmount;
    }

    /// <summary>
    /// Check if input does not exceed the max or min boundry.
    /// Set valid input.
    /// </summary>
    /// <param name="value">Input given from the player through the input field</param>
    public void SetGridX(string value)
    {
        if (value == "") GridSizeX = MinGridSize;
        else GridSizeX = int.Parse(value);
        CheckAllValuesToBeValid();
    }

    /// <summary>
    /// Check if input does not exceed the max or min boundry.
    /// Set valid input.
    /// </summary>
    /// <param name="value">Input given from the player through the input field</param>
    public void SetGridZ(string value)
    {
        if (value == "") GridSizeZ = MinGridSize;
        else GridSizeZ = int.Parse(value);
        CheckAllValuesToBeValid();
    }

    /// <summary>
    /// Set inputfield amount to variable.
    /// Then call check to validate value.
    /// </summary>
    /// <param name="amount">Amount of rooms.</param>
    public void SetValueRoom(string amount)
    {
        if (amount == "") amount = "0";
        RoomAmount = int.Parse(amount);
        CheckAllValuesToBeValid();
    }

    /// <summary>
    /// Set inputfield amount to variable.
    /// Then call check to validate value.
    /// </summary>
    /// <param name="amount">Amount of ammo.</param>
    public void SetValueItemAmmo(string amount)
    {
        if (amount == "") amount = "0";
        ItemAmmoAmount = int.Parse(amount);
        CheckAllValuesToBeValid();
    }

    /// <summary>
    /// Set inputfield amount to variable.
    /// Then call check to validate value.
    /// </summary>
    /// <param name="amount">Amount of health potions.</param>
    public void SetValueItemHealth(string amount)
    {
        if (amount == "") amount = "0";
        ItemHealthAmount = int.Parse(amount);
        CheckAllValuesToBeValid();
    }

    /// <summary>
    /// Set inputfield amount to variable.
    /// Then call check to validate value.
    /// </summary>
    /// <param name="amount">Amount of rage potions.</param>
    public void SetValueItemRage(string amount)
    {
        if (amount == "") amount = "0";
        ItemRageAmount = int.Parse(amount);
        CheckAllValuesToBeValid();
    }

    /// <summary>
    /// Set inputfield amount to variable.
    /// Then call check to validate value.
    /// </summary>
    /// <param name="amount">Amount of melee enemies.</param>
    public void SetValueEnemyMelee(string amount)
    {
        if (amount == "") amount = "0";
        EnemyMeleeAmount = int.Parse(amount);
        CheckAllValuesToBeValid();
    }

    /// <summary>
    /// Set inputfield amount to variable.
    /// Then call check to validate value.
    /// </summary>
    /// <param name="amount">Amount of ranged enemies.</param>
    public void SetValueEnemyRanged(string amount)
    {
        if (amount == "") amount = "0";
        EnemyRangedAmount = int.Parse(amount);
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

        // Check room value.
        if (RoomAmount > GridVolume * _maxPercentage) RoomAmount = Mathf.CeilToInt(GridVolume * _maxPercentage);
        else if (RoomAmount < 0) RoomAmount = Mathf.CeilToInt(GridVolume * _minPercentage);

        // Check ItemAmmoAmount value.
        if (ItemAmmoAmount > GridVolume * _maxPercentage) ItemAmmoAmount = Mathf.CeilToInt(GridVolume * _maxPercentage);
        else if (ItemAmmoAmount < 0) ItemAmmoAmount = Mathf.CeilToInt(GridVolume * _minPercentage);

        // Check ItemHealthAmount value.
        if (ItemHealthAmount > GridVolume * _maxPercentage) ItemHealthAmount = Mathf.CeilToInt(GridVolume * _maxPercentage);
        else if (ItemHealthAmount < 0) ItemHealthAmount = Mathf.CeilToInt(GridVolume * _minPercentage);

        // Check ItemRageAmount value.
        if (ItemRageAmount > GridVolume * _maxPercentage) ItemRageAmount = Mathf.CeilToInt(GridVolume * _maxPercentage);
        else if (ItemRageAmount < 0) ItemRageAmount = Mathf.CeilToInt(GridVolume * _minPercentage);

        // Check EnemyMeleeAmount value.
        if (EnemyMeleeAmount > GridVolume * _maxPercentage) EnemyMeleeAmount = Mathf.CeilToInt(GridVolume * _maxPercentage);
        else if (EnemyMeleeAmount < 0) EnemyMeleeAmount = Mathf.CeilToInt(GridVolume * _minPercentage);

        // Check EnemyRangedAmount value.
        if (EnemyRangedAmount > GridVolume * _maxPercentage) EnemyRangedAmount = Mathf.CeilToInt(GridVolume * _maxPercentage);
        else if (EnemyRangedAmount < 0) EnemyRangedAmount = Mathf.CeilToInt(GridVolume * _minPercentage);

        // Check if items and enemies don't go over grid vol * _maxPrecentage(0.7f)
        CheckItemOverflow();
        CheckEnemyOverflow();

        // Print correct number in text field. Skip this when testing.
        if (!_test)
        {
            inputFieldGridW.text = GridSizeX.ToString();
            inputFieldGridH.text = GridSizeZ.ToString();
            inputFieldRoom.text = RoomAmount.ToString();
            inputFieldItemAmmo.text = ItemAmmoAmount.ToString();
            inputFieldItemHealth.text = ItemHealthAmount.ToString();
            inputFieldItemRage.text = ItemRageAmount.ToString();
            inputFieldEnemyMelee.text = EnemyMeleeAmount.ToString();
            inputFieldEnemyRanged.text = EnemyRangedAmount.ToString();

            SetValues();
        }
    }

    public void CheckItemOverflow()
    {
        TotalItemCount = ItemAmmoAmount + ItemHealthAmount + ItemRageAmount;
        if (TotalItemCount > GridVolume * _maxPercentage)
        {
            int maxCount = Mathf.CeilToInt(GridVolume * _maxPercentage / 3);

            TotalItemCount = Mathf.CeilToInt(GridVolume * _maxPercentage);
            ItemAmmoAmount = maxCount;
            ItemHealthAmount = maxCount;
            ItemRageAmount = maxCount;
        }
    }

    public void CheckEnemyOverflow()
    {
        TotalEnemyCount = EnemyRangedAmount + EnemyMeleeAmount;
        if (TotalEnemyCount > GridVolume * _maxPercentage)
        {
            int maxCount = Mathf.CeilToInt(GridVolume * _maxPercentage / 3);

            TotalItemCount = Mathf.CeilToInt(GridVolume * _maxPercentage);
            EnemyMeleeAmount = maxCount;
            EnemyRangedAmount = maxCount;
        }
    }

    private void SetValues()
    {
        // Set size and rooms.
        _gridConfig.GridWidthX = GridSizeX;
        _gridConfig.GridWidthZ = GridSizeZ;
        _gridConfig.AmountOfRooms = RoomAmount;

        // Set enemies.
        _spawnableEnemies.Enemies[0].Count = EnemyMeleeAmount;
        _spawnableEnemies.Enemies[1].Count = EnemyRangedAmount;

        // Set items.
        _spawnableItems.Items[0].Count = ItemAmmoAmount;
        _spawnableItems.Items[1].Count = ItemHealthAmount;
        _spawnableItems.Items[2].Count = ItemRageAmount;
    }
}
