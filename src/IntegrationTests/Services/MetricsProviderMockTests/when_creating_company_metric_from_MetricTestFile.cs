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
        private Metric[] _metrics;

        [SetUp]
        protected void When()
        {
            var fakeMetricsFileGetter = new FakeMetricsFileGetter("Metrics.csv");
            _metricsProviderMock = new MetricsProviderMock(fakeMetricsFileGetter);
            _metrics = _metricsProviderMock.GetCompanyMetrics();
        }

        [Then]
        public void there_should_be_three_valid_metrics()
        {
            Assert.That(_metrics.Count(), Is.EqualTo(3));
        }

        [Then]
        public void first_metrics_name_should_be_Metric_1()
        {
            Assert.That(_metrics.First().Name, Is.EqualTo("Metric 1"));
        }

        [Then]
        public void first_metrics_IsDefault_should_be_true()
        {
            Assert.That(_metrics.First().IsDefault);
        }

        [Then]
        public void last_metrics_weight_should_be_negative_50()
        {
            Assert.That(_metrics.Last().Weight, Is.EqualTo(-50));
        }

        [Then]

        public void last_metrics_allowedAge_should_be_zero()
        {
            Assert.That(_metrics.Last().AllowedAge, Is.EqualTo(1));
        }

        [Then]
        public void last_metrics_requiresAlert_should_be_zero()
        {
            Assert.That(!_metrics.Last().RequiresAlert);
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