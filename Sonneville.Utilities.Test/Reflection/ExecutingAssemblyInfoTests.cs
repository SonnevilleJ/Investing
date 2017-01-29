using System.Reflection;
using NUnit.Framework;
using Sonneville.Utilities.Reflection;

namespace Sonneville.Utilities.Test.Reflection
{
    [TestFixture]
    public class ExecutingAssemblyInfoTests
    {
        [Test]
        public void ShouldGetCustomAttribute()
        {
            var expected = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyTitleAttribute>().Title;

            var actual = ExecutingAssemblyInfo.GetAssemblyAttribute<AssemblyTitleAttribute>().Title;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldGetAssemblyCompany()
        {
            var expected = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyCompanyAttribute>().Company;

            var actual = ExecutingAssemblyInfo.Company;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldGetAssemblyCopyright()
        {
            var expected = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyCopyrightAttribute>().Copyright;

            var actual = ExecutingAssemblyInfo.Copyright;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldGetAssemblyDescription()
        {
            var expected = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyDescriptionAttribute>().Description;

            var actual = ExecutingAssemblyInfo.Description;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldGetAssemblyProduct()
        {
            var expected = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyProductAttribute>().Product;

            var actual = ExecutingAssemblyInfo.Product;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldGetAssemblyTitle()
        {
            var expected = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyTitleAttribute>().Title;

            var actual = ExecutingAssemblyInfo.Title;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldGetAssemblyTrademark()
        {
            var expected = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyTrademarkAttribute>().Trademark;

            var actual = ExecutingAssemblyInfo.Trademark;

            Assert.AreEqual(expected, actual);
        }
    }
}