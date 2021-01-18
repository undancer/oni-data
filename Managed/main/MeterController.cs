using System;
using UnityEngine;

public class MeterController
{
	public GameObject gameObject;

	private KAnimLink link;

	public KBatchedAnimController meterController
	{
		get;
		private set;
	}

	public MeterController(KMonoBehaviour target, Meter.Offset front_back, Grid.SceneLayer user_specified_render_layer, params string[] symbols_to_hide)
	{
		string[] array = new string[symbols_to_hide.Length + 1];
		Array.Copy(symbols_to_hide, array, symbols_to_hide.Length);
		array[array.Length - 1] = "meter_target";
		KBatchedAnimController component = target.GetComponent<KBatchedAnimController>();
		Initialize(component, "meter_target", "meter", front_back, user_specified_render_layer, Vector3.zero, array);
	}

	public MeterController(KAnimControllerBase building_controller, string meter_target, string meter_animation, Meter.Offset front_back, Grid.SceneLayer user_specified_render_layer, params string[] symbols_to_hide)
	{
		Initialize(building_controller, meter_target, meter_animation, front_back, user_specified_render_layer, Vector3.zero, symbols_to_hide);
	}

	public MeterController(KAnimControllerBase building_controller, string meter_target, string meter_animation, Meter.Offset front_back, Grid.SceneLayer user_specified_render_layer, Vector3 tracker_offset, params string[] symbols_to_hide)
	{
		Initialize(building_controller, meter_target, meter_animation, front_back, user_specified_render_layer, tracker_offset, symbols_to_hide);
	}

	private void Initialize(KAnimControllerBase building_controller, string meter_target, string meter_animation, Meter.Offset front_back, Grid.SceneLayer user_specified_render_layer, Vector3 tracker_offset, params string[] symbols_to_hide)
	{
		string name = building_controller.name + "." + meter_animation;
		GameObject gameObject = UnityEngine.Object.Instantiate(Assets.GetPrefab(MeterConfig.ID));
		gameObject.name = name;
		gameObject.SetActive(value: false);
		gameObject.transform.parent = building_controller.transform;
		this.gameObject = gameObject;
		gameObject.GetComponent<KPrefabID>().PrefabTag = new Tag(name);
		Vector3 position = building_controller.transform.GetPosition();
		switch (front_back)
		{
		case Meter.Offset.Behind:
			position.z = building_controller.transform.GetPosition().z + 0.1f;
			break;
		case Meter.Offset.Infront:
			position.z = building_controller.transform.GetPosition().z - 0.1f;
			break;
		case Meter.Offset.UserSpecified:
			position.z = Grid.GetLayerZ(user_specified_render_layer);
			break;
		}
		gameObject.transform.SetPosition(position);
		KBatchedAnimController component = gameObject.GetComponent<KBatchedAnimController>();
		component.AnimFiles = new KAnimFile[1]
		{
			building_controller.AnimFiles[0]
		};
		component.initialAnim = meter_animation;
		component.fgLayer = Grid.SceneLayer.NoLayer;
		component.initialMode = KAnim.PlayMode.Paused;
		component.isMovable = true;
		component.FlipX = building_controller.FlipX;
		component.FlipY = building_controller.FlipY;
		if (Meter.Offset.UserSpecified == front_back)
		{
			component.sceneLayer = user_specified_render_layer;
		}
		meterController = component;
		KBatchedAnimTracker component2 = gameObject.GetComponent<KBatchedAnimTracker>();
		component2.offset = tracker_offset;
		component2.symbol = new HashedString(meter_target);
		gameObject.SetActive(value: true);
		building_controller.SetSymbolVisiblity(meter_target, is_visible: false);
		if (symbols_to_hide != null)
		{
			for (int i = 0; i < symbols_to_hide.Length; i++)
			{
				building_controller.SetSymbolVisiblity(symbols_to_hide[i], is_visible: false);
			}
		}
		link = new KAnimLink(building_controller, component);
	}

	public MeterController(KAnimControllerBase building_controller, KBatchedAnimController meter_controller, params string[] symbol_names)
	{
		if (!(meter_controller == null))
		{
			meterController = meter_controller;
			link = new KAnimLink(building_controller, meter_controller);
			for (int i = 0; i < symbol_names.Length; i++)
			{
				building_controller.SetSymbolVisiblity(symbol_names[i], is_visible: false);
			}
			meterController.GetComponent<KBatchedAnimTracker>().symbol = new HashedString(symbol_names[0]);
		}
	}

	public void SetPositionPercent(float percent_full)
	{
		if (!(meterController == null))
		{
			meterController.SetPositionPercent(percent_full);
		}
	}

	public void SetSymbolTint(KAnimHashedString symbol, Color32 colour)
	{
		if (meterController != null)
		{
			meterController.SetSymbolTint(symbol, colour);
		}
	}

	public void SetRotation(float rot)
	{
		if (!(meterController == null))
		{
			meterController.Rotation = rot;
		}
	}
}
