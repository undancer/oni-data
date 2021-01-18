using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/TileTemperature")]
public class TileTemperature : KMonoBehaviour
{
	[MyCmpReq]
	private PrimaryElement primaryElement;

	[MyCmpReq]
	private KSelectable selectable;

	protected override void OnPrefabInit()
	{
		primaryElement.getTemperatureCallback = OnGetTemperature;
		primaryElement.setTemperatureCallback = OnSetTemperature;
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	private static float OnGetTemperature(PrimaryElement primary_element)
	{
		SimCellOccupier component = primary_element.GetComponent<SimCellOccupier>();
		if (component != null && component.IsReady())
		{
			int i = Grid.PosToCell(primary_element.transform.GetPosition());
			return Grid.Temperature[i];
		}
		return primary_element.InternalTemperature;
	}

	private static void OnSetTemperature(PrimaryElement primary_element, float temperature)
	{
		SimCellOccupier component = primary_element.GetComponent<SimCellOccupier>();
		if (component != null && component.IsReady())
		{
			Debug.LogWarning("Only set a tile's temperature during initialization. Otherwise you should be modifying the cell via the sim!");
		}
		else
		{
			primary_element.InternalTemperature = temperature;
		}
	}
}
