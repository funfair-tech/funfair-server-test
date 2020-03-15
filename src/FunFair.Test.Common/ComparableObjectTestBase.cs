using System;
using Xunit;

namespace FunFair.Test.Common
{
    /// <summary>
    /// Base class for test objects that are equality comparable.
    /// </summary>
    /// <typeparam name="TObject">The object to compare.</typeparam>
    public abstract class ComparableObjectTestBase<TObject> : EquatableObjectTestBase<TObject>
        where TObject : class, IEquatable<TObject>, IComparable<TObject>, IComparable
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="zeroObject">The object that's equivalent to zero or null.</param>
        /// <param name="value1">A value to use for comparisons.</param>
        /// <param name="equivalentToValue1">An equivalent value to <paramref name="value1"/> that is not ReferenceEqual to <paramref name="value1"/>.</param>
        /// <param name="value2">Another value to use for comparisons.  Should be greater than <paramref name="value1"/>.</param>
        protected ComparableObjectTestBase(TObject zeroObject, TObject value1, TObject equivalentToValue1, TObject value2)
            : base(zeroObject, value1, equivalentToValue1)
        {
            this.Value2 = value2;
        }

        private TObject Value2 { get; }

        /// <summary>
        /// Implementation should call operator &gt;=(x,y)
        /// </summary>
        /// <param name="l">An item</param>
        /// <param name="r">Another item.</param>
        /// <returns>True if the r is greater than or equal to l; otherwise, false.</returns>
        protected abstract bool OperatorGreaterThanOrEqualTo(TObject? l, TObject? r);

        /// <summary>
        /// Implementation should call operator &lt;=(x,y)
        /// </summary>
        /// <param name="l">An item</param>
        /// <param name="r">Another item.</param>
        /// <returns>True if the r is less than or equal to l; otherwise, false.</returns>
        protected abstract bool OperatorLessThanOrEqualTo(TObject? l, TObject? r);

        /// <summary>
        /// Implementation should call operator &gt;(x,y)
        /// </summary>
        /// <param name="l">An item</param>
        /// <param name="r">Another item.</param>
        /// <returns>True if the r is greater than l; otherwise, false.</returns>
        protected abstract bool OperatorGreaterThan(TObject? l, TObject? r);

        /// <summary>
        /// Implementation should call operator &lt;(x,y)
        /// </summary>
        /// <param name="l">An item</param>
        /// <param name="r">Another item.</param>
        /// <returns>True if the r is less than l; otherwise, false.</returns>
        protected abstract bool OperatorLessThan(TObject? l, TObject? r);

        private static int TypedCompareTo(TObject l, TObject? r)
        {
            IComparable<TObject> cmp = l;

            return cmp.CompareTo(r);
        }

        private static int UntypedCompareTo(TObject l, object? r)
        {
            IComparable cmp = l;

            return cmp.CompareTo(r);
        }

        /// <summary>
        /// Compares Value1 with NullObject.
        /// </summary>
        [Fact]
        public void OperatorGreaterOrEqualToThanNullObjectIsNotGreaterOrEquivalentToNullObject()
        {
            Assert.False(this.OperatorGreaterThanOrEqualTo(this.Value1, this.NullObject), "NullObject >= Value1");
        }

        /// <summary>
        /// Compares NullObject with Value1.
        /// </summary>
        [Fact]
        public void OperatorGreaterThanNullObjectIsGreaterThanValue1()
        {
            Assert.True(this.OperatorGreaterThan(this.NullObject, this.Value2), "NullObject > Value2");
        }

        /// <summary>
        /// Compares Value1 with NullObject.
        /// </summary>
        [Fact]
        public void OperatorGreaterThanNullObjectIsNotGreaterThanNullObject()
        {
            Assert.False(this.OperatorGreaterThan(this.Value1, this.NullObject), "NullObject > Value1");
        }

