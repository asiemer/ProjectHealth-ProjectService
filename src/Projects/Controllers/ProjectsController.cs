using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using NServiceBus;
using Projects.Contracts.Commands;

namespace Projects.Controllers
{
    [RoutePrefix("api/projects")]
    public class ProjectsController : ApiController
    {
        private readonly IBus _bus;

        public ProjectsController(IBus bus)
        {
            _bus = bus;
        }

        [HttpPost]
        [Route("")]
        public HttpResponseMessage Create(CreateProjectRequest req)
        {
            var id = Guid.NewGuid();
            _bus.Send(new CreateProject { Id = id, Name = req.Name });
            return Request.CreateResponse(HttpStatusCode.Created, new { ProjectId = id });
        }

        [HttpPost]
        [Route("{projectId}/cem")]
        public void SetClientEngagementManager(Guid projectId, [FromBody]SetCemRequest req)
        {
            _bus.Send(new SetCem{Id = projectId, StaffId = req.StaffId});
        }

        [HttpPost]
        [Route("{projectId}/pm")]
        public void SetProjectManager(Guid projectId, [FromBody]SetPmRequest req)
        {
            _bus.Send(new SetPm{Id = projectId, StaffId = req.StaffId});
        }

        [HttpPost]
        [Route("{projectId}/teamMembers/add")]
        public void AddTeamMembers(Guid projectId, [FromBody]AddTeamMembersRequest req)
        {
            _bus.Send(new AddTeamMembers { Id = projectId, StaffIds = req.StaffIds });
        }

        [HttpPost]
        [Route("{projectId}/teamMembers/remove")]
        public void RemoveTeamMembers(Guid projectId, [FromBody]RemoveTeamMembersRequest req)
        {
            _bus.Send(new RemoveTeamMembers { Id = projectId, StaffIds = req.StaffIds });
        }

        [HttpPost]
        [Route("{projectId}/metrics/add")]
        public void AddMetrics(Guid projectId, [FromBody]AddMetricsRequest req)
        {
            _bus.Send(new AddMetrics { Id = projectId, MetricIds = req.MetricIds });
        }

        [HttpPost]
        [Route("{projectId}/metrics/remove")]
        public void RemoveMetrics(Guid projectId, [FromBody]RemoveMetricsRequest req)
        {
            _bus.Send(new RemoveMetrics { Id = projectId, MetricIds = req.MetricIds });
        }

        [HttpPost]
        [Route("{projectId}/metrics/update/{metricId}/value")]
        public void UpdateMetrics(Guid projectId, Guid metricId, [FromBody]UpdateMetricsRequest req)
        {
             _bus.Send(new UpdateMetric { Id = projectId, MetricId = metricId, Value = req.Value });
        }

        [HttpPost]
        [Route("{projectId}/suspend")]
        public void Suspend(Guid projectId)
        {
            _bus.Send(new SuspendProject { Id = projectId });
        }
    }

    public class UpdateMetricsRequest
    {
        public Guid MetricId { get; set; }
        public int Value { get; set; }
    }

    public class RemoveMetricsRequest
    {
        public Guid[] MetricIds { get; set; }
    }

    public class AddMetricsRequest
    {
        public Guid[] MetricIds { get; set; }
    }

    public class RemoveTeamMembersRequest
    {
        public Guid[] StaffIds { get; set; }
    }

    public class AddTeamMembersRequest
    {
        public Guid[] StaffIds { get; set; }
    }

    public class SetPmRequest
    {
        public Guid StaffId { get; set; }
    }

    public class SetCemRequest
    {
        public Guid StaffId { get; set; }
    }

    public class CreateProjectRequest
    {
        public string Name { get; set; }
    }
}