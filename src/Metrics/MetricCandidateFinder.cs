using System.Collections.Generic;
using System.Linq;

namespace Metrics
{
	public class MetricCandidate
	{
		public string MetricId { get; set; }
		public string Configuration { get; set; }
	}

	public class MetricCandidateFinder
	{
		private readonly MetricSetupView _setupView;

		public MetricCandidateFinder(MetricSetupView setupView)
		{
			_setupView = setupView;
		}

		public IReadOnlyCollection<MetricCandidate> FindMetricCandidates(MetricTriggeredEvent ev)
		{
			var setups = _setupView.GetAllSetups(ev.EntityType).ToList();
			return setups
				.Select(s => new MetricCandidate {MetricId = s.MetricId, Configuration = s.Configuration})
				.ToList();
		}
	}
}