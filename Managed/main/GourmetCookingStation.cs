using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class GourmetCookingStation : ComplexFabricator, IGameObjectEffectDescriptor
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, GourmetCookingStation, object>.GameInstance
	{
		public StatesInstance(GourmetCookingStation smi)
			: base(smi)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, GourmetCookingStation>
	{
		public static StatusItem waitingForFuelStatus;

		public State waitingForFuel;

		public State ready;

		public override void InitializeStates(out BaseState default_state)
		{
			if (waitingForFuelStatus == null)
			{
				waitingForFuelStatus = new StatusItem("waitingForFuelStatus", BUILDING.STATUSITEMS.ENOUGH_FUEL.NAME, BUILDING.STATUSITEMS.ENOUGH_FUEL.TOOLTIP, "status_item_no_gas_to_pump", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
				waitingForFuelStatus.resolveStringCallback = delegate(string str, object obj)
				{
					GourmetCookingStation gourmetCookingStation = (GourmetCookingStation)obj;
					return string.Format(str, gourmetCookingStation.fuelTag.ProperName(), GameUtil.GetFormattedMass(5f));
				};
			}
			default_state = waitingForFuel;
			waitingForFuel.Enter(delegate(StatesInstance smi)
			{
				smi.master.operational.SetFlag(gourmetCookingStationFlag, value: false);
			}).ToggleStatusItem(waitingForFuelStatus, (StatesInstance smi) => smi.master).EventTransition(GameHashes.OnStorageChange, ready, (StatesInstance smi) => smi.master.GetAvailableFuel() >= 5f);
			ready.Enter(delegate(StatesInstance smi)
			{
				smi.master.SetQueueDirty();
				smi.master.operational.SetFlag(gourmetCookingStationFlag, value: true);
			}).EventTransition(GameHashes.OnStorageChange, waitingForFuel, (StatesInstance smi) => smi.master.GetAvailableFuel() <= 0f);
		}
	}

	private static readonly Operational.Flag gourmetCookingStationFlag = new Operational.Flag("gourmet_cooking_station", Operational.Flag.Type.Requirement);

	public float GAS_CONSUMPTION_RATE;

	public float GAS_CONVERSION_RATIO = 0.1f;

	public const float START_FUEL_MASS = 5f;

	public Tag fuelTag;

	[SerializeField]
	private int diseaseCountKillRate = 150;

	private StatesInstance smi;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		keepAdditionalTags.SetTag(fuelTag);
		choreType = Db.Get().ChoreTypes.Cook;
		fetchChoreTypeIdHash = Db.Get().ChoreTypes.CookFetch.IdHash;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		workable.requiredSkillPerk = Db.Get().SkillPerks.CanElectricGrill.Id;
		workable.WorkerStatusItem = Db.Get().DuplicantStatusItems.Cooking;
		workable.overrideAnims = new KAnimFile[1] { Assets.GetAnim("anim_interacts_cookstation_gourtmet_kanim") };
		workable.AttributeConverter = Db.Get().AttributeConverters.CookingSpeed;
		workable.AttributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
		workable.SkillExperienceSkillGroup = Db.Get().SkillGroups.Cooking.Id;
		workable.SkillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
		ComplexFabricatorWorkable complexFabricatorWorkable = workable;
		complexFabricatorWorkable.OnWorkTickActions = (Action<Worker, float>)Delegate.Combine(complexFabricatorWorkable.OnWorkTickActions, (Action<Worker, float>)delegate(Worker worker, float dt)
		{
			Debug.Assert(worker != null, "How did we get a null worker?");
			if (diseaseCountKillRate > 0)
			{
				PrimaryElement component = GetComponent<PrimaryElement>();
				int num = Math.Max(1, (int)((float)diseaseCountKillRate * dt));
				component.ModifyDiseaseCount(-num, "GourmetCookingStation");
			}
		});
		smi = new StatesInstance(this);
		smi.StartSM();
	}

	public float GetAvailableFuel()
	{
		return inStorage.GetAmountAvailable(fuelTag);
	}

	protected override List<GameObject> SpawnOrderProduct(ComplexRecipe recipe)
	{
		List<GameObject> list = base.SpawnOrderProduct(recipe);
		foreach (GameObject item in list)
		{
			PrimaryElement component = item.GetComponent<PrimaryElement>();
			component.ModifyDiseaseCount(-component.DiseaseCount, "GourmetCookingStation.CompleteOrder");
		}
		GetComponent<Operational>().SetActive(value: false);
		return list;
	}

	public override List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> descriptors = base.GetDescriptors(go);
		descriptors.Add(new Descriptor(UI.BUILDINGEFFECTS.REMOVES_DISEASE, UI.BUILDINGEFFECTS.TOOLTIPS.REMOVES_DISEASE));
		return descriptors;
	}
}
