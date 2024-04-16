public class Key : Item
{
    public int id;
    private void Start()
    {
        stackable = false;
        uses = 1;
        startUses = 1;
    }
}
