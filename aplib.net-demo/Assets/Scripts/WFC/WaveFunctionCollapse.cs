using System.Collections.Generic;
using ThreadSafeRandom;

namespace Assets.Scripts.Wfc
{
    public class WaveFunctionCollapse
    {
        private readonly int _widthX;

        private readonly int _widthY;

        public WaveFunctionCollapse(int widthX, int widthY)
        {
            _widthX = widthX;
            _widthY = widthY;
        }

        public Grid Grid { get; private set; }

        public void Init()
        {
            Grid = new Grid(_widthX, _widthY);
            Grid.Init();
        }

        public void PlaceRandomRooms(int amountOfRooms)
        {
            int numberOfRooms = 0;

            while (numberOfRooms < amountOfRooms)
            {
                Grid.PlaceRandomRoom();
                numberOfRooms++;
            }
        }

        public void Run()
        {
            while (!Grid.IsFullyCollapsed())
            {
                List<Cell> lowestEntropyCells = Grid.GetLowestEntropyCells();

                int index = SharedRandom.Next(lowestEntropyCells.Count);

                Cell cell = lowestEntropyCells[index];
                cell.Tile = cell.Candidates[SharedRandom.Next(cell.Candidates.Count)];
                cell.Candidates.Clear();

                Grid.RemoveUnconnectedNeighbourCandidates(cell);
            }
        }
    }
}
