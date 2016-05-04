using System;
using System.Collections.Generic;
using System.Linq;
using RestSharp;

namespace Metrics
{
	public class MetricSetup
	{
		public string MetricId { get; set; }
		public string Configuration { get; set; }
	}

	public class MetricSetupViewDto
	{
		public string MetricId { get; set; }
		public string Configuration { get; set; }
		public List<string> ListenedTypes { get; set; }
	}

	public interface IMetricSetupView
	{
		IEnumerable<MetricSetupViewDto> GetAllSetups(string triggerType);
	}

	internal class MetricSetupViewDtoCollection
	{
		public List<MetricSetupViewDto> Items { get; set; }
	}

	public class MetricSetupView : IMetricSetupView
	{
		public IEnumerable<MetricSetupViewDto> GetAllSetups(string triggerType)
		{
			var client = new RestClient("http://localhost:8888/metricSetupView");
			var request = new RestRequest($"setups/{triggerType}");
			var response = client.Execute<MetricSetupViewDtoCollection>(request);
			return response.Data.Items;
		}
	}
}