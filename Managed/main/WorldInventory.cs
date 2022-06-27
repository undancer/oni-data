using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/WorldInventory")]
public class WorldInventory : KMonoBehaviour, ISaveLoadable
{
	private WorldContainer m_worldContainer;

	[Serialize]
	public List<Tag> pinnedResources = new List<Tag>();

	[Serialize]
	public List<Tag> notifyResources = new List<Tag>();

	private Dictionary<Tag, HashSet<Pickupable>> Inventory = new Dictionary<Tag, HashSet<Pickupable>>();

	private MinionGroupProber Prober;

	private Dictionary<Tag, float> accessibleAmounts = new Dictionary<Tag, float>();

	private bool hasValidCount;

	private static readonly EventSystem.IntraObjectHandler<WorldInventory> OnNewDayDelegate = new EventSystem.IntraObjectHandler<WorldInventory>(delegate(WorldInventory component, object data)
	{
		component.GenerateInventoryReport(data);
	});

	private int accessibleUpdateIndex;

	private bool firstUpdate = true;

	public WorldContainer WorldContainer
	{
		get
		{
			if (m_worldContainer == null)
			{
				m_worldContainer = GetComponent<WorldContainer>();
			}
			return m_worldContainer;
		}
	}

	public bool HasValidCount => hasValidCount;

	private int worldId
	{
		get
		{
			WorldContainer worldContainer = WorldContainer;
			if (!(worldContainer != null))
			{
				return -1;
			}
			return worldContainer.id;
		}
	}

	protected override void OnPrefabInit()
	{
		Subscribe(Game.Instance.gameObject, -1588644844, OnAddedFetchable);
		Subscribe(Game.Instance.gameObject, -1491270284, OnRemovedFetchable);
		Subscribe(631075836, OnNewDayDelegate);
		m_worldContainer = GetComponent<WorldContainer>();
	}

	protected override void OnCleanUp()
	{
		Unsubscribe(Game.Instance.gameObject, -1588644844, OnAddedFetchable);
		Unsubscribe(Game.Instance.gameObject, -1491270284, OnRemovedFetchable);
		base.OnCleanUp();
	}

	private void GenerateInventoryReport(object data)
	{
		int num = 0;
		int num2 = 0;
		foreach (Brain worldItem in Components.Brains.GetWorldItems(worldId))
		{
			CreatureBrain creatureBrain = worldItem as CreatureBrain;
			if (creatureBrain != null)
			{
				if (creatureBrain.HasTag(GameTags.Creatures.Wild))
				{
					num++;
					ReportManager.Instance.ReportValue(ReportManager.ReportType.WildCritters, 1f, creatureBrain.GetProperName(), creatureBrain.GetProperName());
				}
				else
				{
					num2++;
					ReportManager.Instance.ReportValue(ReportManager.ReportType.DomesticatedCritters, 1f, creatureBrain.GetProperName(), creatureBrain.GetProperName());
				}
			}
		}
		if (DlcManager.IsExpansion1Active())
		{
			WorldContainer component = GetComponent<WorldContainer>();
			if (component != null && component.IsModuleInterior)
			{
				Clustercraft clustercraft = component.GetComponent<ClusterGridEntity>() as Clustercraft;
				if (clustercraft != null && clustercraft.Status != 0)
				{
					ReportManager.Instance.ReportValue(ReportManager.ReportType.RocketsInFlight, 1f, clustercraft.Name);
				}
			}
			return;
		}
		foreach (Spacecraft item in SpacecraftManager.instance.GetSpacecraft())
		{
			if (item.state != 0 && item.state != Spacecraft.MissionState.Destroyed)
			{
				ReportManager.Instance.ReportValue(ReportManager.ReportType.RocketsInFlight, 1f, item.rocketName);
			}
		}
	}

	protected override void OnSpawn()
	{
		Prober = MinionGroupProber.Get();
		StartCoroutine(InitialRefresh());
	}

	private IEnumerator InitialRefresh()
	{
		int i = 0;
		while (i < 1)
		{
			yield return null;
			int num = i + 1;
			i = num;
		}
		for (int j = 0; j < Components.Pickupables.Count; j++)
		{
			Pickupable pickupable = Components.Pickupables[j];
			if (pickupable != null)
			{
				pickupable.GetSMI<ReachabilityMonitor.Instance>()?.UpdateReachability();
			}
		}
	}

	public bool IsReachable(Pickupable pickupable)
	{
		return Prober.IsReachable(pickupable);
	}

	public float GetTotalAmount(Tag tag, bool includeRelatedWorlds)
	{
		float value = 0f;
		accessibleAmounts.TryGetValue(tag, out value);
		return value;
	}

	public ICollection<Pickupable> GetPickupables(Tag tag, bool includeRelatedWorlds = false)
	{
		if (!includeRelatedWorlds)
		{
			HashSet<Pickupable> value = null;
			Inventory.TryGetValue(tag, out value);
			return value;
		}
		return ClusterUtil.GetPickupablesFromRelatedWorlds(this, tag);
	}

	public List<Pickupable> CreatePickupablesList(Tag tag)
	{
		HashSet<Pickupable> value = null;
		Inventory.TryGetValue(tag, out value);
		return value?.ToList();
	}

	public List<Tag> GetPickupableTagsFromCategoryTag(Tag t)
	{
		List<Tag> list = new List<Tag>();
		ICollection<Pickupable> pickupables = GetPickupables(t);
		if (pickupables != null && pickupables.Count > 0)
		{
			foreach (Pickupable item in pickupables)
			{
				list.AddRange(item.KPrefabID.Tags);
			}
			return list;
		}
		return list;
	}

