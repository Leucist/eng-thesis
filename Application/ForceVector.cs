using Application.AppMath;
using static Application.AppMath.MathCache;

namespace Application
{
    /// <summary>
    /// Represents a force vector with a magnitude and an angle.
    /// This struct is used to define a force in terms of its strength (value) and direction (angle).
    /// </summary>
    /// <param name="value">The magnitude of the force vector, representing its strength.</param>
    /// <param name="angle">The direction of the force vector, specified in radians.</param>
    public struct ForceVector(int value, float angle)
    {
        public int Value = value;
        public float Angle = angle;

        /// <summary>
        /// Gets a ForceVector representing a zero vector (magnitude 0, angle 0).
        /// </summary>
        public static ForceVector Zero => new(0, 0);

        /// <summary>
        /// Overloads the addition operator for ForceVector.
        /// This method calculates the resultant force vector by adding two force vectors A and B.
        /// It converts the polar coordinates (magnitude and angle) of each vector into Cartesian coordinates,
        /// sums the components, and then converts the result back into polar coordinates.
        /// </summary>
        /// <param name="A">The first force vector.</param>
        /// <param name="B">The second force vector.</param>
        /// <returns>A new ForceVector representing the resultant force vector.</returns>
        public static ForceVector operator +(ForceVector A, ForceVector B) {
            float Rx, Ry;

            float Ax = A.Value * GetCos(A.Angle);
            float Bx = B.Value * GetCos(B.Angle);
            Rx = Ax + Bx;

            float Ay = A.Value * GetSin(A.Angle);
            float By = B.Value * GetSin(B.Angle);
            Ry = Ay + By;

            ForceVector R = Zero;
            R.Value = (int) MathF.Sqrt(Rx * Rx + Ry * Ry);
            R.Angle = GetAtan2(Ry, Rx);

            return R;
        }

        /// <summary>
        /// Calculates the Cartesian coordinates (offset) of the force vector based on its magnitude and angle.
        /// </summary>
        /// <returns>An OffsetEntry containing the offset relative to the X and Y axes for the given force.</returns>
        public readonly OffsetEntry GetOffset() {
            return new(Value * GetCos(Angle), Value * GetSin(Angle));
        }

        /// <summary>
        /// Overloads the multiplication operator for ForceVector.
        /// This method scales up the force vector by a specified integer multiplier.
        /// It multiplies the magnitude of the vector by the given integer.
        /// </summary>
        /// <param name="vector">The force vector to be multiplied.</param>
        /// <param name="multiplier">The integer by which to multiply the vector's magnitude.</param>
        /// <returns>A new ForceVector representing the scaled force vector.</returns>
        public static ForceVector operator *(ForceVector vector, int multiplier) {
            vector.Value *= multiplier;
            return vector;
        }

        /// <summary>
        /// Overloads the division operator for ForceVector.
        /// This method scales down the force vector by a specified integer divider.
        /// </summary>
        /// <param name="vector">The force vector to be divided.</param>
        /// <param name="divider">The integer by which to divide the vector's magnitude.</param>
        /// <returns>A new ForceVector representing the scaled-down force vector.</returns>
        public static ForceVector operator /(ForceVector vector, int divider) {
            vector.Value /= divider;
            return vector;
        }
    }
}