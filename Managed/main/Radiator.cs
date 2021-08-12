using System.Collections.Generic;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Radiator")]
public class Radiator : KMonoBehaviour, IGameObjectEffectDescriptor
{
	public RadiationGridEmitter emitter;

	public int intensity;

	public int projectionCount;

	public int direction;

	public int angle = 360;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		emitter = new RadiationGridEmitter(Grid.PosToCell(base.gameObject), intensity);
		emitter.projectionCount = projectionCount;
		emitter.direction = direction;
		emitter.angle = angle;
		if (GetComponent<Operational>() == null)
		{
			emitter.enabled = true;
		}
		else
		{
			Subscribe(824508782, OnOperationalChanged);
		}
		RadiationGridManager.emitters.Add(emitter);
	}

	protected override void OnCleanUp()
	{
		RadiationGridManager.emitters.Remove(emitter);
		base.OnCleanUp();
	}

	private void OnOperationalChanged(object data)
	{
		bool isActive = GetComponent<Operational>().IsActive;
		emitter.enabled = isActive;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		return new List<Descriptor>
		{
			new Descriptor(string.Format(UI.GAMEOBJECTEFFECTS.EMITS_LIGHT, intensity), UI.GAMEOBJECTEFFECTS.TOOLTIPS.EMITS_LIGHT)
		};
	}

	private void Update()
	{
		emitter.originCell = Grid.PosToCell(base.gameObject);
	}
}
