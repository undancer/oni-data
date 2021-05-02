using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class Db : EntityModifierSet
{
	[Serializable]
	public class SlotInfo : Resource
	{
	}

	private static Db _Instance;

	public TextAsset researchTreeFileVanilla;

	public TextAsset researchTreeFileExpansion1;

	public Diseases Diseases;

	public Database.Sicknesses Sicknesses;

	public Urges Urges;

	public AssignableSlots AssignableSlots;

	public StateMachineCategories StateMachineCategories;

	public Personalities Personalities;

	public Faces Faces;

	public Shirts Shirts;

	public Expressions Expressions;

	public Thoughts Thoughts;

	public BuildingStatusItems BuildingStatusItems;

	public MiscStatusItems MiscStatusItems;

	public CreatureStatusItems CreatureStatusItems;

	public RobotStatusItems RobotStatusItems;

	public StatusItemCategories StatusItemCategories;

	public Deaths Deaths;

	public ChoreTypes ChoreTypes;

	public TechItems TechItems;

	public AccessorySlots AccessorySlots;

	public Accessories Accessories;

	public ScheduleBlockTypes ScheduleBlockTypes;

	public ScheduleGroups ScheduleGroups;

	public RoomTypeCategories RoomTypeCategories;

	public RoomTypes RoomTypes;

	public ArtifactDropRates ArtifactDropRates;

	public SpaceDestinationTypes SpaceDestinationTypes;

	public SkillPerks SkillPerks;

	public SkillGroups SkillGroups;

	public Skills Skills;

	public ColonyAchievements ColonyAchievements;

	public GameplayEvents GameplayEvents;

	public GameplaySeasons GameplaySeasons;

	public PlantMutations PlantMutations;

	public Techs Techs;

	public TechTreeTitles TechTreeTitles;

	public static Db Get()
	{
		if (_Instance == null)
		{
			_Instance = Resources.Load<Db>("Db");
			_Instance.Initialize();
		}
		return _Instance;
	}

	public override void Initialize()
	{
		base.Initialize();
		Urges = new Urges();
		AssignableSlots = new AssignableSlots();
		StateMachineCategories = new StateMachineCategories();
		Personalities = new Personalities();
		Faces = new Faces();
		Shirts = new Shirts();
		Expressions = new Expressions(Root);
		Thoughts = new Thoughts(Root);
		Deaths = new Deaths(Root);
		StatusItemCategories = new StatusItemCategories(Root);
		TechTreeTitles = new TechTreeTitles(Root);
		TechTreeTitles.Load(DlcManager.IsExpansion1Active() ? researchTreeFileExpansion1 : researchTreeFileVanilla);
		Techs = new Techs(Root);
		TechItems = new TechItems(Root);
		Techs.Init();
		Techs.Load(DlcManager.IsExpansion1Active() ? researchTreeFileExpansion1 : researchTreeFileVanilla);
		TechItems.Init();
		Accessories = new Accessories(Root);
		AccessorySlots = new AccessorySlots(Root);
		ScheduleBlockTypes = new ScheduleBlockTypes(Root);
		ScheduleGroups = new ScheduleGroups(Root);
		RoomTypeCategories = new RoomTypeCategories(Root);
		RoomTypes = new RoomTypes(Root);
		ArtifactDropRates = new ArtifactDropRates(Root);
		SpaceDestinationTypes = new SpaceDestinationTypes(Root);
		Diseases = new Diseases(Root);
		Sicknesses = new Database.Sicknesses(Root);
		SkillPerks = new SkillPerks(Root);
		SkillGroups = new SkillGroups(Root);
		Skills = new Skills(Root);
		ColonyAchievements = new ColonyAchievements(Root);
		MiscStatusItems = new MiscStatusItems(Root);
		CreatureStatusItems = new CreatureStatusItems(Root);
		BuildingStatusItems = new BuildingStatusItems(Root);
		RobotStatusItems = new RobotStatusItems(Root);
		ChoreTypes = new ChoreTypes(Root);
		GameplayEvents = new GameplayEvents(Root);
		GameplaySeasons = new GameplaySeasons(Root);
		PlantMutations = new PlantMutations(Root);
		Effect effect = new Effect("CenterOfAttention", DUPLICANTS.MODIFIERS.CENTEROFATTENTION.NAME, DUPLICANTS.MODIFIERS.CENTEROFATTENTION.TOOLTIP, 0f, show_in_ui: true, trigger_floating_text: true, is_bad: false);
		effect.Add(new AttributeModifier("StressDelta", -0.008333334f, DUPLICANTS.MODIFIERS.CENTEROFATTENTION.NAME));
		effects.Add(effect);
		CollectResources(Root, ResourceTable);
	}

	private void CollectResources(Resource resource, List<Resource> resource_table)
	{
		if (resource.Guid != null)
		{
			resource_table.Add(resource);
		}
		ResourceSet resourceSet = resource as ResourceSet;
		if (resourceSet != null)
		{
			for (int i = 0; i < resourceSet.Count; i++)
			{
				CollectResources(resourceSet.GetResource(i), resource_table);
			}
		}
	}

	public ResourceType GetResource<ResourceType>(ResourceGuid guid) where ResourceType : Resource
	{
		Resource resource = ResourceTable.FirstOrDefault((Resource s) => s.Guid == guid);
		if (resource == null)
		{
			Debug.LogWarning("Could not find resource: " + guid);
			return null;
		}
		ResourceType val = (ResourceType)resource;
		if (val == null)
		{
			Debug.LogError("Resource type mismatch for resource: " + resource.Id + "\nExpecting Type: " + typeof(ResourceType).Name + "\nGot Type: " + resource.GetType().Name);
			return null;
		}
		return val;
	}
}
