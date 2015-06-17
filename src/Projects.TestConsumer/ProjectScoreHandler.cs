using System;
using NServiceBus;
using Projects.Contracts.Events;

namespace Projects.TestConsumer
{
    public class ProjectScoreHandler : IHandleMessages<ProjectScoreChanged>
    {
        public void Handle(ProjectScoreChanged message)
        {
            Console.WriteLine("Project score changed - ID={0}, red={1}, yellow={2}, green={3}", 
                message.Id, message.Red, message.Yellow, message.Green);
        }
    }
}