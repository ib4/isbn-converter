using System;
using Xunit;

namespace isbn_converter.Tests
{
    public class ISBNConverterTest
    {
        [Fact]
        public void ConvertISBN10ToISBN13()
        {
            const string validISBN10 = "147670869X";
            const string validISBN13 = "9781476708690";
            var converter = new ISBNConverter(validISBN10);

            Assert.True(converter.IsValid());
            Assert.Null(converter.ErrorMessage);
            Assert.Equal(validISBN13, converter.ISBN13);
        }

        [Fact]
        public void ConvertISBN13ToISBN10()
        {
            const string validISBN10 = "147670869X";
            const string validISBN13 = "9781476708690";
            var converter = new ISBNConverter(validISBN13);

            Assert.True(converter.IsValid());
            Assert.Null(converter.ErrorMessage);
            Assert.Equal(validISBN10, converter.ISBN10);
        }

        [Fact]
        public void InvalidValuesInput()
        {
            const string invalidISBN10 = "247670869X";
            const string invalidISBN13 = "9782476708690";
            var converter1 = new ISBNConverter(invalidISBN10);
            var converter2 = new ISBNConverter(invalidISBN13);

            Assert.False(converter1.IsValid());
            Assert.False(converter2.IsValid());
            Assert.NotNull(converter1.ErrorMessage);
            Assert.NotNull(converter2.ErrorMessage);
            Assert.NotEqual(invalidISBN10, converter1.ISBN10);
            Assert.NotEqual(invalidISBN13, converter1.ISBN13);
        }

        [Fact]
        public void InvalidLengthInput()
        {
            const string invalidISBN10 = "47670869X";
            const string invalidISBN13 = "978476708690";
            var converter1 = new ISBNConverter(invalidISBN10);
            var converter2 = new ISBNConverter(invalidISBN13);

            Assert.False(converter1.IsValid());
            Assert.False(converter2.IsValid());
            Assert.NotNull(converter1.ErrorMessage);
            Assert.NotNull(converter2.ErrorMessage);
            Assert.NotEqual(invalidISBN10, converter1.ISBN10);
            Assert.NotEqual(invalidISBN13, converter1.ISBN13);
        }

        [Fact]
        public void ExceptionOnNullInput()
        {
            Assert.Throws<NullReferenceException>(() => new ISBNConverter(null));
        }
    }
}