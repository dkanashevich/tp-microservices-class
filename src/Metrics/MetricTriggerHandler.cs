using System.Collections.Generic;

namespace Metrics
{
	public class MetricTriggeredEvent
	{
		public string EntityType { get; set; }
		public int EntityId { get; set; }
		public IReadOnlyCollection<string> ChangedFieldNames { get; set; }
	}

	public class MetricTriggerHandler
	{
		private readonly MetricCandidateFinder _candidateFinder;
		private readonly MetricTargetBuilder _targetBuilder;
		private readonly MetricScheduler _scheduler;

		public MetricTriggerHandler(MetricCandidateFinder candidateFinder, MetricTargetBuilder targetBuilder, MetricScheduler scheduler)
		{
			_candidateFinder = candidateFinder;
			_targetBuilder = targetBuilder;
			_scheduler = scheduler;
		}

		public void Handle(MetricTriggeredEvent ev)
		{
			var candidates = _candidateFinder.FindMetricCandidates(ev);
			var targets = _targetBuilder.BuildTargets(candidates);

			_scheduler.Schedule(new ScheduleMetricCommand
			{
				Targets = targets
			});
		}
	}
}