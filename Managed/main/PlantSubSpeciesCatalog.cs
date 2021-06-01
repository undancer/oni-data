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

		public string GetMutationsNames()
		{
			if (mutationIDs == null || mutationIDs.Count == 0)
			{
				return CREATURES.PLANT_MUTATIONS.NONE.NAME;
			}
			return string.Join(", ", Db.Get().PlantMutations.GetNamesForMutations(mutationIDs));
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
			if (!Instance.IsSubSpeciesIdentified(ID))
			{
				return CREATURES.STATUSITEMS.UNKNOWNMUTATION.TOOLTIP;
			}
			string id = mutationIDs[0];
			PlantMutation plantMutation = Db.Get().PlantMutations.Get(id);
			return CREATURES.STATUSITEMS.SPECIFICPLANTMUTATION.TOOLTIP.Replace("{MutationName}", plantMutation.Name) + "\n" + plantMutation.GetTooltip();
		}
	}

	public static PlantSubSpeciesCatalog Instance;

	[Serialize]
	private Dictionary<Tag, List<SubSpeciesInfo>> discoveredSubspeciesBySpecies = new Dictionary<Tag, List<SubSpeciesInfo>>();

	[Serialize]
	private HashSet<Tag> identifiedSubSpecies = new HashSet<Tag>();

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

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
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
		subSpeciesInfo = discoveredSubspeciesBySpecies[speciesID].Find((SubSpeciesInfo i) => i.IsOriginal);
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

	public void IdentifySubSpecies(Tag subSpeciesID)
	{
		if (!identifiedSubSpecies.Add(subSpeciesID))
		{
			return;
		}
		SubSpeciesInfo subSpeciesInfo = FindSubSpecies(subSpeciesID);
		foreach (MutantPlant mutantPlant in Components.MutantPlants)
		{
			if (mutantPlant.HasTag(subSpeciesID))
			{
				mutantPlant.UpdateNameAndTags();
			}
		}
		GeneticAnalysisCompleteMessage message = new GeneticAnalysisCompleteMessage(subSpeciesID);
		Messenger.Instance.QueueMessage(message);
	}

	public bool IsSubSpeciesIdentified(Tag subSpeciesID)
	{
		return identifiedSubSpecies.Contains(subSpeciesID);
	}

	public List<SubSpeciesInfo> GetAllUnidentifiedSubSpecies(Tag speciesID)
	{
		return discoveredSubspeciesBySpecies[speciesID].FindAll((SubSpeciesInfo ss) => !IsSubSpeciesIdentified(ss.ID));
	}

	public bool IsValidPlantableSeed(Tag seedID, Tag subspeciesID)
	{
		if (!seedID.IsValid)
		{
			return false;
		}
		GameObject prefab = Assets.GetPrefab(seedID);
		MutantPlant component = prefab.GetComponent<MutantPlant>();
		if (component == null)
		{
			return !subspeciesID.IsValid;
		}
		List<SubSpeciesInfo> allSubSpeciesForSpecies = Instance.GetAllSubSpeciesForSpecies(component.SpeciesID);
		return allSubSpeciesForSpecies != null && allSubSpeciesForSpecies.FindIndex((SubSpeciesInfo s) => s.ID == subspeciesID) != -1 && Instance.IsSubSpeciesIdentified(subspeciesID);
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
			identifiedSubSpecies.Add(component.SubSpeciesID);
		}
	}
}
