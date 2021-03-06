using System;
using System.IO;
using System.Reflection;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using Moq;
using NUnit.Framework;
using OpenQA.Selenium;
using Optional;
using Sonneville.Utilities;

namespace Sonneville.Selenium.log4net.Test
{
    [TestFixture]
    public class ExceptionReportGeneratorTests
    {
        private ExceptionReportGenerator _exceptionReportGenerator;
        private Mock<IWebDriver> _mockWebDriver;
        private Mock<ITakesScreenshot> _mockTakesScreenshot;
        private FreezableClock _clock;
        private Mock<ILog> _mockLog;
        private string _pathRoot;
        private Screenshot _screenshot;
        private string _pageSource;

        [SetUp]
        public void Setup()
        {
            _mockLog = new Mock<ILog>();

            _pageSource = "page source";

            _mockWebDriver = new Mock<IWebDriver>();
            _mockWebDriver.Setup(webDriver => webDriver.PageSource)
                .Returns(_pageSource);

            _screenshot = new Screenshot("");

            _mockTakesScreenshot = new Mock<ITakesScreenshot>();
            _mockTakesScreenshot.Setup(screenshot => screenshot.GetScreenshot())
                .Returns(_screenshot);

            _pathRoot = Path.Combine(Path.GetTempPath(), "Test Reports");
            CreateEmptyDirectory(_pathRoot);

            _clock = new FreezableClock();

            _exceptionReportGenerator = new ExceptionReportGenerator(
                _mockLog.Object,
                _mockWebDriver.Object,
                _mockTakesScreenshot.Object,
                _pathRoot,
                _clock);
        }

        [TearDown]
        public void Teardown()
        {
            TeardownLogger();

            _clock.Unfreeze();

            DeleteDirectory(_pathRoot);
        }

        [Test]
        public void ShouldDispose()
        {
            _exceptionReportGenerator.Dispose();

            _mockWebDriver.Verify(webDriver => webDriver.Dispose());
        }

        [Test]
        public void ShouldLogReportsDirectoryOnStartup()
        {
            _mockLog.Verify(log => log.Info(It.Is<string>(content => content.Contains(_pathRoot))));
        }

        [Test]
        public void ShouldReplaceInvalidPathCharsInReportPath()
        {
            _clock.Freeze(DateTime.Now);

            var reportPath = _exceptionReportGenerator.DetermineReportPath(_clock.Now);

            Assert.Throws<DirectoryNotFoundException>(() => new DirectoryInfo(reportPath).EnumerateFileSystemInfos());
        }

        [Test]
        public void ShouldCreateFolderForException()
        {
            Assert.AreEqual(0, Directory.GetDirectories(_pathRoot).Length);
            _clock.Freeze(DateTime.Now);
            var reportPath = DetermineReportPath(_clock.Now);

            var result = _exceptionReportGenerator.DocumentException(CreateTestException());

            DirectoryAssert.Exists(reportPath, "FAIL: Report directory not found!");
            Assert.AreEqual(1, Directory.GetDirectories(_pathRoot).Length);
            Assert.AreEqual(Option.Some(reportPath), result);
        }

        [Test]
        public void ShouldLogWhenExceptionOccursDuringReportGeneration()
        {
            Assert.AreEqual(0, Directory.GetDirectories(_pathRoot).Length);
            _clock.Freeze(DateTime.Now);
            var exception = new Exception();
            _mockLog.Setup(log => log.Info(It.IsAny<string>()))
                .Throws(exception);

            var result = _exceptionReportGenerator.DocumentException(CreateTestException());

            _mockLog.Verify(log => log.ErrorFormat("Exception occurred during report generation: {0}", exception));
            Assert.AreEqual(Option.None<string>(), result);
        }

