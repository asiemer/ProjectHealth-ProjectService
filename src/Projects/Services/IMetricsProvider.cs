using System;

namespace Projects.Services
{
    public interface IMetricsProvider
    {
        Metric[] GetCompanyMetrics();
    }

    public class MetricsProviderMock : IMetricsProvider
    {
        public Metric[] GetCompanyMetrics()
        {
            return new []
            {
                new Metric{Id = Guid.NewGuid(), Name = "Has unit tests", IsDefault = true, Weight = 3, AllowedAge = 2}, 
                new Metric{Id = Guid.NewGuid(), Name = "Has integration tests", IsDefault = true, Weight = 2, AllowedAge = 2}, 
                new Metric{Id = Guid.NewGuid(), Name = "Has CI build", IsDefault = true, Weight = 4, AllowedAge = 2}, 
                new Metric{Id = Guid.NewGuid(), Name = "Code in repo on GitHub/BitBucket/VSO", IsDefault = true, Weight = 4, AllowedAge = 1}, 
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
    }
}