using Metrics;
using MetricSetupAggregate;

namespace TestApp
{
	class Program
	{
		static void Main(string[] args)
		{
			MetricSetupAggregate.MetricSetupAggregate setupAggregate = null;
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
