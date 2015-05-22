using System;

namespace Projects.Contracts.Events
{
    public class ProjectCreated
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}