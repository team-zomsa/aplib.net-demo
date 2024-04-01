using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public float uses;
    public string itemName;
    public bool stackable;
    private RawImage icon;
    public Texture iconTexture;
    /// <summary>
    /// Sets the uses and fetches the rawimage component
    /// </summary>
    void Start()
    {
        uses = 1;
        icon = GetComponent<RawImage>();
        icon.texture = iconTexture;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
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


}
