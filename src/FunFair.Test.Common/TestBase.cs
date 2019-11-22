using System.Threading.Tasks;
using Xunit;

namespace FunFair.Test.Common
{
    /// <summary>
    ///     Simple base class for tests.
    /// </summary>
    public abstract class TestBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        protected TestBase()
        {
            // Nothing to do here!
            Assert.False(false, "Because");
        }

        /// <summary>
        ///     Extracts the result as a task with an optional nullable return.
        /// </summary>
        /// <param name="value">The value to return.</param>
        /// <typeparam name="T">The type to return.</typeparam>
        /// <returns>An optional result</returns>
        protected static Task<T?> FromOptionalResult<T>(T? value)
            where T : class
        {
            return Task.FromResult(value);
        }

        /// <summary>
        ///     Returns a null result for the type.
        /// </summary>
        /// <typeparam name="T">The type to return.</typeparam>
        /// <returns>A task with a null result.</returns>
        protected static Task<T?> NullResult<T>()
            where T : class
        {
            return FromOptionalResult((T?) null);
        }
    }
}

