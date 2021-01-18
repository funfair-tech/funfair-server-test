using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
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
    [SuppressMessage(category: "ReSharper", checkId: "UnusedType.Global", Justification = "Base class for further tests")]
    public abstract class SqlTypeMapperTestsBase<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
                                                 TTypeMapper, TMappedType> : TestBase
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

        // ReSharper disable once UnusedMember.Global
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

        // ReSharper disable once UnusedMember.Global
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

        // ReSharper disable once UnusedMember.Global
        protected void ShouldSetValue(TMappedType value, in byte[] expected)
        {
            // note special case for byte arrays as NSubstitute whatever you give it always says it received something other than the expected
            IDbDataParameter parameter = new MockParameter();

            this.Handler.SetValue(parameter: parameter, value: value);

            object? result = parameter.Value;
            Assert.NotNull(result);
            Assert.IsType<byte[]>(result);

            Assert.Equal(BitConverter.ToString(expected), BitConverter.ToString((byte[]) result!));
        }

        /// <summary>
        ///     Checks that the value does not parse, and raises an the <typeparamref name="TExceptionType" /> exception.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <typeparam name="TExceptionType">The exception that should be raised.</typeparam>
        /// <typeparam name="TValueType">The type of the value.</typeparam>

        // ReSharper disable once UnusedMember.Global
        protected void ShouldNotParse<TExceptionType, TValueType>(TValueType value)
            where TExceptionType : Exception
        {
            Assert.Throws<TExceptionType>(() => this.Handler.Parse(value));
        }

        private sealed class MockParameter : IDbDataParameter
        {
            public DbType DbType { get; set; }

            public ParameterDirection Direction { get; set; }

            public bool IsNullable => false;

            [AllowNull]
            public string ParameterName { get; set; } = default!;

            [AllowNull]
            public string SourceColumn { get; set; } = default!;

            public DataRowVersion SourceVersion { get; set; }

            public object? Value { get; set; } = default!;

            public byte Precision { get; set; }

            public byte Scale { get; set; }

            public int Size { get; set; }
        }
    }
}