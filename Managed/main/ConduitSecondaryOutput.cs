using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ConduitSecondaryOutput")]
public class ConduitSecondaryOutput : KMonoBehaviour, ISecondaryOutput
{
	[SerializeField]
	public ConduitPortInfo portInfo;

	public bool HasSecondaryConduitType(ConduitType type)
	{
		return portInfo.conduitType == type;
	}

	public CellOffset GetSecondaryConduitOffset(ConduitType type)
	{
		if (type == portInfo.conduitType)
		{
			return portInfo.offset;
		}
		return CellOffset.none;
	}
}
