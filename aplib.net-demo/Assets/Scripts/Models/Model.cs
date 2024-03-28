using System;

namespace Assets.Scripts.Models
{
    public class Model
    {
        private readonly Func<bool> _func;

        public Model(Func<bool> func) => _func = func;

        public bool Feasable() => _func();
    }
}
