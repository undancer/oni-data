using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class PlantSubSpeciesCatalog : KMonoBehaviour
{
	[Serializable]
	public struct SubSpeciesInfo : IEquatable<SubSpeciesInfo>
	{
		public Tag speciesID;

		public Tag ID;

		public List<string> mutationIDs;

		private const string ORIGINAL_SUFFIX = "_Original";

		public bool IsValid => ID.IsValid;

		public bool IsOriginal => mutationIDs == null || mutationIDs.Count == 0;

		public SubSpeciesInfo(Tag speciesID, List<string> mutationIDs)
		{
			this.speciesID = speciesID;
			this.mutationIDs = ((mutationIDs != null) ? new List<string>(mutationIDs) : new List<string>());
			ID = SubSpeciesIDFromMutations(speciesID, mutationIDs);
		}

		public static Tag SubSpeciesIDFromMutations(Tag speciesID, List<string> mutationIDs)
		{
			if (mutationIDs == null || mutationIDs.Count == 0)
			{
				return string.Concat(speciesID, "_Original");
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(speciesID);
			foreach (string mutationID in mutationIDs)
			{
				stringBuilder.Append("_");
				stringBuilder.Append(mutationID);
			}
			return stringBuilder.ToString().ToTag();
		}

		public string GetNameWithMutations(string properName, bool identified, bool cleanOriginal)
		{
			if (mutationIDs == null || mutationIDs.Count == 0)
			{
				if (cleanOriginal)
				{
					return properName;
				}
				return CREATURES.PLANT_MUTATIONS.PLANT_NAME_FMT.Replace("{PlantName}", properName).Replace("{MutationList}", CREATURES.PLANT_MUTATIONS.NONE.NAME);
			}
			if (!identified)
			{
				return CREATURES.PLANT_MUTATIONS.PLANT_NAME_FMT.Replace("{PlantName}", properName).Replace("{MutationList}", CREATURES.PLANT_MUTATIONS.UNIDENTIFIED);
			}
			return CREATURES.PLANT_MUTATIONS.PLANT_NAME_FMT.Replace("{PlantName}", properName).Replace("{MutationList}", string.Join(", ", Db.Get().PlantMutations.GetNamesForMutations(mutationIDs)));
		}

		public static bool operator ==(SubSpeciesInfo obj1, SubSpeciesInfo obj2)
		{
			return obj1.Equals(obj2);
		}

		public static bool operator !=(SubSpeciesInfo obj1, SubSpeciesInfo obj2)
		{
			return !(obj1 == obj2);
		}

		public override bool Equals(object other)
		{
			if (!(other is SubSpeciesInfo))
			{
				return false;
			}
			return this == (SubSpeciesInfo)other;
		}

		public bool Equals(SubSpeciesInfo other)
		{
			return ID == other.ID;
		}

		public override int GetHashCode()
		{
			return ID.GetHashCode();
		}

		public string GetMutationsTooltip()
		{
			if (mutationIDs == null || mutationIDs.Count == 0)
			{
				return CREATURES.STATUSITEMS.ORIGINALPLANTMUTATION.TOOLTIP;
			}
			if (!instance.IsSubSpeciesIdentified(ID))
			{
				return CREATURES.STATUSITEMS.UNKNOWNMUTATION.TOOLTIP;
			}
			string id = mutationIDs[0];
			PlantMutation plantMutation = Db.Get().PlantMutations.Get(id);
			return CREATURES.STATUSITEMS.SPECIFICPLANTMUTATION.TOOLTIP.Replace("{MutationName}", plantMutation.Name) + "\n" + plantMutation.GetTooltip();
		}
	}

	public static PlantSubSpeciesCatalog instance;

	[Serialize]
	private Dictionary<Tag, List<SubSpeciesInfo>> discoveredSubspeciesBySpecies = new Dictionary<Tag, List<SubSpeciesInfo>>();

	[Serialize]
	private Dictionary<Tag, float> identificationProgress = new Dictionary<Tag, float>();

	[Serialize]
	private Dictionary<Tag, bool> identifiedSubSpecies = new Dictionary<Tag, bool>();

	public bool AnyNonOriginalDiscovered
	{
		get
		{
			foreach (KeyValuePair<Tag, List<SubSpeciesInfo>> discoveredSubspeciesBySpecy in discoveredSubspeciesBySpecies)
			{
				if (discoveredSubspeciesBySpecy.Value.Find((SubSpeciesInfo ss) => !ss.IsOriginal).IsValid)
				{
					return true;
				}
			}
			return false;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		instance = this;
	}

	private void EnsureOriginalSubSpecies()
	{
		foreach (GameObject item in Assets.GetPrefabsWithComponent<MutantPlant>())
		{
			MutantPlant component = item.GetComponent<MutantPlant>();
			Tag speciesID = component.SpeciesID;
			if (!discoveredSubspeciesBySpecies.ContainsKey(speciesID))
			{
				discoveredSubspeciesBySpecies[speciesID] = new List<SubSpeciesInfo>();
				discoveredSubspeciesBySpecies[speciesID].Add(component.GetSubSpeciesInfo());
			}
			if (identifiedSubSpecies == null)
			{
				identifiedSubSpecies = new Dictionary<Tag, bool>();
			}
			identifiedSubSpecies[component.SubSpeciesID] = true;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		EnsureOriginalSubSpecies();
	}

	public List<Tag> GetAllDiscoveredSpecies()
	{
		return discoveredSubspeciesBySpecies.Keys.ToList();
	}

	public List<SubSpeciesInfo> GetAllSubSpeciesForSpecies(Tag speciesID)
	{
		if (discoveredSubspeciesBySpecies.TryGetValue(speciesID, out var value))
		{
			return value;
		}
		return null;
	}

	public bool GetOriginalSubSpecies(Tag speciesID, out SubSpeciesInfo subSpeciesInfo)
	{
		if (!discoveredSubspeciesBySpecies.ContainsKey(speciesID))
		{
			subSpeciesInfo = default(SubSpeciesInfo);
			return false;
		}
		subSpeciesInfo = discoveredSubspeciesBySpecies[speciesID][0];
		return true;
	}

	public SubSpeciesInfo GetSubSpecies(Tag speciesID, Tag subSpeciesID)
	{
		return discoveredSubspeciesBySpecies[speciesID].Find((SubSpeciesInfo i) => i.ID == subSpeciesID);
	}

	public SubSpeciesInfo FindSubSpecies(Tag subSpeciesID)
	{
		foreach (KeyValuePair<Tag, List<SubSpeciesInfo>> discoveredSubspeciesBySpecy in discoveredSubspeciesBySpecies)
		{
			SubSpeciesInfo result = discoveredSubspeciesBySpecy.Value.Find((SubSpeciesInfo i) => i.ID == subSpeciesID);
			if (result.ID.IsValid)
			{
				return result;
			}
		}
		return default(SubSpeciesInfo);
	}

	public void DiscoverSubSpecies(SubSpeciesInfo newSubSpeciesInfo, MutantPlant source)
	{
		if (!discoveredSubspeciesBySpecies[newSubSpeciesInfo.speciesID].Contains(newSubSpeciesInfo))
		{
			discoveredSubspeciesBySpecies[newSubSpeciesInfo.speciesID].Add(newSubSpeciesInfo);
			Notification notification = new Notification(MISC.NOTIFICATIONS.NEWMUTANTSEED.NAME, NotificationType.Good, NewSubspeciesTooltipCB, newSubSpeciesInfo, expires: true, 0f, null, null, source.transform);
			base.gameObject.AddOrGet<Notifier>().Add(notification);
		}
	}

	private string NewSubspeciesTooltipCB(List<Notification> notifications, object data)
	{
		SubSpeciesInfo subSpeciesInfo = (SubSpeciesInfo)data;
		return MISC.NOTIFICATIONS.NEWMUTANTSEED.TOOLTIP.Replace("{Plant}", subSpeciesInfo.speciesID.ProperName());
	}

	public bool PartiallyIdentifySpecies(Tag speciesID, float amount)
	{
		if (!identificationProgress.ContainsKey(speciesID))
		{
			identificationProgress[speciesID] = 0f;
		}
		identificationProgress[speciesID] += amount;
		if (identificationProgress[speciesID] >= 1f)
		{
			List<SubSpeciesInfo> allUnidentifiedSubSpecies = GetAllUnidentifiedSubSpecies(speciesID);
			IdentifySubSpecies(allUnidentifiedSubSpecies.GetRandom());
			identificationProgress[speciesID] = 0f;
			Trigger(-98362560, speciesID);
			return true;
		}
		Trigger(-1531232972, speciesID);
		return false;
	}

	public void IdentifySubSpecies(SubSpeciesInfo subSpeciesInfo)
	{
		if (!identifiedSubSpecies.ContainsKey(subSpeciesInfo.ID))
		{
			identifiedSubSpecies[subSpeciesInfo.ID] = false;
		}
		bool flag = identifiedSubSpecies[subSpeciesInfo.ID];
		identifiedSubSpecies[subSpeciesInfo.ID] = true;
		if (flag)
		{
			return;
		}
		foreach (MutantPlant mutantPlant in Components.MutantPlants)
		{
			if (mutantPlant.HasTag(subSpeciesInfo.ID))
			{
				mutantPlant.UpdateNameAndTags();
			}
		}
		GeneticAnalysisCompleteMessage message = new GeneticAnalysisCompleteMessage(subSpeciesInfo.ID);
		Messenger.Instance.QueueMessage(message);
	}

	public bool IsSubSpeciesIdentified(Tag subSpeciesID)
	{
		return identifiedSubSpecies.ContainsKey(subSpeciesID) && identifiedSubSpecies[subSpeciesID];
	}

	public float GetIdentificationProgress(Tag speciesID)
	{
		return identificationProgress.ContainsKey(speciesID) ? identificationProgress[speciesID] : 0f;
	}

	public List<SubSpeciesInfo> GetAllUnidentifiedSubSpecies(Tag speciesID)
	{
		return discoveredSubspeciesBySpecies[speciesID].FindAll((SubSpeciesInfo ss) => !IsSubSpeciesIdentified(ss.ID));
	}
}
