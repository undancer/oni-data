public interface ISecondaryInput
{
	bool HasSecondaryConduitType(ConduitType type);

	CellOffset GetSecondaryConduitOffset(ConduitType type);
}
