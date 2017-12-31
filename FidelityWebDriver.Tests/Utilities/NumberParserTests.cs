using System.Globalization;
using log4net;
using Moq;
using NUnit.Framework;
using Sonneville.FidelityWebDriver.Utilities;

namespace Sonneville.FidelityWebDriver.Tests.Utilities
{
    [TestFixture]
    public class NumberParserTests
    {
        private Mock<ILog> _logMock;

        [SetUp]
        public void Setup()
        {
            _logMock = new Mock<ILog>();

            NumberParser.SetLogger(_logMock.Object);
        }

        [Test]
        [TestCase("0", 0)]
        [TestCase("0.0", 0)]
        [TestCase("1.0", 1)]
        [TestCase("-1.0", -1)]
        public void ShouldParseDoubles(string input, double expected)
        {
            var actual = NumberParser.ParseDouble(input);

            Assert.AreEqual(expected, actual);
            _logMock.Verify(log => log.Debug(It.Is<object>(o => o.ToString().Contains(input))));
        }

        [Test]
        [TestCase("0")]
        [TestCase("0.0")]
        [TestCase("1.0")]
        [TestCase("-1.0")]
        public void ShouldParseDecimals(string input)
        {
            var actual = NumberParser.ParseDecimal(input);

            Assert.IsAssignableFrom<decimal>(actual);
            Assert.AreEqual(input, actual.ToString(CultureInfo.InvariantCulture));
            _logMock.Verify(log => log.Debug(It.Is<object>(o => o.ToString().Contains(input))));
        }

        [Test]
        [TestCase("0", 0)]
        [TestCase("1", 1)]
        [TestCase("-1", -1)]
        [TestCase("2147483647", 2147483647)]
        [TestCase("-2147483648", -2147483648)]
        public void ShouldParseIntegers(string input, double expected)
        {
            var actual = NumberParser.ParseInt(input);

            Assert.AreEqual(expected, actual);
            _logMock.Verify(log => log.Debug(It.Is<object>(o => o.ToString().Contains(input))));
        }

        [Test]
        [TestCase("0", 0)]
        [TestCase("1", 1)]
        [TestCase("-1", -1)]
        [TestCase("2147483648", 2147483648)]
        [TestCase("-2147483649", -2147483649)]
        [TestCase("9223372036854775807", 9223372036854775807)]
        [TestCase("-9223372036854775808", -9223372036854775808)]
        public void ShouldParseLongs(string input, long expected)
        {
            var actual = NumberParser.ParseLong(input);

            Assert.AreEqual(expected, actual);
            _logMock.Verify(log => log.Debug(It.Is<object>(o => o.ToString().Contains(input))));
        }

        [Test]
        public void ShouldParseDoubleWithoutProvidedLogger()
        {
            NumberParser.SetLogger();

            Assert.AreEqual(5.5, NumberParser.ParseDouble("5.5"));
        }

        [Test]
        public void ShouldParseDecimalWithoutProvidedLogger()
        {
            NumberParser.SetLogger();

            Assert.AreEqual(5.5M, NumberParser.ParseDecimal("5.5"));
        }

        [Test]
        public void ShouldParseIntWithoutProvidedLogger()
        {
            NumberParser.SetLogger();

            Assert.AreEqual(5, NumberParser.ParseInt("5"));
        }

        [Test]
        public void ShouldParseLongWithoutProvidedLogger()
        {
            NumberParser.SetLogger();

            Assert.AreEqual(5L, NumberParser.ParseLong("5"));
        }
    }
}
