using System;
using System.IO;
using Projects.Services;

namespace IntegrationTests.Services.MetricsProviderMockTests
{
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