#nullable enable
using System;

namespace ThreadSafeRandom
{
    public static class SharedRandom
    {
        [ThreadStatic]
        private static Random? _local;

        private static readonly Random _global = new();

        private static Random Instance
        {
            get
            {
                if (_local is not null) return _local;

                int seed;
                lock (_global)
                {
                    seed = _global.Next();
                }

                _local = new Random(seed);

                return _local;
            }
        }

        public static void SetSeed(int seed)
        {
            lock (_global)
            {
                if (_local == null) _local = new Random();
                else
                    lock (_local)
                    {
                        _local = new Random(seed);
                    }
            }
        }

        public static int Next() => Instance.Next();
        public static int Next(int maxValue) => Instance.Next(maxValue);
    }
}
