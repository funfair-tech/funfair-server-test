namespace FunFair.Test.Common.Mocks
{
    /// <summary>
    ///     Base class for value.
    /// </summary>
    /// <typeparam name="T">The type of the mock.</typeparam>
    public abstract class MockBase<T>
        where T: notnull
    {
        private readonly T _value;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="value">The reference instance value.</param>
        protected MockBase(T value)
        {
            this._value = value;
        }

        /// <summary>
        ///     Gets the reference object instance.
        /// </summary>
        /// <param name="instance">The instance to convert.</param>
        /// <returns>The instance value.</returns>
        public static implicit operator T(MockBase<T> instance)
        {
            return instance._value;
        }

        /// <summary>
        ///     Gets a new T
        /// </summary>
        /// <returns></returns>
        public abstract T Next();

        /// <inheritdoc />
        public override string ToString()
        {
            return this._value.ToString() ?? string.Empty;
        }
    }
}