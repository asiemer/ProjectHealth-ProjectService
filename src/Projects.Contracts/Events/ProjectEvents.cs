using System;

namespace Projects.Contracts.Events
{
    public class ProjectCreated
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public MetricInfo[] DefaultMetrics { get; set; }
    }

    public class MetricInfo
    {
        public Guid MetricId { get; set; }
        public bool IsDefault { get; set; }
        public int Weight { get; set; }
        public int Value { get; set; }
    }

    public class CemSet
    {
        public Guid Id { get; set; }
        public Guid StaffId { get; set; }
    }

    public class PmSet
    {
        public Guid Id { get; set; }
        public Guid StaffId { get; set; }
    }

    public class TeamMembersAdded
    {
        public Guid Id { get; set; }
        public Guid[] StaffIds { get; set; }
    }

    public class TeamMembersRemoved
    {
        public Guid Id { get; set; }
        public Guid[] StaffIds { get; set; }
    }

    public class MetricsAdded
    {
        public Guid Id { get; set; }
        public MetricInfo[] Metrics { get; set; }
    }

    public class MetricsRemoved
    {
        public Guid Id { get; set; }
        public MetricInfo[] Metrics { get; set; }
    }

    public class MetricUpdated
    {
        public Guid Id { get; set; }
        public MetricInfo Metric { get; set; }
    }

    public class ProjectSuspended
    {
        public Guid Id { get; set; }
    }
}