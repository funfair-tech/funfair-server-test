using System;
using System.Data;
using System.Text;
using Dapper;

namespace FunFair.Test.Common.Tests;

public sealed class ExampleRecordTypeMapper : SqlMapper.TypeHandler<ExampleRecord>
{
    public override void SetValue(IDbDataParameter parameter, ExampleRecord? value)
    {
        if (value is null)
        {
            parameter.Value = DBNull.Value;

            return;
        }

        if (StringComparer.Ordinal.Equals(x: value.Name, y: "Exception"))
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                actualValue: value.Name,
                message: "Example"
            );
        }

        if (StringComparer.Ordinal.Equals(x: value.Name, y: "Binary"))
        {
            parameter.Value = Encoding.UTF8.GetBytes(value.Name);

            return;
        }

        parameter.Value = value.Name;
    }

    public override ExampleRecord Parse(object value)
    {
        return new((string)value);
    }
}
