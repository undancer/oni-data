using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/TinkerStation")]
public class TinkerStation : Workable, IGameObjectEffectDescriptor, ISim1000ms
{
	public HashedString choreType;

	public HashedString fetchChoreType;

	private Chore chore;

	[MyCmpAdd]
	private Operational operational;

	[MyCmpAdd]
	private Storage storage;

	public bool useFilteredStorage;

	protected FilteredStorage filteredStorage;

	public bool alwaysTinker;

	public float massPerTinker;

	public Tag inputMaterial;

	public Tag outputPrefab;

	public float outputTemperature;

	private static readonly EventSystem.IntraObjectHandler<TinkerStation> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<TinkerStation>(delegate(TinkerStation component, object data)
	{
		component.OnOperationalChanged(data);
	});

	public AttributeConverter AttributeConverter
	{
		set
		{
			attributeConverter = value;
		}
	}

	public float AttributeExperienceMultiplier
	{
		set
		{
			attributeExperienceMultiplier = value;
		}
	}

	public string SkillExperienceSkillGroup
	{
		set
		{
			skillExperienceSkillGroup = value;
		}
	}

	public float SkillExperienceMultiplier
	{
		set
		{
			skillExperienceMultiplier = value;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		attributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
		skillExperienceSkillGroup = Db.Get().SkillGroups.Technicals.Id;
		skillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
		if (useFilteredStorage)
		{
			ChoreType byHash = Db.Get().ChoreTypes.GetByHash(fetchChoreType);
			filteredStorage = new FilteredStorage(this, null, null, null, use_logic_meter: false, byHash);
		}
		SetWorkTime(15f);
		Subscribe(-592767678, OnOperationalChangedDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (useFilteredStorage && filteredStorage != null)
		{
			filteredStorage.FilterChanged();
		}
	}

	protected override void OnCleanUp()
	{
		if (filteredStorage != null)
		{
			filteredStorage.CleanUp();
		}
		base.OnCleanUp();
	}

	private bool CorrectRolePrecondition(MinionIdentity worker)
	{
		MinionResume component = worker.GetComponent<MinionResume>();
		if (component != null)
		{
			return component.HasPerk(requiredSkillPerk);
		}
		return false;
	}

	private void OnOperationalChanged(object data)
	{
		RoomTracker component = GetComponent<RoomTracker>();
		if (component != null && component.room != null)
		{
			component.room.RetriggerBuildings();
		}
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		if (operational.IsOperational)
		{
			operational.SetActive(value: true);
		}
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
		ShowProgressBar(show: false);
		operational.SetActive(value: false);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		storage.ConsumeAndGetDisease(inputMaterial, massPerTinker, out var _, out var _, out var _);
		GameObject obj = GameUtil.KInstantiate(Assets.GetPrefab(outputPrefab), base.transform.GetPosition(), Grid.SceneLayer.Ore);
		obj.GetComponent<PrimaryElement>().Temperature = outputTemperature;
		obj.SetActive(value: true);
		chore = null;
	}

	public void Sim1000ms(float dt)
	{
		UpdateChore();
	}

	private void UpdateChore()
	{
		if (operational.IsOperational && (ToolsRequested() || alwaysTinker) && HasMaterial())
		{
			if (chore == null)
			{
				chore = new WorkChore<TinkerStation>(Db.Get().ChoreTypes.GetByHash(choreType), this);
				chore.AddPrecondition(ChorePreconditions.instance.HasSkillPerk, requiredSkillPerk);
				SetWorkTime(workTime);
			}
		}
		else if (chore != null)
		{
			chore.Cancel("Can't tinker");
			chore = null;
		}
	}

	private bool HasMaterial()
	{
		return storage.MassStored() > 0f;
	}

	private bool ToolsRequested()
	{
		if (MaterialNeeds.GetAmount(outputPrefab, base.gameObject.GetMyWorldId(), includeRelatedWorlds: false) > 0f)
		{
			return this.GetMyWorld().worldInventory.GetAmount(outputPrefab, includeRelatedWorlds: true) <= 0f;
		}
		return false;
	}

	public override List<Descriptor> GetDescriptors(GameObject go)
	{
		string arg = inputMaterial.ProperName();
		List<Descriptor> descriptors = base.GetDescriptors(go);
		descriptors.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTCONSUMEDPERUSE, arg, GameUtil.GetFormattedMass(massPerTinker)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMEDPERUSE, arg, GameUtil.GetFormattedMass(massPerTinker)), Descriptor.DescriptorType.Requirement));
		descriptors.AddRange(GameUtil.GetAllDescriptors(Assets.GetPrefab(outputPrefab)));
		List<Tinkerable> list = new List<Tinkerable>();
		foreach (GameObject item2 in Assets.GetPrefabsWithComponent<Tinkerable>())
		{
			Tinkerable component = item2.GetComponent<Tinkerable>();
			if (component.tinkerMaterialTag == outputPrefab)
			{
				list.Add(component);
			}
		}
		if (list.Count > 0)
		{
			Effect effect = Db.Get().effects.Get(list[0].addedEffect);
			descriptors.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.ADDED_EFFECT, effect.Name), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ADDED_EFFECT, effect.Name, Effect.CreateTooltip(effect, showDuration: true))));
			descriptors.Add(new Descriptor(UI.BUILDINGEFFECTS.IMPROVED_BUILDINGS, UI.BUILDINGEFFECTS.TOOLTIPS.IMPROVED_BUILDINGS));
			{
				foreach (Tinkerable item3 in list)
				{
					Descriptor item = new Descriptor(string.Format(UI.BUILDINGEFFECTS.IMPROVED_BUILDINGS_ITEM, item3.GetProperName()), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.IMPROVED_BUILDINGS_ITEM, item3.GetProperName()));
					item.IncreaseIndent();
					descriptors.Add(item);
				}
				return descriptors;
			}
		}
		return descriptors;
	}

	public static TinkerStation AddTinkerStation(GameObject go, string required_room_type)
	{
		TinkerStation result = go.AddOrGet<TinkerStation>();
		go.AddOrGet<RoomTracker>().requiredRoomType = required_room_type;
		return result;
	}
}
