using Assets.Scripts.Doors;

public class DoorPointsAdder : PointsAdderComponent
{
    private void Awake()
    {
        Door door = GetComponent<Door>();
        door.DoorOpened += SendPoints;
    }
}
