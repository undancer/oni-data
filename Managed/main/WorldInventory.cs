using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/WorldInventory")]
public class WorldInventory : KMonoBehaviour, ISaveLoadable
{
	[Serialize]
	private HashSet<Tag> Discovered = new HashSet<Tag>();

	[Serialize]
	private Dictionary<Tag, HashSet<Tag>> DiscoveredCategories = new Dictionary<Tag, HashSet<Tag>>();

	private Dictionary<Tag, HashSet<Pickupable>> Inventory = new Dictionary<Tag, HashSet<Pickupable>>();

	private MinionGroupProber Prober;

	private Dictionary<Tag, float> accessibleAmounts = new Dictionary<Tag, float>();

	private static readonly EventSystem.IntraObjectHandler<WorldInventory> OnNewDayDelegate = new EventSystem.IntraObjectHandler<WorldInventory>(delegate(WorldInventory component, object data)
	{
		component.GenerateInventoryReport(data);
	});

	private int accessibleUpdateIndex;

	private bool firstUpdate = true;

	public static WorldInventory Instance
	{
		get;
		private set;
	}

	public event Action<Tag, Tag> OnDiscover;

	protected override void OnPrefabInit()
	{
		Instance = this;
		Subscribe(Game.Instance.gameObject, -1588644844, OnAddedFetchable);
		Subscribe(Game.Instance.gameObject, -1491270284, OnRemovedFetchable);
		Subscribe(631075836, OnNewDayDelegate);
	}

	private void GenerateInventoryReport(object data)
	{
		int num = 0;
		int num2 = 0;
		foreach (object brain in Components.Brains)
		{
			CreatureBrain creatureBrain = brain as CreatureBrain;
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

	public float GetTotalAmount(Tag tag)
	{
		float value = 0f;
		accessibleAmounts.TryGetValue(tag, out value);
		return value;
	}

	public ICollection<Pickupable> GetPickupables(Tag tag)
	{
		HashSet<Pickupable> value = null;
		Inventory.TryGetValue(tag, out value);
		return value;
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

	public float GetAmount(Tag tag)
	{
		return Mathf.Max(GetTotalAmount(tag) - MaterialNeeds.Instance.GetAmount(tag), 0f);
	}

	public void Discover(Tag tag, Tag categoryTag)
	{
		bool num = Discovered.Add(tag);
		DiscoverCategory(categoryTag, tag);
		if (num && this.OnDiscover != null)
		{
			this.OnDiscover(categoryTag, tag);
		}
	}

	private void DiscoverCategory(Tag category_tag, Tag item_tag)
	{
		if (!DiscoveredCategories.TryGetValue(category_tag, out var value))
		{
			value = new HashSet<Tag>();
			DiscoveredCategories[category_tag] = value;
		}
		value.Add(item_tag);
	}

	public HashSet<Tag> GetDiscovered()
	{
		return Discovered;
	}

	public bool IsDiscovered(Tag tag)
	{
		if (!Discovered.Contains(tag))
		{
			return DiscoveredCategories.ContainsKey(tag);
		}
		return true;
	}

	public bool AnyDiscovered(ICollection<Tag> tags)
	{
		foreach (Tag tag in tags)
		{
			if (IsDiscovered(tag))
			{
				return true;
			}
		}
		return false;
	}

	public bool Contains(Recipe.Ingredient[] ingredients)
	{
		bool result = true;
		foreach (Recipe.Ingredient ingredient in ingredients)
		{
			if (GetAmount(ingredient.tag) < ingredient.amount)
			{
				result = false;
				break;
			}
		}
		return result;
	}

	public bool TryGetDiscoveredResourcesFromTag(Tag tag, out HashSet<Tag> resources)
	{
		return DiscoveredCategories.TryGetValue(tag, out resources);
	}

	public HashSet<Tag> GetDiscoveredResourcesFromTag(Tag tag)
	{
		if (DiscoveredCategories.TryGetValue(tag, out var value))
		{
			return value;
		}
		return new HashSet<Tag>();
	}

	public Dictionary<Tag, HashSet<Tag>> GetDiscoveredResourcesFromTagSet(TagSet tagSet)
	{
		Dictionary<Tag, HashSet<Tag>> dictionary = new Dictionary<Tag, HashSet<Tag>>();
		foreach (Tag item in tagSet)
		{
			if (DiscoveredCategories.TryGetValue(item, out var value))
			{
				dictionary[item] = value;
			}
		}
		return dictionary;
	}

	private void Update()
	{
		int num = 0;
		Dictionary<Tag, HashSet<Pickupable>>.Enumerator enumerator = Inventory.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<Tag, HashSet<Pickupable>> current = enumerator.Current;
			if (num == accessibleUpdateIndex || firstUpdate)
			{
				Tag key = current.Key;
				HashSet<Pickupable> value = current.Value;
				float num2 = 0f;
				foreach (Pickupable item in (IEnumerable<Pickupable>)value)
				{
					if (item != null && !item.HasTag(GameTags.StoredPrivate))
					{
						num2 += item.TotalAmount;
					}
				}
				accessibleAmounts[key] = num2;
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
		Instance = null;
	}

	public static Tag GetCategoryForTags(HashSet<Tag> tags)
	{
		Tag invalid = Tag.Invalid;
		foreach (Tag tag in tags)
		{
			if (GameTags.AllCategories.Contains(tag))
			{
				return tag;
			}
		}
		return invalid;
	}

	public static Tag GetCategoryForEntity(KPrefabID entity)
	{
		ElementChunk component = entity.GetComponent<ElementChunk>();
		if (component != null)
		{
			return component.GetComponent<PrimaryElement>().Element.materialCategory;
		}
		return GetCategoryForTags(entity.Tags);
	}

	private void OnAddedFetchable(object data)
	{
		GameObject gameObject = (GameObject)data;
		if (gameObject.GetComponent<Navigator>() != null)
		{
			return;
		}
		Pickupable component = gameObject.GetComponent<Pickupable>();
		KPrefabID component2 = component.GetComponent<KPrefabID>();
		Tag tag = component2.PrefabID();
		if (!Inventory.ContainsKey(tag))
		{
			Tag categoryForEntity = GetCategoryForEntity(component2);
			DebugUtil.DevAssertArgs(categoryForEntity.IsValid, component.name, "was found by worldinventory but doesn't have a category! Add it to the element definition.");
			Discover(tag, categoryForEntity);
		}
		foreach (Tag tag2 in component2.Tags)
		{
			if (!Inventory.TryGetValue(tag2, out var value))
			{
				value = new HashSet<Pickupable>();
				Inventory[tag2] = value;
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
