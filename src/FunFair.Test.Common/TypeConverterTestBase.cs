using System;
using System.ComponentModel;
using Xunit;

namespace FunFair.Test.Common
{
    /// <summary>
    ///     The type convert test base
    /// </summary>
    /// <typeparam name="TConverter">The type of converter</typeparam>
    /// <typeparam name="TObject">The object being converted.</typeparam>
    [Obsolete("2021-08-19 Use Model Binding Instead")]
    public abstract class TypeConverterTestBase<TConverter, TObject> : TestBase
        where TConverter : TypeConverter, new() where TObject : class
    {
        private readonly Type _convertType;
        private readonly Type _objectType;

        /// <summary>
        ///     The type convert test base
        /// </summary>
        protected TypeConverterTestBase()
        {
            this._convertType = typeof(TConverter);
            this._objectType = typeof(TObject);
        }

        /// <summary>
        ///     Convert value into model
        /// </summary>
        /// <param name="rawValue">value to convert</param>
        /// <returns>Convert value</returns>
        protected object? GetConvertedValue(string rawValue)
        {
            Assert.NotNull(rawValue);

            TypeConverter converter = this.GetConverter();

            return converter.ConvertFrom(rawValue);
        }

        /// <summary>
        ///     Get converter for given model
        /// </summary>
        /// <returns>Converter</returns>
        private TypeConverter GetConverter()
        {
            TypeDescriptor.AddAttributes(type: this._objectType, new TypeConverterAttribute(this._convertType));

            return TypeDescriptor.GetConverter(this._objectType);
        }
    }
}