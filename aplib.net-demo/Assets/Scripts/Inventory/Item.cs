using UnityEngine;
using UnityEngine.UI;

/// <summary>
///  This is where you change variables that are shared between all the items. The values themselves you want to change in the prefab for the specific item you want.
///  To create a new item: Create a new script that inherits from this one, and then make a prefab that has that script attached.
/// </summary>
public abstract class Item : MonoBehaviour
{
    /// <summary>
    /// Since the inventory works by comparing the name of the item,
    /// all instances of the same item should have the same name.
    /// </summary>
    [SerializeField]
    protected string _itemName;

    /// <summary>
    /// The amount of uses the item currently has.
    /// </summary>
    public float uses;

    /// <summary>
    /// The amount of uses that get added to the item when it is picked up.
    /// </summary>
    public float usesAddedPerPickup;

    /// <summary>
    /// If the item is stackable, it will add uses to the item if it is already in the inventory.
    /// </summary>
    public bool stackable;

    /// <summary>
    /// The RawImage is the object on which the _inventoryIndicator texture is projected.
    /// </summary>
    protected RawImage icon;

    /// <summary>
    /// The texture the _inventoryIndicator object should have in the inventory.
    /// </summary>
    public Texture iconTexture;

    protected virtual void Awake() => gameObject.name = _itemName;

    /// <summary>
    /// Uses the item, by default just reduces uses by 1. Implementation will differ depending on the item itself.
    /// </summary>
    public virtual void UseItem() => uses -= 1;

    /// <summary>
    /// Resets the uses to the starting uses.
    /// </summary>
    public virtual void Reset()
        => uses = usesAddedPerPickup;
}
