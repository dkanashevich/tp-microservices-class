using System.Collections.Generic;
using System.Linq;

namespace Metrics
{
	public class MetricTarget
	{
		public int EntityId { get; set; }
		public string EntityType { get; set; }
	}

	public class MetricTargetWithSetup
	{
		public string MetricId { get; set; }
		public MetricTarget Target { get; set; }
		public string Configuration { get; set; }
	}

	public class MetricTargetBuilder
	{
		public IReadOnlyCollection<MetricTargetWithSetup> BuildTargets(IReadOnlyCollection<MetricCandidate> candidates)
		{
			return candidates
				.Select((c, i) => new MetricTargetWithSetup
				{
					Target = new MetricTarget { EntityId = 40 + i, EntityType = "UserStory"},
					Configuration = c.Configuration,
					MetricId = c.MetricId
				})
				.ToList();
		}

		public IReadOnlyCollection<MetricTargetWithSetup> BuildTargets(MetricSetup setup)
		{
			return new[]
			{
				new MetricTargetWithSetup
				{
					Target = new MetricTarget {EntityId = 42, EntityType = "UserStory"},
					Configuration = setup.Configuration,
					MetricId = setup.MetricId
				}
			};
		}
	}
}