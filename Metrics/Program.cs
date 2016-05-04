using System.Collections.Generic;
using System.Linq;

namespace Metrics
{
	internal class MetricTriggeredEvent
	{
		public string EntityType { get; set; }
		public int EntityId { get; set; }
		public IReadOnlyCollection<string> ChangedFieldNames { get; set; }
	}

	internal class MetricTriggerHandler
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

	internal class MetricCandidateFinder
	{
		private readonly MetricSetupView _setupView;

		public MetricCandidateFinder(MetricSetupView setupView)
		{
			_setupView = setupView;
		}

		public IReadOnlyCollection<MetricCandidate> FindMetricCandidates(MetricTriggeredEvent ev)
		{
			var setups = _setupView.GetAllSetups(ev.EntityType).ToList();
			return setups
				.Select(s => new MetricCandidate {MetricId = s.MetricId, Configuration = s.Configuration})
				.ToList();
		}
	}

	internal class MetricCandidate
	{
		public string MetricId { get; set; }
		public string Configuration { get; set; }
	}

	internal class MetricTarget
	{
		//public string MetricId { get; set; }
		public int EntityId { get; set; }
		public string EntityType { get; set; }
	}

	internal class MetricTargetWithSetup
	{
		public string MetricId { get; set; }
		public MetricTarget Target { get; set; }
		public string Configuration { get; set; }
	}

	internal class ScheduleMetricCommand
	{
		public IReadOnlyCollection<MetricTargetWithSetup> Targets { get; set; }
	}

	internal class MetricTargetBuilder
	{
		public IReadOnlyCollection<MetricTargetWithSetup> BuildTargets(IReadOnlyCollection<MetricCandidate> candidates)
		{
			return candidates
				.Select((c, i) => new MetricTargetWithSetup
				{
					Target = new MetricTarget { EntityId = 40 + i, EntityType = "UserStory"},
					Configuration = c.Configuration,
					MetricId = c.MetricId
				})
				.ToList();
		}

		public IReadOnlyCollection<MetricTargetWithSetup> BuildTargets(MetricSetup setup)
		{
			return new[]
			{
				new MetricTargetWithSetup
				{
					Target = new MetricTarget {EntityId = 42, EntityType = "UserStory"},
					Configuration = setup.Configuration,
					MetricId = setup.MetricId
				}
			};
		}
	}

	internal class MetricScheduler
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

	internal class ExecuteMetricCommand
	{
		public MetricTarget Target { get; set; }
		public string Configuration { get; set; }
	}

	internal class MetricExecutor
	{
		private readonly MetricResultApplier _resultApplier;

		public MetricExecutor(MetricResultApplier resultApplier)
		{
			_resultApplier = resultApplier;
		}

		public void Execute(ExecuteMetricCommand cmd)
		{
			_resultApplier.Apply(new MetricExecutedEvent
			{
				Target = cmd.Target,
				Setter = new FieldSetter
				{
					FieldName = "Effort",
					FieldValue = 42
				}
			});
		}
	}

	internal class MetricExecutedEvent
	{
		public MetricTarget Target { get; set; }
		public FieldSetter Setter { get; set; }
	}

	internal class FieldSetter
	{
		public string FieldName { get; set; }
		public object FieldValue { get; set; }
	}

	internal class MetricResultApplier
	{
		private readonly IWriteServiceGateway _service;

		public MetricResultApplier(IWriteServiceGateway service)
		{
			_service = service;
		}

		public void Apply(MetricExecutedEvent result)
		{
			_service.Write(new UpdateFieldCommand
			{
				EntityId = result.Target.EntityId,
				EntityType = result.Target.EntityType,
				Setter = result.Setter
			});
		}
	}

	internal interface IWriteServiceGateway
	{
		void Write(UpdateFieldCommand cmd);
	}

	internal class UpdateFieldCommand
	{
		public int EntityId { get; set; }
		public string EntityType { get; set; }
		public FieldSetter Setter { get; set; }
	}

	internal class ResourceChangedEvent
	{
		public int EntityId { get; set; }
		public string EntityType { get; set; }
		public IReadOnlyCollection<string> ChangedFields { get; set; }
	}

	internal class ResourceChangedEventAdapter
	{
		private readonly MetricTriggerHandler _triggerHandler;

		public ResourceChangedEventAdapter(MetricCandidateFinder candidateFinder, MetricTriggerHandler triggerHandler)
		{
			_triggerHandler = triggerHandler;
		}

		public void ProcessEvent(ResourceChangedEvent ev)
		{
			_triggerHandler.Handle(new MetricTriggeredEvent
			{
				EntityId = ev.EntityId,
				EntityType = ev.EntityType,
				ChangedFieldNames = ev.ChangedFields
			});
		}
	}

	class Program
	{
		static void Main(string[] args)
		{
			MetricSetupAggregate setupAggregate = null;
			setupAggregate.CreateMetricSetup(new CreateMetricSetupCommand
			{
				MetricId = "CustomMetric",
				Configuration = ""
			});

			ResourceChangedEventAdapter rceAdapter = null;
			rceAdapter.ProcessEvent(new ResourceChangedEvent
			{
				EntityType = "UserStory",
				EntityId = 41,
				ChangedFields = new[] {"Name", "Effort"}
			});
		}
	}
}
