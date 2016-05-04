using System.Collections.Generic;

namespace Metrics
{
	public class ScheduleMetricCommand
	{
		public IReadOnlyCollection<MetricTargetWithSetup> Targets { get; set; }
	}

	public class MetricScheduler
	{
		private readonly MetricExecutor _executor;

		public MetricScheduler(MetricExecutor executor)
		{
			_executor = executor;
		}

		public void Schedule(ScheduleMetricCommand ev)
		{
			foreach (var target in ev.Targets)
			{
				_executor.Execute(new ExecuteMetricCommand
				{
					Target = target.Target,
					Configuration = target.Configuration
				});
			}
		}
	}
}