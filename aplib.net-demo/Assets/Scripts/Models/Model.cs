using Assets.Scripts.WFC;
using System;

namespace Assets.Scripts.Models
{
    public class Model
    {
        private readonly Func<int, int, Grid, bool> _func;

        public Model(Func<int, int, Grid, bool> func) => _func = func;

        public bool Feasible(int x, int y, Grid grid) => _func(x, y, grid);
    }
}
