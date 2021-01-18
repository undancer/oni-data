using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ConduitSecondaryOutput")]
public class ConduitSecondaryOutput : KMonoBehaviour, ISecondaryOutput
{
	[SerializeField]
	public ConduitPortInfo portInfo;

	public ConduitType GetSecondaryConduitType()
	{
		return portInfo.conduitType;
	}

	public CellOffset GetSecondaryConduitOffset()
	{
		return portInfo.offset;
	}
}
