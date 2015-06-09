using System.IO;
using System.Reflection;
using Projects.Services;

namespace IntegrationTests.Services.MetricsProviderMockTests
{
    public class FakeMetricsFileGetter : IMetricsFileGetter
    {
        private readonly string _fileName;
        private const string TestFilesNamespace = "IntegrationTests.TestFiles";

        public FakeMetricsFileGetter(string fileName)
        {
            _fileName = fileName;
        }

        public StreamReader GetMetricsFile()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName =  TestFilesNamespace + "." + _fileName;
            var stream = assembly.GetManifestResourceStream(resourceName);
            return new StreamReader(stream);
        }
    }
}