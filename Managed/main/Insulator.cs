using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/Insulator")]
public class Insulator : KMonoBehaviour
{
	[MyCmpReq]
	private Building building;

	[SerializeField]
	public CellOffset offset = CellOffset.none;

	protected override void OnSpawn()
	{
		SimMessages.SetInsulation(Grid.OffsetCell(Grid.PosToCell(base.transform.GetPosition()), offset), building.Def.ThermalConductivity);
	}

	protected override void OnCleanUp()
	{
		SimMessages.SetInsulation(Grid.OffsetCell(Grid.PosToCell(base.transform.GetPosition()), offset), 1f);
	}
}
