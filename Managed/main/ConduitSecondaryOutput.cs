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
		return portInfo.offset;
	}
}
