using System.Collections.Generic;

namespace MetricSetupAggregate
{
	public class CreateMetricSetupCommand
	{
		public string MetricId { get; set; }
		public string Configuration { get; set; }
	}

	public class MetricSetup
	{
		public string MetricId { get; set; }
		public string Configuration { get; set; }
	}
	
	public class MetricSetupCreatedEvent
	{
		public MetricSetup Setup { get; set; }
	}
	
	public class MetricSetupViewDto
	{
		public string MetricId { get; set; }
		public string Configuration { get; set; }
		public IReadOnlyCollection<string> ListenedTypes { get; set; }
	}

	public interface IMetricSetupView
	{
		void Update(MetricSetupCreatedEvent ev);
		IEnumerable<MetricSetupViewDto> GetAllSetups(string triggerType);
	}

	public interface IMetricScheduler
	{
		void Schedule(ScheduleMetricCommand ev);
	}
	
	public interface IMetricTargetBuilder
	{
		IReadOnlyCollection<MetricTargetWithSetup> BuildTargets(MetricSetup setup);
	}

	public class ScheduleMetricCommand
	{
		public IReadOnlyCollection<MetricTargetWithSetup> Targets { get; set; }
	}
	
	public class MetricTargetWithSetup
	{
		public string MetricId { get; set; }
		public MetricTarget Target { get; set; }
		public string Configuration { get; set; }
	}
	
	public class MetricTarget
	{
		public int EntityId { get; set; }
		public string EntityType { get; set; }
	}

	public class MetricSetupAggregate
	{
		private readonly IMetricSetupView _view;
		private readonly IMetricScheduler _scheduler;
		private readonly IMetricTargetBuilder _targetBuilder;

		private readonly List<MetricSetup> _setups = new List<MetricSetup>();

		public MetricSetupAggregate(IMetricSetupView view, IMetricScheduler scheduler, IMetricTargetBuilder targetBuilder)
		{
			_view = view;
			_scheduler = scheduler;
			_targetBuilder = targetBuilder;
		}

		public void CreateMetricSetup(CreateMetricSetupCommand cmd)
		{
			var setup = new MetricSetup
			{
				MetricId = cmd.MetricId,
				Configuration = cmd.Configuration
			};

			_setups.Add(setup);

			_view.Update(new MetricSetupCreatedEvent
			{
				Setup = setup
			});

			var targets = _targetBuilder.BuildTargets(setup);
			_scheduler.Schedule(new ScheduleMetricCommand {Targets = targets});
		}
	}
}
