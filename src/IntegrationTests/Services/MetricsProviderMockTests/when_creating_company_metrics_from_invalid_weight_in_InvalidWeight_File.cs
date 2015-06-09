using NUnit.Framework;
using Projects.Services;

namespace IntegrationTests.Services.MetricsProviderMockTests
{
    public class when_creating_company_metrics_from_invalid_weight_in_InvalidWeight_File
    {
        private MetricsProviderMock _metricsProviderMock;
        private Metric[] _metrics;

        [SetUp]
        protected void When()
        {
            var fakeMetricsFileGetter = new FakeMetricsFileGetter("InvalidWeight.csv");
            _metricsProviderMock = new MetricsProviderMock(fakeMetricsFileGetter);
        }

        [Then]
        public void CreateDefaultCompanyMetric_should_throw_format_exception()
        {
            var testDelegate = new TestDelegate(MethodThatShouldThrow);
            Assert.Catch(testDelegate, "One of the columns has values entered in the incorrect format.");
        }

        private void MethodThatShouldThrow()
        {
            _metricsProviderMock.GetCompanyMetrics();
        }
    }
}