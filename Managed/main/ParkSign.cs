using Klei.AI;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ParkSign")]
public class ParkSign : KMonoBehaviour
{
	private static readonly EventSystem.IntraObjectHandler<ParkSign> TriggerRoomEffectsDelegate = new EventSystem.IntraObjectHandler<ParkSign>(delegate(ParkSign component, object data)
	{
		component.TriggerRoomEffects(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-832141045, TriggerRoomEffectsDelegate);
	}

	private void TriggerRoomEffects(object data)
	{
		GameObject gameObject = (GameObject)data;
		Game.Instance.roomProber.GetRoomOfGameObject(base.gameObject)?.roomType.TriggerRoomEffects(base.gameObject.GetComponent<KPrefabID>(), gameObject.GetComponent<Effects>());
	}
}
