using System;

namespace FunFair.Test.Common.Helpers
{
    /// <summary>
    ///     Helpers for comparing reference objects.
    /// </summary>
    public static class ReferenceObjectHelpers
    {
        /// <summary>
        ///     Compares two objects for equality.
        /// </summary>
        /// <param name="left">The left-most object of the comparison.</param>
        /// <param name="right">The right-most object of the comparison.</param>
        /// <param name="eq">How to do the non-reference equals part of the comparison.</param>
        /// <typeparam name="T">The type of object being compared.</typeparam>
        /// <returns>true, if the items are the same; otherwise, false.</returns>
        public static bool AreEqual<T>(T? left, T? right, Func<T, T, bool> eq)
            where T : class
        {
            if (ReferenceEquals(objA: left, objB: right))
            {
                return true;
            }

            if (ReferenceEquals(objA: null, objB: right))
            {
                return false;
            }

            if (ReferenceEquals(objA: null, objB: left))
            {
                return false;
            }

            return eq(arg1: left, arg2: right);
        }

        /// <summary>
        ///     Compares two objects.
        /// </summary>
        /// <param name="left">The left-most object of the comparison.</param>
        /// <param name="right">The right-most object of the comparison.</param>
        /// <param name="cmp">How to do the non-reference equals part of the comparison.</param>
        /// <typeparam name="T">The type of object being compared.</typeparam>
        /// <returns>0, if the items are the same; negative if less than; positive if greater than.</returns>
        public static int Compare<T>(T? left, T? right, Func<T, T, int> cmp)
            where T : class
        {
            if (ReferenceEquals(objA: left, objB: right))
            {
                return 0;
            }

            if (ReferenceEquals(objA: null, objB: right))
            {
                return -1;
            }

            if (ReferenceEquals(objA: null, objB: left))
            {
                return 1;
            }

            return cmp(arg1: left, arg2: right);
        }
    }
}