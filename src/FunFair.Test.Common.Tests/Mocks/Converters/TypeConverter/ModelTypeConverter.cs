using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace FunFair.Test.Common.Tests.Mocks.Converters.TypeConverter;

public sealed class ModelTypeConverter : System.ComponentModel.TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(string) || base.CanConvertFrom(context: context, sourceType: sourceType);
    }

    public override Model? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (IsNull(value))
        {
            return null;
        }

        string rawValue = value.ToString() ?? string.Empty;

        return Convert(rawValue);
    }

    private static bool IsNull([NotNullWhen(false)] object? value)
    {
        return value == null;
    }

    private static Model? Convert(string value)
    {
        return Model.TryParse(source: value, out Model? model)
            ? model
            : null;
    }
}