using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{

    //This is where you change variables that are shared between all the items. The values themselves you want to change in the prefab for the specific item you want.
    //To create a new item: Create a new script that inherits from this one, and then make a prefab that has that script attached.
    public float uses;
    public float startUses;
    public string itemName;
    public bool stackable;
    protected RawImage icon;
    public Texture iconTexture;


    /// <summary>
    /// Uses the item, by default just reduces uses by 1. Implementation will differ depending on the item itself.
    /// </summary>
   public void UseItem()
    {
        uses -= 1;
        if (uses < 0)
        {
            Debug.LogError("Uses is " + uses + " it should not go below 0");
        }
    }
    /// <summary>
    /// Resets the uses to the starting uses, doing it this way means we get reuse items without affecting performance
    /// </summary>
    public void Reset()
    {
        uses = startUses;
    }
    


}
