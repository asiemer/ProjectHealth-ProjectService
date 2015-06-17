using System;

namespace Projects.Contracts.Events
{
    public class ProjectScoreChanged
    {
        public Guid Id { get; set; }
        public int Red { get; set; }
        public int Yellow { get; set; }
        public int Green { get; set; }
    }
}