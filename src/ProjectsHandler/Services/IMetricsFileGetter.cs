using System;
using System.IO;

namespace Projects.Services
{
    public interface IMetricsFileGetter
    {
        StreamReader GetMetricsFile();
    }

    public class MetricsFileGetter : IMetricsFileGetter
    {
        public StreamReader GetMetricsFile()
        {
            //Placed here since this is only a temporary location
            var fullPath =  AppDomain.CurrentDomain.BaseDirectory + "\\Metrics.csv";
            return new StreamReader(File.OpenRead(@fullPath));
        }
    }

}