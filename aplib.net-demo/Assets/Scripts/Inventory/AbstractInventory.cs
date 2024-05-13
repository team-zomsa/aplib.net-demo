using UnityEngine;
using UnityEngine.UI;

public abstract class AbstractInventory : MonoBehaviour
{
    /// <summary>
    /// The RawImage is the object on which the _inventoryIndicator texture is projected.
    /// </summary>
    protected RawImage _inventoryIndicator;
    /// <summary>
    /// The texture of the _inventoryIndicator object.
    /// </summary>
    public Texture emptyInventoryImage;
    public GameObject inventoryObject;

    private void Start()
    {

    }

    public virtual void PickUpItem(Item item)
    {

    }
    public virtual void PickUpWeapon(Weapon weapon)
    {

    }

    public virtual void ActivateItem()
    {

    }

    public virtual void SwitchItem()
    {

    }

    public virtual void DisplayItem()
    {

    }
}
