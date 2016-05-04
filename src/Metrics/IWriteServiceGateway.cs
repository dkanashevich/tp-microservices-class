namespace Metrics
{
	public interface IWriteServiceGateway
	{
		void Write(UpdateFieldCommand cmd);
	}
}
