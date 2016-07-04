using System.IO;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Sonneville.Fidelity.Shell.AppStartup;

namespace Sonneville.Fidelity.Shell.Test.AppStartup
{
    [TestFixture]
    public class AppTests
    {
        private App _app;
        private Mock<IAccountRebalancer> _accountRebalancerMock;
        private string[] _cliArgs;
        private StreamWriter _inputWriter;
        private StreamReader _outputReader;
        private Task _task;

        [SetUp]
        public void Setup()
        {
            _cliArgs = new string[0];

            _accountRebalancerMock = new Mock<IAccountRebalancer>();

            var inputStream = new MemoryStream();
            _inputWriter = new StreamWriter(inputStream) {AutoFlush = true};

            var outputStream = new MemoryStream();
            _outputReader = new StreamReader(outputStream);

            _app = new App(_accountRebalancerMock.Object, new StreamReader(inputStream),
                new StreamWriter(outputStream) {AutoFlush = true});
        }

        [TearDown]
        public void Teardown()
        {
            _inputWriter.Dispose();
            _outputReader.Dispose();
            _app.Dispose();
        }

        [Test]
        public void DisposeShouldNotThrow()
        {
            _app.Dispose();
            _app.Dispose();

            _accountRebalancerMock.Verify(rebalancer => rebalancer.Dispose());
        }

        [Test]
        public void RunShouldWaitForExitCommand()
        {
            _task = Task.Run(() => _app.Run(_cliArgs));
            _task.Wait(1000);
            Assert.IsFalse(_task.IsCompleted);

            SendCommand("exit");

            Assert.IsTrue(_task.IsCompleted);
        }

        [Test]
        public void HelpCommandShouldPrintHelp()
        {
            _task = Task.Run(() => _app.Run(_cliArgs));

            SendCommand("help");

            var lines = ReadOutput();
            AssertHelpWasPrinted(lines);
        }

        [Test]
        public void UnknownCommandShouldPrintHelp()
        {
            _task = Task.Run(() => _app.Run(_cliArgs));

            SendCommand("asdf");

            var lines = ReadOutput();
            Assert.IsTrue(lines.Contains("Unknown"));
            AssertHelpWasPrinted(lines);
        }

        private void SendCommand(string format)
        {
            _inputWriter.BaseStream.Position = 0;
            _inputWriter.WriteLine(format);
            _inputWriter.BaseStream.Position = 0;
            _task.Wait(1000);
        }

        private string ReadOutput()
        {
            _outputReader.BaseStream.Position = 0;
            return _outputReader.ReadToEnd();
        }

        private void AssertHelpWasPrinted(string lines)
        {
            Assert.IsTrue(lines.Contains("usage"), $"Output wasn't as expected. Actual output: {lines}");
        }
    }
}