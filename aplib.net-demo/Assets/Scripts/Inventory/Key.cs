public class Key : Item
{
    /*
    todo: flesh out the pickup function with picking up keys
    change the door collision to check the list in the inventory (make inventory static?)
    find key image (see through?) 
    create key display on top left of screen with number
    add number to doors
     */
    public int id;
    private void Start()
    {
        stackable = false;
        uses = 1;
        startUses = 1;
    }
}
