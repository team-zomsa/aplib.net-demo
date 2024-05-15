public class Key : Item
{
    /// <summary>
    /// The key ID, to check which door it can open.
    /// </summary>
    public int Id;

    private void Start()
    {
        stackable = false;
        uses = 1;
        usesAddedPerPickup = 1;
    }
}
