public class Key : Item
{
    /// <summary>
    /// The key ID, to check which door it can open
    /// </summary>
    public int Id { get; private set; }
    
    /// <summary>
    /// the constructor that gives the key its Id
    /// </summary>
    /// <param name="constrId">the Id the key is given</param>
    public Key(int constrId) => id = constrId;
    private void Start()
    {
        stackable = false;
        uses = 1;
        startUses = 1;
    }
}
