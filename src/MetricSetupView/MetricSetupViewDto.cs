using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetricSetupView
{
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

	public class MetricSetupView
	{
		private readonly List<MetricSetupViewDto> _setups = new List<MetricSetupViewDto>
		{
			new MetricSetupViewDto {Configuration = "Test config", MetricId = "123", ListenedTypes = new[] {"UserStory"}}
		};

		public void Update(MetricSetupCreatedEvent ev)
		{
			_setups.Add(new MetricSetupViewDto
			{
				MetricId = ev.MetricId,
				Configuration = ev.Configuration,
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