        [Test]
        public void ShouldIncludeExceptionArtifact()
        {
            _clock.Freeze(DateTime.Now);
            var reportPath = DetermineReportPath(_clock.Now);
            var testException = CreateTestException();

            _exceptionReportGenerator.DocumentException(testException);

            var exceptionTextFile = GetReportFilePath(reportPath, "exception.txt");
            FileAssert.Exists(exceptionTextFile, "FAIL: Exception file not found!");
            Assert.IsTrue(File.ReadAllText(exceptionTextFile).Contains(testException.ToString()));
        }

        [Test]
        public void ShouldIncludeScreenshotArtifact()
        {
            _clock.Freeze(DateTime.Now);
            var reportPath = DetermineReportPath(_clock.Now);
            var testException = CreateTestException();

            _exceptionReportGenerator.DocumentException(testException);

            FileAssert.Exists(GetReportFilePath(reportPath, "screenshot.png"), "FAIL: Screenshot file not found!");
        }

        [Test]
        public void ShouldIncludePageSourceArtifact()
        {
            _clock.Freeze(DateTime.Now);
            var reportPath = DetermineReportPath(_clock.Now);
            var testException = CreateTestException();

            _exceptionReportGenerator.DocumentException(testException);

            var sourceTextFile = GetReportFilePath(reportPath, "source.html");
            FileAssert.Exists(sourceTextFile, "FAIL: HTML source file not found!");
            Assert.IsTrue(File.ReadAllText(sourceTextFile).Contains(_pageSource));
        }

        [Test]
        public void ShouldIncludeLogsArtifact()
        {
            _clock.Freeze(DateTime.Now);
            var reportPath = DetermineReportPath(_clock.Now);
            SetupLogger(Path.Combine(_pathRoot, "LogSource", "Demo.log"));
            LogManager.GetLogger(typeof(ExceptionReportGenerator)).Info("sample log entry");

            _exceptionReportGenerator.DocumentException(CreateTestException());

            FileAssert.Exists(GetReportFilePath(reportPath, "Demo.log"), "FAIL: Log file not found!");
        }

        [Test]
        public void ShouldLogLocationWhenCompleted()
        {
            _clock.Freeze(DateTime.Now);
            var reportPath = DetermineReportPath(_clock.Now);
            LogManager.GetLogger(typeof(ExceptionReportGenerator)).Info("sample log entry");

            _exceptionReportGenerator.DocumentException(CreateTestException());

            _mockLog.Verify(log => log.Info(It.Is<string>(content => content.Contains(reportPath))));
        }

        private string GetReportFilePath(string reportPath, string filename)
        {
            return Path.Combine(_pathRoot, reportPath, filename);
        }

        private string DetermineReportPath(DateTime time)
        {
            return _exceptionReportGenerator.DetermineReportPath(time);
        }

        private static Exception CreateTestException()
        {
            return new Exception("test exception");
        }

        private static void SetupLogger(string logFilename)
        {
            LayoutSkeleton layout = new PatternLayout
            {
                ConversionPattern = "%date [%thread] %-5level %logger - %message%newline"
            };
            layout.ActivateOptions();

            AppenderSkeleton appender = new RollingFileAppender
            {
                Name = "Default logger",
                AppendToFile = true,
                File = logFilename,
                Layout = layout,
                MaxSizeRollBackups = 5,
                MaximumFileSize = "1MB",
                RollingStyle = RollingFileAppender.RollingMode.Size,
                StaticLogFileName = true,
                ImmediateFlush = true,
                Threshold = Level.All,
            };
            appender.ActivateOptions();

            var hierarchy = (Hierarchy) LogManager.GetRepository(Assembly.GetEntryAssembly());
            hierarchy.Root.AddAppender(appender);
            hierarchy.Root.Level = Level.All;
            hierarchy.Configured = true;
        }

        private static void TeardownLogger()
        {
            LogManager.Shutdown();
        }

        private static void CreateEmptyDirectory(string path)
        {
            DeleteDirectory(path);
            Directory.CreateDirectory(path);
        }

        private static void DeleteDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }
    }
}