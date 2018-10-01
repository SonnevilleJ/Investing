using System;
using System.IO;
using System.Linq;
using log4net;
using log4net.Appender;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using Optional;
using Sonneville.Utilities;

namespace Sonneville.Investing.Fidelity.WebDriver.Logging
{
    public interface IExceptionReportGenerator
    {
        Option<string> DocumentException(Exception exception);
    }
    
    public class ExceptionReportGenerator : IExceptionReportGenerator, IDisposable
    {
        private readonly ILog _log;
        private readonly RemoteWebDriver _webDriver;
        private readonly string _pathRoot;
        private readonly IClock _clock;

        public ExceptionReportGenerator(ILog log, RemoteWebDriver webDriver, string pathRoot, IClock clock)
        {
            _log = log;
            _webDriver = webDriver;
            _pathRoot = pathRoot;
            _clock = clock;
            
            _log.Info($"Exception reports will be generated in {pathRoot}");
        }

        public Option<string> DocumentException(Exception exception)
        {
            try
            {
                var exceptionTime = _clock.Now;
                var reportPath = CreateReportPath(exceptionTime);

                SaveException(reportPath, exception);
                SaveScreenshot(reportPath, _webDriver);
                SavePageSource(reportPath, _webDriver);
                CopyLogs(reportPath);
                
                _log.Info($"Wrote exception report to: {reportPath}");
                return Option.Some(reportPath);
            }
            catch (Exception e)
            {
                _log.ErrorFormat("Exception occurred during report generation: {0}", e);
                return Option.None<string>();
            }
        }

        private string CreateReportPath(DateTime exceptionTime)
        {
            var reportPath = Path.Combine(_pathRoot, $"Exception-{exceptionTime:O}");
            if (!Directory.Exists(reportPath)) Directory.CreateDirectory(reportPath);
            return reportPath;
        }

        private static void SaveException(string reportPath, Exception exception)
        {
            File.WriteAllText($"{reportPath}/exception.txt", exception.ToString());
        }

        private static void SaveScreenshot(string reportPath, ITakesScreenshot takesScreenshot)
        {
            var fileName = $"{reportPath}/screenshot.png";
            takesScreenshot.GetScreenshot().SaveAsFile(fileName);
        }

        private static void SavePageSource(string reportPath, IWebDriver webDriver)
        {
            File.WriteAllText($"{reportPath}/source.html", webDriver.PageSource);
        }

        private static void CopyLogs(string reportPath)
        {
            var logs = LogManager.GetAllRepositories()
                .SelectMany(repository => repository.GetAppenders())
                .OfType<FileAppender>();
            foreach (var log in logs)
            {
                log.Flush(0);
                File.Copy(log.File, $"{reportPath}/{Path.GetFileName(log.File)}");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _webDriver?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}