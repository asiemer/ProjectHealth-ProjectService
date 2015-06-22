using System;

namespace Projects.Contracts.Commands
{
    public class CreateProject
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class SetCem
    {
        public Guid Id { get; set; }
        public Guid StaffId { get; set; }
    }

    public class SetPm
    {
        public Guid Id { get; set; }
        public Guid StaffId { get; set; }
    }

    public class AddTeamMembers
    {
        public Guid Id { get; set; }
        public Guid[] StaffIds { get; set; }
    }

    public class RemoveTeamMembers
    {
        public Guid Id { get; set; }
        public Guid[] StaffIds { get; set; }
    }

    public class AddMetrics
    {
        public Guid Id { get; set; }
        public Guid[] MetricIds { get; set; }
    }

    public class RemoveMetrics
    {
        public Guid Id { get; set; }
        public Guid[] MetricIds { get; set; }
    }

    public class UpdateMetric
    {
        public Guid Id { get; set; }
        public Guid MetricId { get; set; }
        public int Value { get; set; }
    }

    public class SuspendProject
    {
        public Guid Id { get; set; }
    }
}