        /// <summary>
        /// Compares NullObject with Value2.
        /// </summary>
        [Fact]
        public void OperatorGreaterThanOrEqualToNullObjectIsGreaterThanOrEquivalentToValue1()
        {
            Assert.True(this.OperatorGreaterThanOrEqualTo(this.NullObject, this.Value2), "NullObject >= Value2");
        }

        /// <summary>
        /// Compares Value1 with EquivalentToValue1.
        /// </summary>
        [Fact]
        public void OperatorGreaterThanOrEqualToValue1IsGreaterThanOrEquivalentToValue1()
        {
            Assert.True(this.OperatorGreaterThanOrEqualTo(this.Value1, this.EquivalentToValue1), "Value1 >= Value1");
        }

        /// <summary>
        /// Compares Value1 with NullObject.
        /// </summary>
        [Fact]
        public void OperatorGreaterThanOrEqualToValue1IsNotGreaterThanOrEquivalentToNullObject()
        {
            Assert.False(this.OperatorGreaterThanOrEqualTo(this.Value1, this.NullObject), "Value1 >= NullObject");
        }

        /// <summary>
        /// Compares Value1 with EquivalentToValue1.
        /// </summary>
        [Fact]
        public void OperatorGreaterThanOrEqualToValue1IsNotGreaterThanOrEquivalentToValue1()
        {
            Assert.True(this.OperatorGreaterThanOrEqualTo(this.Value1, this.Value1), "Value1 >= Value1");
        }

        /// <summary>
        /// Compares Value1 with Value2.
        /// </summary>
        [Fact]
        public void OperatorGreaterThanOrEqualToValue1IsNotGreaterThanOrEquivalentToValue2()
        {
            Assert.False(this.OperatorGreaterThanOrEqualTo(this.Value1, this.Value2), "Value1 >= Value2");
        }

        /// <summary>
        /// Compares Value2 with Value1.
        /// </summary>
        [Fact]
        public void OperatorGreaterThanOrEqualToValue2IsGreaterThanOrEquivalentToValue1()
        {
            Assert.True(this.OperatorGreaterThanOrEqualTo(this.Value2, this.Value1), "Value2 >= Value1");
        }

        /// <summary>
        /// Compares Value2 with NullObject.
        /// </summary>
        [Fact]
        public void OperatorGreaterThanOrEqualToValue2IsNotGreaterThanOrEquivalentToNullObject()
        {
            Assert.False(this.OperatorGreaterThanOrEqualTo(this.Value2, this.NullObject), "Value2 >= NullObject");
        }

        /// <summary>
        /// Compares Value1 with NullObject.
        /// </summary>
        [Fact]
        public void OperatorGreaterThanValue1IsNotGreaterThanNullObject()
        {
            Assert.False(this.OperatorGreaterThan(this.Value1, this.NullObject), "Value1 > NullObject");
        }

        /// <summary>
        /// Compares Value1 with Value1.
        /// </summary>
        [Fact]
        public void OperatorGreaterThanValue1IsNotGreaterThanValue1()
        {
            Assert.False(this.OperatorGreaterThan(this.Value1, this.Value1), "Value1 > Value1");
        }

        /// <summary>
        /// Compares Value1 with Value2.
        /// </summary>
        [Fact]
        public void OperatorGreaterThanValue1IsNotGreaterThanValue2()
        {
            Assert.False(this.OperatorGreaterThan(this.Value1, this.Value2), "Value1 > Value2");
        }

        /// <summary>
        /// Compares Value2 with Value1.
        /// </summary>
        [Fact]
        public void OperatorGreaterThanValue2IsGreaterThanValue1()
        {
            Assert.True(this.OperatorGreaterThan(this.Value2, this.Value1), "Value2 > Value1");
        }

        /// <summary>
        /// Compares Value2 with NullObject.
        /// </summary>
        [Fact]
        public void OperatorGreaterThanValue2IsNotGreaterThanNullObject()
        {
            Assert.False(this.OperatorGreaterThan(this.Value2, this.NullObject), "Value2 > NullObject");
        }

