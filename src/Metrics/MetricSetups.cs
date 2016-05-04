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

	public class MetricSetup
	{
		public string MetricId { get; set; }
		public string Configuration { get; set; }
	}

	public class MetricSetupCreatedEvent
	{
		public MetricSetup Setup { get; set; }
	}

	public class MetricSetupViewDto
	{
		public string MetricId { get; set; }
		public string Configuration { get; set; }
		public IReadOnlyCollection<string> ListenedTypes { get; set; }
	}

	public class MetricSetupView
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
}