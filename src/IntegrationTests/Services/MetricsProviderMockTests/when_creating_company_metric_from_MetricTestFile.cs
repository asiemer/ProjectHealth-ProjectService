using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Projects.Services;

namespace IntegrationTests.Services.MetricsProviderMockTests
{
    public class when_creating_company_metric_from_MetricTestFile
    {
        private MetricsProviderMock _metricsProviderMock;

        [SetUp]
        protected void When()
        {
            var fakeMetricsFileGetter = new FakeMetricsFileGetter("Metrics.csv");
            _metricsProviderMock = new MetricsProviderMock(fakeMetricsFileGetter);
        }

        [Then]
        public void there_should_be_three_valid_metrics()
        {
            var metrics = _metricsProviderMock.GetCompanyMetrics();
            Assert.That(metrics.Count(), Is.EqualTo(3));
        }
    }

    public class FakeMetricsFileGetter : IMetricsFileGetter
    {
        private readonly string _fileName;

        public FakeMetricsFileGetter(string fileName)
        {
            _fileName = fileName;
        }

        public StreamReader GetMetricsFile()
        {
            var fullPath = AppDomain.CurrentDomain.BaseDirectory + "\\TestFiles\\" + _fileName;
            return new StreamReader(File.OpenRead(@fullPath));
        }
    }
}