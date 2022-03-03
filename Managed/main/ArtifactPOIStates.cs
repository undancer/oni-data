using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ArtifactPOIStates")]
public class ArtifactPOIStates : GameStateMachine<ArtifactPOIStates, ArtifactPOIStates.Instance, IStateMachineTarget, ArtifactPOIStates.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance, IGameObjectEffectDescriptor
	{
		[Serialize]
		public ArtifactPOIConfigurator.ArtifactPOIInstanceConfiguration configuration;

		[Serialize]
		private float _poiCharge;

		[Serialize]
		public string artifactToHarvest;

		[Serialize]
		private int numHarvests;

		public float poiCharge
		{
			get
			{
				return _poiCharge;
			}
			set
			{
				_poiCharge = value;
				base.smi.sm.poiCharge.Set(value, base.smi);
			}
		}

		public Instance(IStateMachineTarget target, Def def)
			: base(target, def)
		{
		}

		public void PickNewArtifactToHarvest()
		{
			if (numHarvests <= 0 && !string.IsNullOrEmpty(configuration.GetArtifactID()))
			{
				artifactToHarvest = configuration.GetArtifactID();
				ArtifactSelector.Instance.ReserveArtifactID(artifactToHarvest);
			}
			else
			{
				artifactToHarvest = ArtifactSelector.Instance.GetUniqueArtifactID(ArtifactType.Space);
			}
		}

		public string GetArtifactToHarvest()
		{
			if (CanHarvestArtifact())
			{
				if (string.IsNullOrEmpty(artifactToHarvest))
				{
					PickNewArtifactToHarvest();
				}
				return artifactToHarvest;
			}
			return null;
		}

		public void HarvestArtifact()
		{
			if (CanHarvestArtifact())
			{
				numHarvests++;
				poiCharge = 0f;
				artifactToHarvest = null;
				PickNewArtifactToHarvest();
			}
		}

		public void RechargePOI(float dt)
		{
			float delta = dt / configuration.GetRechargeTime();
			DeltaPOICharge(delta);
		}

		public float RechargeTimeRemaining()
		{
			return (float)Mathf.CeilToInt((configuration.GetRechargeTime() - configuration.GetRechargeTime() * poiCharge) / 600f) * 600f;
		}

		public void DeltaPOICharge(float delta)
		{
			poiCharge += delta;
			poiCharge = Mathf.Min(1f, poiCharge);
		}

		public bool CanHarvestArtifact()
		{
			return poiCharge >= 1f;
		}

		public List<Descriptor> GetDescriptors(GameObject go)
		{
			return new List<Descriptor>();
		}
	}

	public State idle;

	public State recharging;

	public FloatParameter poiCharge = new FloatParameter(1f);

	public override void InitializeStates(out BaseState default_state)
	{
		base.serializable = SerializeType.ParamsOnly;
		default_state = idle;
		root.Enter(delegate(Instance smi)
		{
			if (smi.configuration == null || smi.configuration.typeId == HashedString.Invalid)
			{
				smi.configuration = smi.GetComponent<ArtifactPOIConfigurator>().MakeConfiguration();
				smi.PickNewArtifactToHarvest();
				smi.poiCharge = 1f;
			}
		});
		idle.ParamTransition(poiCharge, recharging, (Instance smi, float f) => f < 1f);
		recharging.ParamTransition(poiCharge, idle, (Instance smi, float f) => f >= 1f).EventHandler(GameHashes.NewDay, (Instance smi) => GameClock.Instance, delegate(Instance smi)
		{
			smi.RechargePOI(600f);
		});
	}
}
