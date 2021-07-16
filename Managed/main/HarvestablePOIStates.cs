using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/HarvestablePOIStates")]
public class HarvestablePOIStates : GameStateMachine<HarvestablePOIStates, HarvestablePOIStates.Instance, IStateMachineTarget, HarvestablePOIStates.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance, IGameObjectEffectDescriptor
	{
		[Serialize]
		public HarvestablePOIConfigurator.HarvestablePOIInstanceConfiguration configuration;

		[Serialize]
		private float _poiCapacity;

		public float poiCapacity
		{
			get
			{
				return _poiCapacity;
			}
			set
			{
				_poiCapacity = value;
				base.smi.sm.poiCapacity.Set(value, base.smi);
			}
		}

		public Instance(IStateMachineTarget target, Def def)
			: base(target, def)
		{
		}

		public void RechargePOI(float dt)
		{
			float num = dt / configuration.GetRechargeTime();
			float delta = configuration.GetMaxCapacity() * num;
			DeltaPOICapacity(delta);
		}

		public void DeltaPOICapacity(float delta)
		{
			poiCapacity += delta;
			poiCapacity = Mathf.Min(configuration.GetMaxCapacity(), poiCapacity);
		}

		public bool POICanBeHarvested()
		{
			return poiCapacity > 0f;
		}

		public List<Descriptor> GetDescriptors(GameObject go)
		{
			List<Descriptor> list = new List<Descriptor>();
			foreach (KeyValuePair<SimHashes, float> elementsWithWeight in configuration.GetElementsWithWeights())
			{
				SimHashes key = elementsWithWeight.Key;
				string arg = ElementLoader.FindElementByHash(key).tag.ProperName();
				list.Add(new Descriptor(string.Format(UI.SPACEDESTINATIONS.HARVESTABLE_POI.POI_PRODUCTION, arg), string.Format(UI.SPACEDESTINATIONS.HARVESTABLE_POI.POI_PRODUCTION_TOOLTIP, key.ToString())));
			}
			list.Add(new Descriptor($"{GameUtil.GetFormattedMass(poiCapacity)}/{GameUtil.GetFormattedMass(configuration.GetMaxCapacity())}", "Capacity"));
			return list;
		}
	}

	public State idle;

	public State recharging;

	public FloatParameter poiCapacity = new FloatParameter(1f);

	public override void InitializeStates(out BaseState default_state)
	{
		base.serializable = SerializeType.ParamsOnly;
		default_state = idle;
		root.Enter(delegate(Instance smi)
		{
			if (smi.configuration == null || smi.configuration.typeId == HashedString.Invalid)
			{
				smi.configuration = smi.GetComponent<HarvestablePOIConfigurator>().MakeConfiguration();
				smi.poiCapacity = Random.Range(0f, smi.configuration.GetMaxCapacity());
			}
		});
		idle.ParamTransition(poiCapacity, recharging, (Instance smi, float f) => f < smi.configuration.GetMaxCapacity());
		recharging.EventHandler(GameHashes.NewDay, (Instance smi) => GameClock.Instance, delegate(Instance smi)
		{
			smi.RechargePOI(600f);
		}).ParamTransition(poiCapacity, idle, (Instance smi, float f) => f >= smi.configuration.GetMaxCapacity());
	}
}
