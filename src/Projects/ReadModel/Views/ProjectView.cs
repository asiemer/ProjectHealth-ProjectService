using System;
using Projects.Contracts.Enums;

namespace Projects.ReadModel.Views
{
    public class ProjectView
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ProjectStatus Status { get; set; }
    }
}