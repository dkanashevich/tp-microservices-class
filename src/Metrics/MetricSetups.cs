using System;
using System.Collections.Generic;
using System.Linq;

namespace Metrics
{
	public class MetricSetup
	{
		public string MetricId { get; set; }
		public string Configuration { get; set; }
	}

	public class MetricSetupCreatedEvent
	{
		public string MetricId { get; set; }
		public string Configuration { get; set; }
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
}