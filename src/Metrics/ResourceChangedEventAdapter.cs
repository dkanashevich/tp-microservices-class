using System.Collections.Generic;

namespace Metrics
{
	public class ResourceChangedEvent
	{
		public int EntityId { get; set; }
		public string EntityType { get; set; }
		public IReadOnlyCollection<string> ChangedFields { get; set; }
	}

	public class ResourceChangedEventAdapter
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
}