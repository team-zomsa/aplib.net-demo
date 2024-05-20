using UnityEngine;

public class Key : Item
{
    /// <summary>
    /// The key ID, to check which door it can open.
    /// </summary>
    public int Id;

    /// <summary>
    /// Set the key id and color.
    /// </summary>
    /// <param name="id">The key id, which should match to the door id.</param>
    /// <param name="color">The color of the key, which should match to the door color.</param>
    public void Initialize(int id, Color color)
    {
        Id = id;
        GetComponent<Renderer>().material.color = color;
    }

    private void Start()
    {
        stackable = false;
        uses = 1;
        usesAddedPerPickup = 1;
    }
}
