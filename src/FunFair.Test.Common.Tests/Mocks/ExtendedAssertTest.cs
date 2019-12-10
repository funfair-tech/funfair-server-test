using FunFair.Test.Common.Mocks;
using Xunit;
using Xunit.Sdk;

namespace FunFair.Test.Common.Tests.Mocks
{
    public sealed class ExtendedAssertTest
    {
        private sealed class Model
        {
            public string Value { get; }

            public string[] NestedValue { get; set; }

            public Model(string value)
            {
                this.Value = value;
                this.NestedValue = new[] { value };
            }
        }

        [Fact]
        public void TwoObjectsAreDeepEqualIfAllValuesAreEquals()
        {
            Model expected = new Model(value: "expected");
            Model actual = new Model(value: "expected");

            ExtendedAssert.DeepEqual(expected, actual);
        }

        [Fact]
        public void TwoObjectsAreNotDeepEqualIfAllValuesAreEquals()
        {
            Model expected = new Model(value: "expected");
            Model actual = new Model(value: "actual");

            Assert.Throws<EqualException>(() => { ExtendedAssert.DeepEqual(expected, actual); });
        }

        [Fact]
        public void TwoObjectsAreNotDeepEqualIfAnyNestedValueIsNotEqual()
        {
            Model expected = new Model(value: "expected");
            expected.NestedValue = new[] { "new nested value" };
            Model actual = new Model(value: "expected");

            Assert.Throws<EqualException>(() => { ExtendedAssert.DeepEqual(expected, actual); });
        }
    }
}