	public float GetAmount(Tag tag, bool includeRelatedWorlds)
	{
		float num = 0f;
		if (!includeRelatedWorlds)
		{
			num = GetTotalAmount(tag, includeRelatedWorlds);
			num -= MaterialNeeds.GetAmount(tag, worldId, includeRelatedWorlds);
		}
		else
		{
			num = ClusterUtil.GetAmountFromRelatedWorlds(this, tag);
		}
		return Mathf.Max(num, 0f);
	}

	public int GetCountWithAdditionalTag(Tag tag, Tag additionalTag, bool includeRelatedWorlds = false)
	{
		ICollection<Pickupable> collection;
		if (!includeRelatedWorlds)
		{
			collection = GetPickupables(tag);
		}
		else
		{
			ICollection<Pickupable> pickupablesFromRelatedWorlds = ClusterUtil.GetPickupablesFromRelatedWorlds(this, tag);
			collection = pickupablesFromRelatedWorlds;
		}
		ICollection<Pickupable> collection2 = collection;
		int num = 0;
		if (collection2 != null)
		{
			if (additionalTag.IsValid)
			{
				{
					foreach (Pickupable item in collection2)
					{
						if (item.HasTag(additionalTag))
						{
							num++;
						}
					}
					return num;
				}
			}
			num = collection2.Count;
		}
		return num;
	}

	public float GetAmountWithoutTag(Tag tag, bool includeRelatedWorlds = false, Tag[] forbiddenTags = null)
	{
		if (forbiddenTags == null)
		{
			return GetAmount(tag, includeRelatedWorlds);
		}
		float num = 0f;
		ICollection<Pickupable> collection;
		if (!includeRelatedWorlds)
		{
			collection = GetPickupables(tag);
		}
		else
		{
			ICollection<Pickupable> pickupablesFromRelatedWorlds = ClusterUtil.GetPickupablesFromRelatedWorlds(this, tag);
			collection = pickupablesFromRelatedWorlds;
		}
		ICollection<Pickupable> collection2 = collection;
		if (collection2 != null)
		{
			foreach (Pickupable item in collection2)
			{
				if (item != null && !item.HasTag(GameTags.StoredPrivate) && !item.HasAnyTags(forbiddenTags))
				{
					num += item.TotalAmount;
				}
			}
			return num;
		}
		return num;
	}

	private void Update()
	{
		int num = 0;
		Dictionary<Tag, HashSet<Pickupable>>.Enumerator enumerator = Inventory.GetEnumerator();
		int num2 = worldId;
		while (enumerator.MoveNext())
		{
			KeyValuePair<Tag, HashSet<Pickupable>> current = enumerator.Current;
			if (num == accessibleUpdateIndex || firstUpdate)
			{
				Tag key = current.Key;
				HashSet<Pickupable> value = current.Value;
				float num3 = 0f;
				foreach (Pickupable item in (IEnumerable<Pickupable>)value)
				{
					if (item != null && item.GetMyWorldId() == num2 && !item.HasTag(GameTags.StoredPrivate))
					{
						num3 += item.TotalAmount;
					}
				}
				if (!hasValidCount && accessibleUpdateIndex + 1 >= Inventory.Count)
				{
					hasValidCount = true;
					if (worldId == ClusterManager.Instance.activeWorldId)
					{
						hasValidCount = true;
						PinnedResourcesPanel.Instance.ClearExcessiveNewItems();
						PinnedResourcesPanel.Instance.Refresh();
					}
				}
				accessibleAmounts[key] = num3;
				accessibleUpdateIndex = (accessibleUpdateIndex + 1) % Inventory.Count;
				break;
			}
			num++;
		}
		firstUpdate = false;
	}

	protected override void OnLoadLevel()
	{
		base.OnLoadLevel();
	}

	private void OnAddedFetchable(object data)
	{
		GameObject gameObject = (GameObject)data;
		if (gameObject.GetComponent<Navigator>() != null)
		{
			return;
		}
		Pickupable component = gameObject.GetComponent<Pickupable>();
		if (component.GetMyWorldId() != worldId)
		{
			return;
		}
		KPrefabID component2 = component.GetComponent<KPrefabID>();
		Tag key = component2.PrefabID();
		if (!Inventory.ContainsKey(key))
		{
			Tag categoryForEntity = DiscoveredResources.GetCategoryForEntity(component2);
			DebugUtil.DevAssertArgs(categoryForEntity.IsValid, component.name, "was found by worldinventory but doesn't have a category! Add it to the element definition.");
			DiscoveredResources.Instance.Discover(key, categoryForEntity);
		}
		foreach (Tag tag in component2.Tags)
		{
			if (!Inventory.TryGetValue(tag, out var value))
			{
				value = new HashSet<Pickupable>();
				Inventory[tag] = value;
			}
			value.Add(component);
		}
	}

	private void OnRemovedFetchable(object data)
	{
		Pickupable component = ((GameObject)data).GetComponent<Pickupable>();
		foreach (Tag tag in component.GetComponent<KPrefabID>().Tags)
		{
			if (Inventory.TryGetValue(tag, out var value))
			{
				value.Remove(component);
			}
		}
	}

	public Dictionary<Tag, float> GetAccessibleAmounts()
	{
		return accessibleAmounts;
	}
}
