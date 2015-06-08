using System;
using System.Collections.Generic;
using System.IO;

namespace Projects.Services
{
    public interface IMetricsProvider
    {
        Metric[] GetCompanyMetrics();
    }

    public class MetricsProviderMock : IMetricsProvider
    {
        private readonly IMetricsFileGetter _metricsFileGetter;

        public MetricsProviderMock(IMetricsFileGetter metricsFileGetter)
        {
            _metricsFileGetter = metricsFileGetter;
        }

        public Metric[] GetCompanyMetrics()
        {
            var reader = _metricsFileGetter.GetMetricsFile();
            var companyMetrics = new List<Metric>();
            
            //Skip header row
            var headerLine = reader.ReadLine();
            AddMetricsFromFile(reader, companyMetrics);
            return companyMetrics.ToArray();
        }

        private void AddMetricsFromFile(StreamReader reader, List<Metric> companyMetrics)
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line != null)
                {
                    companyMetrics.Add(CreateDefaultCompanyMetric(line.Split(',')));
                }
            }
        }

        public Metric CreateDefaultCompanyMetric(string[] metricRow)
        {
             return new Metric
            {
                Id = Guid.NewGuid(),
                Name = metricRow[0],
                IsDefault = (metricRow[1] == "TRUE"),
                Weight = Convert.ToInt32(metricRow[2]),
                AllowedAge = Convert.ToInt32(metricRow[3]),
                RequiresAlert = (metricRow[4] == "TRUE")
            };
        }
    }

    public class Metric
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Weight { get; set; }
        public bool IsDefault { get; set; }
        public int AllowedAge { get; set; }
        public bool RequiresAlert { get; set; }
        public int Value { get; set; }
    }
}