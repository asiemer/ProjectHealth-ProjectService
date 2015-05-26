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
        private readonly IProjectsApplicationService _projectsApplicationService;

        public ProjectsController(IProjectsApplicationService projectsApplicationService)
        {
            _projectsApplicationService = projectsApplicationService;
        }

        [HttpPost]
        [Route("")]
        public HttpResponseMessage Create(CreateProjectRequest req)
        {
            var id = _projectsApplicationService.When(new CreateProject
            {
                Name = req.Name
            });
            return Request.CreateResponse(HttpStatusCode.Created, new {ProjectId = id});
        }

        [HttpPost]
        [Route("{projectId}/cem")]
        public void SetClientEngagementManager(Guid projectId, [FromBody]SetCemRequest req)
        {
            _projectsApplicationService.Execute(new SetCem{Id = projectId, StaffId = req.StaffId});
        }

        [HttpPost]
        [Route("{projectId}/pm")]
        public void SetProjectManager(Guid projectId, [FromBody]SetPmRequest req)
        {
            _projectsApplicationService.Execute(new SetPm{Id = projectId, StaffId = req.StaffId});
        }

        [HttpPost]
        [Route("{projectId}/team/add")]
        public void AddTeamMembers(Guid projectId, [FromBody]AddTeamMembersRequest req)
        {
            _projectsApplicationService.Execute(new AddTeamMembers { Id = projectId, StaffIds = req.StaffIds });
        }

        [HttpPost]
        [Route("{projectId}/team/remove")]
        public void RemoveTeamMembers(Guid projectId, [FromBody]RemoveTeamMembersRequest req)
        {
            _projectsApplicationService.Execute(new RemoveTeamMembers { Id = projectId, StaffIds = req.StaffIds });
        }

        [HttpPost]
        [Route("{projectId}/metrics/add")]
        public void AddMetrics(Guid projectId, [FromBody]AddMetricsRequest req)
        {
            _projectsApplicationService.Execute(new AddMetrics { Id = projectId, MetricIds = req.MetricIds });
        }

        [HttpPost]
        [Route("{projectId}/metrics/remove")]
        public void RemoveMetrics(Guid projectId, [FromBody]RemoveMetricsRequest req)
        {
            _projectsApplicationService.Execute(new RemoveMetrics { Id = projectId, MetricIds = req.MetricIds });
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