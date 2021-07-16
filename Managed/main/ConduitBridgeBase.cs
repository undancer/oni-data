public class ConduitBridgeBase : KMonoBehaviour
{
	public delegate float DesiredMassTransfer(float dt, SimHashes element, float mass, float temperature, byte disease_idx, int disease_count, Pickupable pickupable);

	public delegate void ConduitBridgeEvent(SimHashes element, float mass, float temperature, byte disease_idx, int disease_count, Pickupable pickupable);

	public DesiredMassTransfer desiredMassTransfer;

	public ConduitBridgeEvent OnMassTransfer;

	protected void SendEmptyOnMassTransfer()
	{
		if (OnMassTransfer != null)
		{
			OnMassTransfer(SimHashes.Void, 0f, 0f, 0, 0, null);
		}
	}
}
