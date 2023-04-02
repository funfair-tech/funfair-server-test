using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using Dapper;
using NSubstitute;
using Xunit;

namespace FunFair.Test.Common;

public abstract class SqlTypeMapperTestsBase<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TTypeMapper, TMappedType> : TestBase
    where TTypeMapper : SqlMapper.TypeHandler<TMappedType>, new()
{
    protected SqlTypeMapperTestsBase()
    {
        this.Handler = new();
    }

    protected TTypeMapper Handler { get; }

    protected void ShouldParse<TValueType>(TValueType value, TMappedType expected)
    {
        TMappedType result = this.Handler.Parse(value);

        Assert.Equal(expected: expected, actual: result);
    }

    protected void ShouldSetValue<TValueType>(TMappedType value, TValueType expected)
    {
        IDbDataParameter parameter = GetSubstitute<IDbDataParameter>();

        this.Handler.SetValue(parameter: parameter, value: value);

        parameter.Received(requiredNumberOfCalls: 1)
                 .Value = expected;
    }

    protected void ShouldSetValue(TMappedType value, in byte[] expected)
    {
        // note special case for byte arrays as NSubstitute whatever you give it always says it received something other than the expected
        IDbDataParameter parameter = new MockParameter();

        this.Handler.SetValue(parameter: parameter, value: value);

        object? result = parameter.Value;
        Assert.NotNull(result);
        Assert.IsType<byte[]>(result);

        Assert.Equal(BitConverter.ToString(expected), BitConverter.ToString((byte[])result));
    }

    protected void ShouldNotSetValue<TExceptionType>(TMappedType value)
        where TExceptionType : Exception
    {
        // note special case for byte arrays as NSubstitute whatever you give it always says it received something other than the expected
        IDbDataParameter parameter = new MockParameter();

        Assert.Throws<TExceptionType>(() => this.Handler.SetValue(parameter: parameter, value: value));
    }

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