using System.Diagnostics.CodeAnalysis;

namespace FunFair.Test.Common.Mocks
{
    /// <summary>
    ///     Base class for value.
    /// </summary>
    /// <typeparam name="T">The type of the mock.</typeparam>
    [SuppressMessage(category: "ReSharper", checkId: "UnusedType.Global", Justification = "Base class for further tests")]
    public abstract class MockBase<T>
        where T : notnull
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
        [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Review")]
        public abstract T Next();

        /// <inheritdoc />
        [SuppressMessage(category: "ToStringWithoutOverrideAnalyzer",
                         checkId: "ExplicitToStringWithoutOverrideAnalyzer: Calling ToString() on object of type 'T' but it does not override ToString()",
                         Justification = "TODO: Review")]
        public override string ToString()
        {
            return this._value.ToString() ?? string.Empty;
        }
    }
}

