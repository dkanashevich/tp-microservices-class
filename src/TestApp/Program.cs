using System;
using System.Linq;
using Metrics;
using MetricSetupAggregate;

namespace TestApp
{
	class Program
	{
		static void Main(string[] args)
		{
			//MetricSetupAggregate.MetricSetupAggregate setupAggregate = null;
			//setupAggregate.CreateMetricSetup(new CreateMetricSetupCommand
			//{
			//	MetricId = "CustomMetric",
			//	Configuration = ""
			//});

			//ResourceChangedEventAdapter rceAdapter = null;
			//rceAdapter.ProcessEvent(new ResourceChangedEvent
			//{
			//	EntityType = "UserStory",
			//	EntityId = 41,
			//	ChangedFields = new[] {"Name", "Effort"}
			//});

			var viewGateway = new MetricSetupView();
			foreach (var setup in viewGateway.GetAllSetups("userstory"))
			{
				Console.WriteLine($"MetricId: {setup.MetricId}; Configuration: {setup.Configuration}");
			}

			Console.ReadLine();
		}
	}
}
