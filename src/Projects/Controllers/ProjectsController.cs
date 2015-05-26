using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Projects.Contracts.Commands;
using Projects.Domain;

namespace Projects.Controllers
{
    [RoutePrefix("api/projects")]
    public class ProjectsController : ApiController
    {
        private readonly IProjectApplicationService _projectApplicationService;

        public ProjectsController(IProjectApplicationService projectApplicationService)
        {
            _projectApplicationService = projectApplicationService;
        }

        [HttpPost]
        [Route("")]
        public HttpResponseMessage Create(CreateProjectRequest req)
        {
            var id = _projectApplicationService.Create(new CreateProject
            {
                Name = req.Name
            });
            return Request.CreateResponse(HttpStatusCode.Created, new {ProjectId = id});
        }

        [HttpPost]
        [Route("{projectId}/cem")]
        public void SetClientEngagementManager(Guid projectId, [FromBody]SetCemRequest req)
        {
            _projectApplicationService.Execute(new SetCem{Id = projectId, StaffId = req.StaffId});
        }

        [HttpPost]
        [Route("{projectId}/pm")]
        public void SetProjectManager(Guid projectId, [FromBody]SetPmRequest req)
        {
            _projectApplicationService.Execute(new SetPm{Id = projectId, StaffId = req.StaffId});
        }

        [HttpPost]
        [Route("{projectId}/teamMembers/add")]
        public void AddTeamMembers(Guid projectId, [FromBody]AddTeamMembersRequest req)
        {
            _projectApplicationService.Execute(new AddTeamMembers { Id = projectId, StaffIds = req.StaffIds });
        }

        [HttpPost]
        [Route("{projectId}/teamMembers/remove")]
        public void RemoveTeamMembers(Guid projectId, [FromBody]RemoveTeamMembersRequest req)
        {
            _projectApplicationService.Execute(new RemoveTeamMembers { Id = projectId, StaffIds = req.StaffIds });
        }

        [HttpPost]
        [Route("{projectId}/metrics/add")]
        public void AddMetrics(Guid projectId, [FromBody]AddMetricsRequest req)
        {
            _projectApplicationService.Execute(new AddMetrics { Id = projectId, MetricIds = req.MetricIds });
        }

        [HttpPost]
        [Route("{projectId}/metrics/remove")]
        public void RemoveMetrics(Guid projectId, [FromBody]RemoveMetricsRequest req)
        {
            _projectApplicationService.Execute(new RemoveMetrics { Id = projectId, MetricIds = req.MetricIds });
        }

        [HttpPost]
        [Route("{projectId}/suspend")]
        public void Suspend(Guid projectId)
        {
            _projectApplicationService.Execute(new SuspendProject { Id = projectId });
        }
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