        /// <summary>
        /// Compares Value1 with NullObject.
        /// </summary>
        [Fact]
        public void OperatorLessOrEqualToThanNullObjectIsLessThanOrEquivalentToNullObject()
        {
            Assert.True(this.OperatorLessThanOrEqualTo(this.Value1, this.NullObject), "NullObject <= Value1");
        }

        /// <summary>
        /// Compares Value1 with NullObject.
        /// </summary>
        [Fact]
        public void OperatorLessThanNullObjectIsLessThanNullObject()
        {
            Assert.True(this.OperatorLessThan(this.Value1, this.NullObject), "NullObject < Value1");
        }

        /// <summary>
        /// Compares NullObject with Value2.
        /// </summary>
        [Fact]
        public void OperatorLessThanNullObjectIsNotLessThanValue1()
        {
            Assert.False(this.OperatorLessThan(this.NullObject, this.Value2), "NullObject < Value2");
        }

        /// <summary>
        /// Compares NullObject with Value2.
        /// </summary>
        [Fact]
        public void OperatorLessThanOrEqualToNullObjectIsNotLessThanOrEquivalentToValue1()
        {
            Assert.False(this.OperatorLessThanOrEqualTo(this.NullObject, this.Value2), "NullObject <= Value2");
        }

        /// <summary>
        /// Compares Value1 with EquivalentToValue1.
        /// </summary>
        [Fact]
        public void OperatorLessThanOrEqualToValue1IsLessThanOrEquivalentToNullObject()
        {
            Assert.True(this.OperatorLessThanOrEqualTo(this.Value1, this.NullObject), "Value1 <= NullObject");
        }

        /// <summary>
        /// Compares Value1 with EquivalentToValue1.
        /// </summary>
        [Fact]
        public void OperatorLessThanOrEqualToValue1IsLessThanOrEquivalentToValue1()
        {
            Assert.True(this.OperatorLessThanOrEqualTo(this.Value1, this.EquivalentToValue1), "Value1 <= Value1");
        }

        /// <summary>
        /// Compares Value1 with Value2.
        /// </summary>
        [Fact]
        public void OperatorLessThanOrEqualToValue1IsLessThanOrEquivalentToValue2()
        {
            Assert.True(this.OperatorLessThanOrEqualTo(this.Value1, this.Value2), "Value1 <= Value2");
        }

        /// <summary>
        /// Compares Value1 with Value1.
        /// </summary>
        [Fact]
        public void OperatorLessThanOrEqualToValue1IsNotLessThanOrEquivalentToValue1()
        {
            Assert.True(this.OperatorLessThanOrEqualTo(this.Value1, this.Value1), "Value1 <= Value1");
        }

        /// <summary>
        /// Compares Value2 with NullObject.
        /// </summary>
        [Fact]
        public void OperatorLessThanOrEqualToValue2IsLessThanOrEquivalentToNullObject()
        {
            Assert.True(this.OperatorLessThanOrEqualTo(this.Value2, this.NullObject), "Value2 <= NullObject");
        }

        /// <summary>
        /// Compares Value2 with Value1.
        /// </summary>
        [Fact]
        public void OperatorLessThanOrEqualToValue2IsNotLessThanOrEquivalentToValue1()
        {
            Assert.False(this.OperatorLessThanOrEqualTo(this.Value2, this.Value1), "Value2 <= Value1");
        }

        /// <summary>
        /// Compares Value1 with NullObject.
        /// </summary>
        [Fact]
        public void OperatorLessThanValue1IsLessThanNullObject()
        {
            Assert.True(this.OperatorLessThan(this.Value1, this.NullObject), "Value1 < NullObject");
        }

        /// <summary>
        /// Compares Value1 with Value2.
        /// </summary>
        [Fact]
        public void OperatorLessThanValue1IsLessThanValue2()
        {
            Assert.True(this.OperatorLessThan(this.Value1, this.Value2), "Value1 < Value2");
        }

        /// <summary>
        /// Compares Value1 with Value1.
        /// </summary>
        [Fact]
        public void OperatorLessThanValue1IsNotLessThanValue1()
        {
            Assert.False(this.OperatorLessThan(this.Value1, this.Value1), "Value1 < Value1");
        }

