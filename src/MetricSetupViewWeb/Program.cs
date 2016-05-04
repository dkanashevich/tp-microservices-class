using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetricSetupView;
using Nancy;
using Nancy.Extensions;
using Nancy.Hosting.Self;
using Nancy.ModelBinding;
using Nancy.Responses;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MetricSetupViewWeb
{
	public class Program
	{
		static void Main()
		{
			SetupQueue();
			Console.WriteLine("Stopped. Good bye!");
		}

		private static Type GetType(string eventType)
		{
			switch (eventType)
			{
				case nameof(MetricSetupCreatedEvent):
					return typeof (MetricSetupCreatedEvent);
				default:
					throw new ArgumentException($"Unknown eventType '{eventType}'");
			}
		}

		private static void SetupQueue()
		{
			var factory = new ConnectionFactory();
			factory.Uri = "amqp://guest:guest@itsu.me";
			using (var connection = factory.CreateConnection())
			{
				using (var channel = connection.CreateModel())
				{
					var consumer = new EventingBasicConsumer(channel);
					consumer.Received += (model, ea) =>
					{
						var body = ea.Body;
						var message = Encoding.UTF8.GetString(body);
						Console.WriteLine(" [x] Received {0}", message);

						var targetType = GetType(ea.BasicProperties.Type);
						var payload = JsonConvert.DeserializeObject(message, targetType);
						var createdEvent = payload as MetricSetupCreatedEvent;
						if (createdEvent != null)
						{
							Console.WriteLine("Start handling Create event");
							Endpoint.Handle(createdEvent);
							Console.WriteLine("Finished handling Create event");
						}
					};

					channel.BasicConsume(queue: "lambda01-metrics",
						noAck: true,
						consumer: consumer);

					Console.WriteLine("Started listener");
					Console.ReadLine();
				}
			}
		}

		private static void SetupNancy()
		{
			var hostUrl = "http://localhost:8888/metricSetupView/";

			using (var nancyHost = new NancyHost(new Uri(hostUrl)))
			{
				nancyHost.Start();

				Console.WriteLine("Nancy now listening - navigating to http://localhost:8888/metricSetupView/. Press enter to stop");
				try
				{
					Process.Start(hostUrl + "/setups/userstory");
				}
				catch (Exception)
				{
				}

				Console.ReadKey();
			}
		}
	}

	internal class Endpoint
	{
		public static readonly MetricSetupView.MetricSetupView View = new MetricSetupView.MetricSetupView();

		public static void Handle(MetricSetupCreatedEvent ev)
		{
			View.Update(ev);
		}
	}

	public class MetricSetupViewModule : Nancy.NancyModule
	{
		public MetricSetupViewModule()
		{
			Get["/"] = _ => "Setup view OK";
			Get["/setups/{entityType}"] = parameters =>
			{
				string entityType = parameters.entityType;
				return Response.AsJson(new
				{
					Items = Endpoint.View.GetAllSetups(entityType).ToList()
				});
			};
			Post["/setups"] = parameters =>
			{
				MetricSetupCreatedEvent cmd = this.Bind();
				Endpoint.Handle(cmd);
				return Response.AsJson(new {});
			};
		}
	}
}
