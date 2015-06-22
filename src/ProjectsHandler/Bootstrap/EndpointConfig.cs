using NServiceBus;

namespace ProjectsHandler.Bootstrap
{
    /*
		This class configures this endpoint as a Server. More information about how to configure the NServiceBus host
		can be found here: http://particular.net/articles/the-nservicebus-host
	*/
    public class EndpointConfig : IConfigureThisEndpoint
    {
        public void Customize(BusConfiguration configuration)
        {
            var container = Bootstrap.Init();

            configuration.UseContainer<StructureMapBuilder>(c => c.ExistingContainer(container));
            configuration.EndpointName("Heartbeat.Projects.Handler");
            configuration.UsePersistence<NHibernatePersistence>();
            configuration.Conventions().DefiningCommandsAs(t => t.Namespace != null && t.Namespace.StartsWith("Projects.Contracts.Commands"));
            configuration.Conventions().DefiningEventsAs(t => t.Namespace != null && t.Namespace.StartsWith("Projects.Contracts.Events"));
        }
    }
}
