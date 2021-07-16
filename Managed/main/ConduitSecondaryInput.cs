using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ConduitSecondaryInput")]
public class ConduitSecondaryInput : KMonoBehaviour, ISecondaryInput
{
	[SerializeField]
	public ConduitPortInfo portInfo;

	public bool HasSecondaryConduitType(ConduitType type)
	{
		return portInfo.conduitType == type;
	}

	public CellOffset GetSecondaryConduitOffset(ConduitType type)
	{
		if (portInfo.conduitType == type)
		{
			return portInfo.offset;
		}
		return CellOffset.none;
	}
}
