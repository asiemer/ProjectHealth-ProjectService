using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Projects.Contracts.Enums;
using Projects.Contracts.Events;
using Projects.Domain;
using Projects.Infrastructure;
using Projects.ReadModel.Views;

namespace Projects.ReadModel.Observers
{
    public class ProjectsObserver
    {
        private readonly IProjectionWriter<ProjectView> _writer;

        public ProjectsObserver(IProjectionWriter<ProjectView> writer)
        {
            _writer = writer;
        }

        public async Task When(ProjectCreated e)
        {
            var projectView = new ProjectView
            {
                Id = e.Id,
                Name = e.Name,
                Status = ProjectStatus.Active,
                Metrics = CreateMetricViews(e.DefaultMetrics)
            };

            await _writer.Add(e.Id, projectView);
        }

        private List<MetricView> CreateMetricViews(MetricInfo[] defaultMetrics)
        {
            var metricViews = new List<MetricView>();
            foreach (var defaultMetric in defaultMetrics)
            {
                //Default value to -1, so user is forced to update specific project in the future.
                metricViews.Add(new MetricView {IsDefault = defaultMetric.IsDefault, MetricId = defaultMetric.MetricId, Value = -1, Weight = defaultMetric.Weight });
            }
            return metricViews;
        }

        public async Task When(ProjectSuspended e)
        {
            await _writer.Update(e.Id, x => x.Status = ProjectStatus.Suspended);
        }

        public async Task When(CemSet e)
        {
            await _writer.Update(e.Id, x => x.CemId = e.StaffId);
        }

        public async Task When(PmSet e)
        {
            await _writer.Update(e.Id, x => x.PmId = e.StaffId);
        }

        public async Task When(TeamMembersAdded e)
        {
            await _writer.Update(e.Id, x => x.Team.AddRange(e.StaffIds));
        }

        public async Task When(TeamMembersRemoved e)
        {
            await _writer.Update(e.Id, x => x.Team.RemoveAll(id => e.StaffIds.Contains(id)));
        }

        public async Task When(MetricsAdded e)
        {
            await _writer.Update(e.Id, v => v.Metrics.AddRange(e.Metrics.ToMetricViews()));
        }

        public async Task When(MetricsRemoved e)
        {
            await _writer.Update(e.Id, v =>
            {
                foreach (var metric in e.Metrics)
                {
                    var m = v.Metrics.SingleOrDefault(x => x.MetricId == metric.MetricId);
                    if (m == null) continue;
                    v.Metrics.Remove(m);
                }
            });
        }
    }

    public static class ExtensMetricInfo
    {
        public static IEnumerable<MetricView> ToMetricViews(this IEnumerable<MetricInfo> list)
        {
            return list.Select(x => new MetricView { MetricId = x.MetricId, IsDefault = x.IsDefault });
        }
    }
}