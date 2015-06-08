using System;
using System.IO;
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
        public void first_metric_should_equal()
        {
            _metricsProviderMock.GetCompanyMetrics();
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