using System.Linq;
using NUnit.Framework;
using Projects.Services;

namespace IntegrationTests.Services.MetricsProviderMockTests
{
    public class when_creating_company_metrics_from_blank_value_in_BlankValue_File
    {
        private MetricsProviderMock _metricsProviderMock;
        private Metric[] _metrics;

        [SetUp]
        protected void When()
        {
            var fakeMetricsFileGetter = new FakeMetricsFileGetter("BlankValue.csv");
            _metricsProviderMock = new MetricsProviderMock(fakeMetricsFileGetter);
        }

        [Then]
        public void error_should_be_thrown()
        {
            var testDelegate = new TestDelegate(MethodThatShouldThrow);
            Assert.Catch(testDelegate, "Check file to make sure all columns have values. Do not leave any blank.");
        }

        private void MethodThatShouldThrow()
        {
            _metricsProviderMock.GetCompanyMetrics();
        }
    }
}