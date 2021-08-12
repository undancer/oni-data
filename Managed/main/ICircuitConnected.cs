public interface ICircuitConnected
{
	bool IsVirtual { get; }

	int PowerCell { get; }

	object VirtualCircuitKey { get; }
}
