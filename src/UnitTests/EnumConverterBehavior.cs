using System;
using System.Collections.Generic;
using System.Text;
using MyLab.HttpMetrics;
using Xunit;

namespace UnitTests
{
    public class EnumConverterBehavior
    {
        [Theory]
        [InlineData(TestEnum.Foo, "foo")]
        [InlineData(TestEnum.Foo1, "foo_1")]
        [InlineData(TestEnum.Foo2Bar, "foo_2_bar")]
        [InlineData(TestEnum.FooBar, "foo_bar")]
        [InlineData(TestEnum.Foo | TestEnum.FooBar, "foo_foo_bar")]
        [InlineData((TestEnum)50, "50")]
        public void ShouldConvertEnumToLabel(TestEnum enumVal, string expected)
        {
            //Arrange


            //Act
            var actual = EnumConverter.ToLabel(enumVal);

            //Assert
            Assert.Equal(expected, actual);
        }

        [Flags]
        public enum TestEnum
        {
            Foo = 1,
            Foo1 = 2,
            Foo2Bar = 4,
            FooBar = 8
        }
    }
}
