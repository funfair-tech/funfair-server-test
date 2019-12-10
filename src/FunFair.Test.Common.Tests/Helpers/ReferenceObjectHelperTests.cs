using System;
using FunFair.Test.Common.Helpers;
using Xunit;

namespace FunFair.Test.Common.Tests.Helpers
{
    public class ReferenceObjectHelperTests
    {
        private readonly Func<Model, Model, int> _compare = (left, right) => left.Value.CompareTo(right.Value);

        private readonly Func<Model, Model, bool> _equals = (left, right) => left.Equals(right);

        private class Model
        {
            public int Value { get; }

            public Model(int value)
            {
                this.Value = value;
            }
        }

        [Fact]
        public void ObjectsAreEqualIfTheyAreSameReference()
        {
            Model obj = new Model(value: 1);

            Assert.True(ReferenceObjectHelpers.AreEqual<Model>(obj, obj, this._equals));
        }

        [Fact]
        public void ObjectsAreEqualIfTheirNonReferencePartsAreEquals()
        {
            int value = 1;
            Model left = new Model(value);
            Model right = new Model(value);

            Assert.True(ReferenceObjectHelpers.AreEqual<Model>(left, right, (l, r) => l.Value.Equals(r.Value)));
        }

        [Fact]
        public void ObjectsAreNotEqualIfLeftIsNull()
        {
            Model right = new Model(value: 1);

            Assert.False(ReferenceObjectHelpers.AreEqual<Model>(left: null, right, this._equals));
        }

        [Fact]
        public void ObjectsAreNotEqualIfRightIsNull()
        {
            Model left = new Model(value: 1);

            Assert.False(ReferenceObjectHelpers.AreEqual<Model>(left, right: null, this._equals));
        }

        [Fact]
        public void ObjectsAreNotEqualIfTheirNonReferencePartsAreNotEquals()
        {
            Model left = new Model(value: 1);
            Model right = new Model(value: 2);

            Assert.False(ReferenceObjectHelpers.AreEqual<Model>(left, right, this._equals));
        }

        [Fact]
        public void ObjectsAreSameIfTheyAreSameReference()
        {
            Model obj = new Model(value: 1);


            Assert.Equal(expected: 0, actual: ReferenceObjectHelpers.Compare<Model>(obj, obj, this._compare));
        }

        [Fact]
        public void ObjectsAreSameIfTheirNonReferencePartsAreEquals()
        {
            int value = 1;
            Model left = new Model(value);
            Model right = new Model(value);

            Assert.Equal(expected: 0, actual: ReferenceObjectHelpers.Compare<Model>(left, right, this._compare));
        }

        [Fact]
        public void ObjectsAreNotSameIfLeftIsNull()
        {
            Model right = new Model(value: 1);

            Assert.Equal(expected: 1, actual: ReferenceObjectHelpers.Compare<Model>(left: null, right, this._compare));
        }

        [Fact]
        public void ObjectsAreNotSameIfRigthIsNull()
        {
            Model left = new Model(value: 1);

            Assert.Equal(expected: -1, actual: ReferenceObjectHelpers.Compare<Model>(left, right: null, this._compare));
        }

        [Fact]
        public void ObjectsAreNotSameIfLeftNonReferenceIsLess()
        {
            Model left = new Model(value: 1);
            Model right = new Model(value: 2);

            Assert.Equal(expected: -1, actual: ReferenceObjectHelpers.Compare<Model>(left, right, this._compare));
        }

        [Fact]
        public void ObjectsAreNotSameIfLeftNonReferenceIsBigger()
        {
            Model left = new Model(value: 2);
            Model right = new Model(value: 1);

            Assert.Equal(expected: 1, actual: ReferenceObjectHelpers.Compare<Model>(left, right, this._compare));
        }
    }
}