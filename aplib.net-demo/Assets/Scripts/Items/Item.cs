using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public float uses;
    // Start is called before the first frame update
    void Start()
    {
        uses = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UseItem()
    {
        uses -= 1;
    }


}
