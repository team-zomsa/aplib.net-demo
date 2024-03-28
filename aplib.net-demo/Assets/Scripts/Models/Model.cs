using Assets.Scripts.Tiles;
using Assets.Scripts.WFC;
using System;

namespace Assets.Scripts.Models
{
    public class Model
    {
        private readonly Func<Tile, int, int, Grid, bool> _func;
        private readonly Tile _tile;
        private readonly int _x;
        private readonly int _y;
        private readonly Grid _grid;

        public Model(Tile tile, int x, int y, Func<Tile, int, int, Grid, bool> func)
        {
            _tile = tile;
            _x = x;
            _y = y;
            _func = func;
        }

        public bool Feasible() => _func(_tile, _x, _y, _grid);
    }
}
