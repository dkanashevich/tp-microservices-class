using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Metrics;
using MetricSetupAggregate;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace TestApp
{
	class Program
	{
		static void Main(string[] args)
		{
			var factory = new ConnectionFactory();
			factory.Uri = "amqp://guest:guest@itsu.me";
			using (var connection = factory.CreateConnection())
			{
				using (var channel = connection.CreateModel())
				{
					channel.QueueDeclare(
						queue: "lambda01-metrics",
						durable: false,
						exclusive: false,
						autoDelete: false,
						arguments: null);

					//Subscribe(channel);

					var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new MetricSetupCreatedEvent
					{
						Configuration = "Test Rabbit config",
						MetricId = "TestRabbitMetric"
					}));

					var basicProperties = channel.CreateBasicProperties();
					basicProperties.Type = "MetricSetupCreatedEvent";

					channel.BasicPublish(exchange: "",
						routingKey: "lambda01-metrics",
						basicProperties: basicProperties,
						body: body);
					Console.WriteLine("Published");

					Console.ReadLine();
				}
			}

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

			//var viewGateway = new MetricSetupView();
			//foreach (var setup in viewGateway.GetAllSetups("userstory"))
			//{
			//	Console.WriteLine($"MetricId: {setup.MetricId}; Configuration: {setup.Configuration}");
			//}

			Console.ReadLine();
		}
	}
}
