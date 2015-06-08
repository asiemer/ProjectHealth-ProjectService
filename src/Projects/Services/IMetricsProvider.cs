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
            try
            {
                AddMetricsFromFile(reader, companyMetrics);
            }
            catch (Exception e)
            {
                //Handle error better
                throw new Exception ("One of the columns has values that are formatted incorrectly.");
            }
            

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

        //This could be pulled out into a static mapper.
        public Metric CreateDefaultCompanyMetric(string[] metricRow)
        {
            var metricToReturn = new Metric();
            metricToReturn.Id = Guid.NewGuid();
            metricToReturn.Name = metricRow[0];
            metricToReturn.IsDefault = (metricRow[1] == "TRUE");
            metricToReturn.Weight = Convert.ToInt32(metricRow[2]);
            metricToReturn.AllowedAge = Convert.ToInt32(metricRow[3]);
            metricToReturn.RequiresAlert = (metricRow[4] == "TRUE");


            return metricToReturn;
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