using System.Data;
using Dapper;
using NSubstitute;
using Xunit;

namespace FunFair.Test.Common
{
    /// <summary>
    ///     Base cass for SQL Type mapper tests.
    /// </summary>
    /// <typeparam name="TTypeMapper">The type mapper to test.</typeparam>
    /// <typeparam name="TMappedType">The mapped type.</typeparam>
    public abstract class SqlTypeMapperTestsBase<TTypeMapper, TMappedType> : TestBase
        where TTypeMapper : SqlMapper.TypeHandler<TMappedType>, new()
    {
        private readonly TTypeMapper _handler;

        /// <summary>
        ///     Constructor.
        /// </summary>
        protected SqlTypeMapperTestsBase()
        {
            this._handler = new TTypeMapper();
        }

        /// <summary>
        ///     Checks that the handler can parse the value into the mapped type.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <param name="expected">The expected value of the mapped type.</param>
        /// <typeparam name="TValueType">The type of the value.</typeparam>
        protected void ShouldParse<TValueType>(TValueType value, TMappedType expected)
        {
            TMappedType result = this._handler.Parse(value);

            Assert.Equal(expected: expected, actual: result);
        }

        /// <summary>
        ///     Checks that the value is set to the expected value.
        /// </summary>
        /// <param name="value">The value to set.</param>
        /// <param name="expected">The expected typed value.</param>
        /// <typeparam name="TValueType">The type of the value.</typeparam>
        protected void ShouldSetValue<TValueType>(TMappedType value, TValueType expected)
        {
            IDbDataParameter parameter = Substitute.For<IDbDataParameter>();

            this._handler.SetValue(parameter: parameter, value: value);

            parameter.Received(requiredNumberOfCalls: 1)
                     .Value = expected;
        }
    }
}