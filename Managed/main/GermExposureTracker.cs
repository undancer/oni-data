using System.Collections.Generic;
using KSerialization;
using ProcGen;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/GermExposureTracker")]
public class GermExposureTracker : KMonoBehaviour
{
	private class WeightedExposure : IWeighted
	{
		public GermExposureMonitor.Instance monitor;

		public float weight
		{
			get;
			set;
		}
	}

	public static GermExposureTracker Instance;

	[Serialize]
	private Dictionary<HashedString, float> accumulation = new Dictionary<HashedString, float>();

	private SeededRandom rng;

	private List<WeightedExposure> exposure_candidates = new List<WeightedExposure>();

	protected override void OnPrefabInit()
	{
		Debug.Assert(Instance == null);
		Instance = this;
	}

	protected override void OnSpawn()
	{
		rng = new SeededRandom(GameClock.Instance.GetCycle());
	}

	protected override void OnCleanUp()
	{
		Instance = null;
	}

	public void AddExposure(ExposureType exposure_type, float amount)
	{
		accumulation.TryGetValue(exposure_type.germ_id, out var value);
		float num = value + amount;
		if (num > 1f)
		{
			foreach (MinionIdentity item in Components.LiveMinionIdentities.Items)
			{
				GermExposureMonitor.Instance sMI = item.GetSMI<GermExposureMonitor.Instance>();
				if (sMI.GetExposureState(exposure_type.germ_id) == GermExposureMonitor.ExposureState.Exposed)
				{
					float exposureWeight = item.GetSMI<GermExposureMonitor.Instance>().GetExposureWeight(exposure_type.germ_id);
					if (exposureWeight > 0f)
					{
						exposure_candidates.Add(new WeightedExposure
						{
							weight = exposureWeight,
							monitor = sMI
						});
					}
				}
			}
			while (num > 1f)
			{
				num -= 1f;
				if (exposure_candidates.Count > 0)
				{
					WeightedExposure weightedExposure = WeightedRandom.Choose(exposure_candidates, rng);
					exposure_candidates.Remove(weightedExposure);
					weightedExposure.monitor.ContractGerms(exposure_type.germ_id);
				}
			}
		}
		accumulation[exposure_type.germ_id] = num;
		exposure_candidates.Clear();
	}
}
