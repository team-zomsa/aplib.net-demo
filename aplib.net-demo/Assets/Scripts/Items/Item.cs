using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public float uses;
    public string itemName;
    public bool stackable;
    // Start is called before the first frame update
    void Start()
    {
        uses = 1;
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
            Debug.Log("Uses is " + uses + " it should not go below 0");
        }
    }


}
