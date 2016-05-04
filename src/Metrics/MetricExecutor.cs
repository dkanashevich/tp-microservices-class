namespace Metrics
{
	public class ExecuteMetricCommand
	{
		public MetricTarget Target { get; set; }
		public string Configuration { get; set; }
	}

	public class MetricExecutedEvent
	{
		public MetricTarget Target { get; set; }
		public FieldSetter Setter { get; set; }
	}

	public class MetricExecutor
	{
		private readonly MetricResultApplier _resultApplier;

		public MetricExecutor(MetricResultApplier resultApplier)
		{
			_resultApplier = resultApplier;
		}

		public void Execute(ExecuteMetricCommand cmd)
		{
			_resultApplier.Apply(new MetricExecutedEvent
			{
				Target = cmd.Target,
				Setter = new FieldSetter
				{
					FieldName = "Effort",
					FieldValue = 42
				}
			});
		}
	}
}