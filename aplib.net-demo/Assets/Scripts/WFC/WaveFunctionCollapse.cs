// This program has been developed by students from the bachelor Computer Science at Utrecht
// University within the Software Project course.
// Copyright Utrecht University (Department of Information and Computing Sciences)

using System.Collections.Generic;
using ThreadSafeRandom;

namespace WFC
{
    /// <summary>
    /// The wave function collapse algorithm.
    /// </summary>
    public class WaveFunctionCollapse
    {
        private readonly int _widthX;

        private readonly int _widthY;

        private Grid _grid;

        private WaveFunctionCollapse(int widthX, int widthY)
        {
            _widthX = widthX;
            _widthY = widthY;
        }

        public static Grid GenerateGrid(int widthX, int widthY, int amountOfRooms)
        {
            WaveFunctionCollapse wfc = new(widthX, widthY);

            wfc.Init();
            wfc.PlaceRandomRooms(amountOfRooms);
            wfc.Run();

            return wfc._grid;
        }

        private void Init()
        {
            _grid = new Grid(_widthX, _widthY);
            _grid.Init();
        }

        private void PlaceRandomRooms(int amountOfRooms)
        {
            int numberOfRooms = 0;

            while (numberOfRooms < amountOfRooms)
            {
                _grid.PlaceRandomRoom();
                numberOfRooms++;
            }
        }

        private void Run()
        {
            while (!_grid.IsFullyCollapsed())
            {
                List<Cell> lowestEntropyCells = _grid.GetLowestEntropyCells();

                int index = SharedRandom.Next(lowestEntropyCells.Count);

                Cell cell = lowestEntropyCells[index];
                cell.Tile = cell.Candidates[SharedRandom.Next(cell.Candidates.Count)];
                cell.Candidates.Clear();

                _grid.RemoveUnconnectedNeighbourCandidates(cell);
            }
        }
    }
}
