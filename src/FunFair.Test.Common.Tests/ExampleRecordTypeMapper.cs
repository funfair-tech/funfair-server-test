using System.Data;
using Dapper;

namespace FunFair.Test.Common.Tests;

public sealed class ExampleRecordTypeMapper : SqlMapper.TypeHandler<ExampleRecord>
{
    public override void SetValue(IDbDataParameter parameter, ExampleRecord value)
    {
        parameter.Value = value.Name;
    }

    public override ExampleRecord Parse(object value)
    {
        return new((string)value);
    }
}