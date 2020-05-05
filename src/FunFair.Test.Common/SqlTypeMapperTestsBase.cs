using System.Data;
using Dapper;
using NSubstitute;
using Xunit;

namespace FunFair.Test.Common
{
    public abstract class SqlTypeMapperTestsBase<TTypeMapper, TMappedType>
        where TTypeMapper : SqlMapper.TypeHandler<TMappedType>, new()
    {
        private readonly TTypeMapper _handler;

        protected SqlTypeMapperTestsBase()
        {
            this._handler = new TTypeMapper();
        }

        protected void Parse(object value, TMappedType expected)
        {
            TMappedType result = this._handler.Parse(value);

            Assert.Equal(expected, result);
        }

        protected void SetValue(TMappedType value, object expected)
        {
            IDbDataParameter parameter = Substitute.For<IDbDataParameter>();

            this._handler.SetValue(parameter, value);

            parameter.Received(requiredNumberOfCalls: 1)
                     .Value = expected;
        }
    }
}