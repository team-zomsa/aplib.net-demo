namespace Assets.Scripts.Tiles
{
    /// <summary>
    /// The direction in which a tile can face.
    /// </summary>
    public enum Direction
    {
        North = 0,
        East = 1,
        South = 2,
        West = 3
    }

    /// <summary>
    /// Useful extension methods for the <see cref="Direction"/> enum.
    /// </summary>
    public static class DirectionExtensions
    {
        /// <summary>
        /// Rotates the direction X times clockwise.
        /// </summary>
        /// <param name="direction">The direction to rotate.</param>
        /// <param name="times">The number of times the direction must be rotated.</param>
        /// <returns>The adjusted direction.</returns>
        public static Direction RotateRight(this Direction direction, int times = 1)
            => (Direction)(((int)direction + times) % 4);

        /// <summary>
        /// Rotates the direction X times counterclockwise.
        /// </summary>
        /// <param name="direction">The direction to rotate.</param>
        /// <param name="times">The number of times the direction must be rotated.</param>
        /// <returns>The adjusted direction.</returns>
        public static Direction RotateLeft(this Direction direction, int times = 1)
            => (Direction)(((int)direction + times * 3) % 4);

        /// <summary>
        /// Rotates the direction X times to the left or right.
        /// </summary>
        /// <param name="direction">The direction to rotate.</param>
        /// <param name="times">
        /// The number of times the direction must be rotated.
        /// Negative numbers indicate rotation towards the left.
        /// </param>
        /// <returns>The adjusted direction.</returns>
        public static Direction Rotate(this Direction direction, int times)
            => times >= 0 ? direction.RotateRight(times) : direction.RotateLeft(-times);

        /// <summary>
        /// Gives the opposite direction.
        /// </summary>
        /// <param name="direction">The direction whose opposite needs to be given.</param>
        /// <returns>The opposite of <paramref name="direction"/>.</returns>
        public static Direction Opposite(this Direction direction)
            => (Direction)(((int)direction + 2) % 4);

        /// <summary>
        /// Calculates how many degrees clockwise the given direction is rotated from the north.
        /// </summary>
        /// <param name="direction"></param>
        /// <returns>The clockwise rotation in degrees, [0..360)</returns>
        public static int RotationDegrees(this Direction direction) => (int)direction * 90;
    }
}
