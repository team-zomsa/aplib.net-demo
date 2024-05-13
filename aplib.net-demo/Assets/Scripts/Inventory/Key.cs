public class Key : Item
{
    /// <summary>
    /// The key ID, to check which door it can open.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// Initializes a key with a given ID.
    /// </summary>
    /// <param name="id">The ID the key is given.</param>
    public Key(int id) => Id = id;

    private void Start()
    {
        stackable = false;
        uses = 1;
        startUses = 1;
    }
}
