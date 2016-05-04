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

namespace MetricSetupViewWeb
{
	public class Program
	{
		static void Main()
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

			Console.WriteLine("Stopped. Good bye!");
		}
	}

	public class MetricSetupViewModule : Nancy.NancyModule
	{
		private static readonly MetricSetupView.MetricSetupView _view = new MetricSetupView.MetricSetupView();

		public MetricSetupViewModule()
		{
			Get["/"] = _ => "Setup view OK";
			Get["/setups/{entityType}"] = parameters =>
			{
				string entityType = parameters.entityType;
				return Response.AsJson(new
				{
					Items = _view.GetAllSetups(entityType).ToList()
				});
			};
			Post["/setups"] = parameters =>
			{
				MetricSetupCreatedEvent cmd = this.Bind();
				_view.Update(cmd);
				return Response.AsJson(new {});
			};
		}
	}
}
