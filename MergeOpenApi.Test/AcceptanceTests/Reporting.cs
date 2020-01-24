using NUnit.Framework;
using TestStack.BDDfy.Configuration;
using TestStack.BDDfy.Reporters.Html;

namespace MergeOpenApi.Test.AcceptanceTests
{
    public class Reporting
    {
        [OneTimeSetUp]
        public void SetupReporting()
        {
            Configurator.BatchProcessors.HtmlReport.Disable();

            var report = new DefaultHtmlReportConfiguration();
            report.OutputFileName = typeof(Program).Namespace + ".AcceptanceTests.html";
            report.ReportHeader = typeof(Program).Namespace + " acceptance tests";
            report.ReportDescription = "Automated tests of business critical functionality";

            Configurator.BatchProcessors.Add(new HtmlReporter(report));
        }
    }
}