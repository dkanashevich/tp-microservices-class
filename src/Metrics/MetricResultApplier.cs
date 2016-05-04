namespace Metrics
{
	public class FieldSetter
	{
		public string FieldName { get; set; }
		public object FieldValue { get; set; }
	}

	public class UpdateFieldCommand
	{
		public int EntityId { get; set; }
		public string EntityType { get; set; }
		public FieldSetter Setter { get; set; }
	}

	public class MetricResultApplier
	{
		private readonly IWriteServiceGateway _service;

		public MetricResultApplier(IWriteServiceGateway service)
		{
			_service = service;
		}

		public void Apply(MetricExecutedEvent result)
		{
			_service.Write(new UpdateFieldCommand
			{
				EntityId = result.Target.EntityId,
				EntityType = result.Target.EntityType,
				Setter = result.Setter
			});
		}
	}
}