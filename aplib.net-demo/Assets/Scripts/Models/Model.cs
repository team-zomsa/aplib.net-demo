using Assets.Scripts.WFC;
using UnityEngine.CoreModule;
using System;

namespace Assets.Scripts.Models
{
    public class Model
    {
        private readonly Func<int, int, Grid, bool> _func;

        private readonly GameObject _model;

        public Model(GameObject model, Func<int, int, Grid, bool> func) { 
            _func = func;
            _model = model;
        }

        public bool Feasible(int x, int y, Grid grid) => _func(x, y, grid);
    }
}