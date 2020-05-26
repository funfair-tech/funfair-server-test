using System;
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
        /// <summary>
        ///     Constructor.
        /// </summary>
        protected SqlTypeMapperTestsBase()
        {
            this.Handler = new TTypeMapper();
        }

        /// <summary>
        ///     The type handler.
        /// </summary>
        protected TTypeMapper Handler { get; }

        /// <summary>
        ///     Checks that the handler can parse the value into the mapped type.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <param name="expected">The expected value of the mapped type.</param>
        /// <typeparam name="TValueType">The type of the value.</typeparam>
        protected void ShouldParse<TValueType>(TValueType value, TMappedType expected)
        {
            TMappedType result = this.Handler.Parse(value);

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

            this.Handler.SetValue(parameter: parameter, value: value);

            parameter.Received(requiredNumberOfCalls: 1)
                     .Value = expected;
        }

        /// <summary>
        ///     Checks that the value is set to the expected value.
        /// </summary>
        /// <param name="value">The value to set.</param>
        /// <param name="expected">The expected typed value.</param>
        protected void ShouldSetValue(TMappedType value, in byte[] expected)
        {
            IDbDataParameter parameter = Substitute.For<IDbDataParameter>();

            this.Handler.SetValue(parameter: parameter, value: value);

            parameter.Received(requiredNumberOfCalls: 1)
                     .Value = expected;
        }

        /// <summary>
        ///     Checks that the value does not parse, and raises an the <typeparamref name="TExceptionType" /> exception.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <typeparam name="TExceptionType">The exception that should be raised.</typeparam>
        /// <typeparam name="TValueType">The type of the value.</typeparam>
        protected void ShouldNotParse<TExceptionType, TValueType>(TValueType value)
            where TExceptionType : Exception
        {
            Assert.Throws<TExceptionType>(() => this.Handler.Parse(value));
        }
    }
}