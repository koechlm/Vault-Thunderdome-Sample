namespace Thunderdome.Util
{
    /// <summary>
    /// Contains utility methods for array operations
    /// </summary>
    internal static class ArrayUtil
    {
        /// <summary>
        /// Compares two arrays
        /// </summary>
        /// <param name="arrayA">First array to compare</param>
        /// <param name="arrayB">Second array to compare</param>
        /// <returns><c>true</c> if arrays are of the same length and have equal elements on the same position, <c>false</c> otherwise</returns>
        public static bool Equal(byte[] arrayA, byte[] arrayB)
        {
            if (arrayA == null || arrayB == null)
                return false;
            if (ReferenceEquals(arrayA, arrayB))
                return true;
            if (arrayA.Length != arrayB.Length)
                return false;

            for (int i = 0; i < arrayA.Length; i++)
            {
                if (arrayA[i] != arrayB[i])
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Creates array containing one element
        /// </summary>
        /// <typeparam name="T">Array elements type</typeparam>
        /// <param name="obj">Element to create array from</param>
        /// <returns>Array containing single <paramref name="obj"/> element</returns>
        internal static T[] FromSingle<T>(T obj)
        {
            return new [] { obj };
        }
    }
}
