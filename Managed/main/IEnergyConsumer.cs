public interface IEnergyConsumer : ICircuitConnected
{
	float WattsUsed
	{
		get;
	}

	float WattsNeededWhenActive
	{
		get;
	}

	int PowerSortOrder
	{
		get;
	}

	string Name
	{
		get;
	}

	bool IsConnected
	{
		get;
	}

	bool IsPowered
	{
		get;
	}

	void SetConnectionStatus(CircuitManager.ConnectionStatus status);
}
