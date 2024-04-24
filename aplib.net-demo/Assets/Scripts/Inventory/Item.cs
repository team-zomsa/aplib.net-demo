using UnityEngine;
using UnityEngine.UI;
/// <summary>
///  This is where you change variables that are shared between all the items. The values themselves you want to change in the prefab for the specific item you want.
///  To create a new item: Create a new script that inherits from this one, and then make a prefab that has that script attached.
/// </summary>
public abstract class Item : MonoBehaviour
{

    public float uses;
    public float startUses;
    public bool stackable;
    // The RawImage is the object on which the _inventoryIndicator texture is projected
    protected RawImage icon;
    public Texture iconTexture;

    /// <summary>
    /// Uses the item, by default just reduces uses by 1. Implementation will differ depending on the item itself.
    /// </summary>
    public void UseItem() => uses -= 1;

    /// <summary>
    /// Resets the uses to the starting uses.
    /// </summary>
    public void Reset()
        => uses = startUses;
}
