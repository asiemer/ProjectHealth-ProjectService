using System;
using System.Collections.Generic;
using Projects.Contracts.Enums;

namespace Projects.ReadModel.Views
{
    public class ProjectView
    {
        public ProjectView()
        {
            Team = new List<Guid>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public ProjectStatus Status { get; set; }
        public Guid? CemId { get; set; }
        public Guid? PmId { get; set; }
        public List<Guid> Team { get; set; }
        public List<MetricView> Metrics { get; set; }
    }

    public class MetricView
    {
        public Guid MetricId { get; set; }
        public bool IsDefault { get; set; }
    }
}