        /// <summary>
        /// Compares Value2 with NullObject.
        /// </summary>
        [Fact]
        public void OperatorLessThanValue2IsLessThanNullObject()
        {
            Assert.True(this.OperatorLessThan(this.Value2, this.NullObject), "Value2 < NullObject");
        }

        /// <summary>
        /// Compares Value2 with Value1.
        /// </summary>
        [Fact]
        public void OperatorLessThanValue2IsNotLessThanValue1()
        {
            Assert.False(this.OperatorLessThan(this.Value2, this.Value1), "Value2 < Value1");
        }

        /// <summary>
        /// Compares Value1 with Value1.
        /// </summary>
        [Fact]
        public void TypedCompareToValue1EqualToEquivalentToValue1()
        {
            Assert.True(TypedCompareTo(this.Value1, this.EquivalentToValue1) == 0, "Should be equal to 0");
        }

        /// <summary>
        /// Compares Value1 with NullObject.
        /// </summary>
        [Fact]
        public void TypedCompareToValue1EqualToNullObject()
        {
            Assert.True(TypedCompareTo(this.Value1, this.NullObject) < 0, "Should be less than 0");
        }

        /// <summary>
        /// Compares Value1 with Value2.
        /// </summary>
        [Fact]
        public void TypedCompareToValue1LessThanValue2()
        {
            Assert.True(TypedCompareTo(this.Value1, this.Value2) < 0, "Should be less than 0");
        }

        /// <summary>
        /// Compares Value1 with Value2.
        /// </summary>
        [Fact]
        public void TypedCompareToValue2GreaterThanValue1()
        {
            Assert.True(TypedCompareTo(this.Value2, this.Value1) > 0, "Should be greater than 0");
        }

        /// <summary>
        /// Compares Value1 with EquivalentToValue1AsObject.
        /// </summary>
        [Fact]
        public void UntypedCompareToValue1EqualsUnTypedValue1Alias()
        {
            Assert.True(UntypedCompareTo(this.Value1, this.EquivalentToValue1AsObject) == 0, "Should be equal to 0");
        }

        /// <summary>
        /// Compares Value1 with Value1.
        /// </summary>
        [Fact]
        public void UntypedCompareToValue1EqualToEquivalentToValue1()
        {
            Assert.True(UntypedCompareTo(this.Value1, this.EquivalentToValue1) == 0, "Should be equal to 0");
        }

        /// <summary>
        /// Compares Value1 with NullObject.
        /// </summary>
        [Fact]
        public void UntypedCompareToValue1LessThanOtherTypedObject()
        {
            Assert.True(UntypedCompareTo(this.Value1, "Banana") < 0, "Should be less than 0");
        }

        /// <summary>
        /// Compares Value1 with NullObject.
        /// </summary>
        [Fact]
        public void UntypedCompareToValue1LessThanToNullObject()
        {
            Assert.True(UntypedCompareTo(this.Value1, this.NullObject) < 0, "Should be less than 0");
        }

        /// <summary>
        /// Compares Value1 with Value2.
        /// </summary>
        [Fact]
        public void UntypedCompareToValue1LessThanValue2()
        {
            Assert.True(UntypedCompareTo(this.Value1, this.Value2) < 0, "Should be less than 0");
        }

        /// <summary>
        /// Compares Value1 with EquivalentToValue1AsObject.
        /// </summary>
        [Fact]
        public void UntypedCompareToValue2GreaterThanUnTypedValue1Alias()
        {
            Assert.True(UntypedCompareTo(this.Value2, this.EquivalentToValue1AsObject) > 0, "Should be greater than to 0");
        }

        /// <summary>
        /// Compares Value2 with Value1.
        /// </summary>
        [Fact]
        public void UntypedCompareToValue2GreaterThanValue1()
        {
            Assert.True(UntypedCompareTo(this.Value2, this.Value1) > 0, "Should be greater than 0");
        }
    }
}