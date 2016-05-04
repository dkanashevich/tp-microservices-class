using System;
using System.Collections.Generic;
using System.Linq;

namespace Metrics
{
	internal class CreateMetricSetupCommand
	{
		public string MetricId { get; set; }
		public string Configuration { get; set; }
	}

	internal class MetricSetup
	{
		public string MetricId { get; set; }
		public string Configuration { get; set; }
	}

	internal class MetricSetupCreatedEvent
	{
		public MetricSetup Setup { get; set; }
	}

	internal class MetricSetupViewDto
	{
		public string MetricId { get; set; }
		public string Configuration { get; set; }
		public IReadOnlyCollection<string> ListenedTypes { get; set; }
	}

	internal class MetricSetupView
	{
		private readonly List<MetricSetupViewDto> _setups = new List<MetricSetupViewDto>();

		public void Update(MetricSetupCreatedEvent ev)
		{
			_setups.Add(new MetricSetupViewDto
			{
				MetricId = ev.Setup.MetricId,
				Configuration = ev.Setup.Configuration,
				ListenedTypes = BuildListenedTypes(ev).ToList()
			});
		}

		public IEnumerable<MetricSetupViewDto> GetAllSetups(string triggerType) => _setups
			.Where(s => s.ListenedTypes.Contains(triggerType, StringComparer.OrdinalIgnoreCase));
		
		private IEnumerable<string> BuildListenedTypes(MetricSetupCreatedEvent ev)
		{
			yield return "UserStory";
		}
	}

	internal class MetricSetupAggregate
	{
		private readonly MetricSetupView _view;
		private readonly MetricScheduler _scheduler;
		private readonly MetricTargetBuilder _targetBuilder;

		private readonly List<MetricSetup> _setups = new List<MetricSetup>();

		public MetricSetupAggregate(MetricSetupView view, MetricScheduler scheduler, MetricTargetBuilder targetBuilder)
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