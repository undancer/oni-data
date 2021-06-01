using System.Collections.Generic;
using Database;
using KSerialization;
using STRINGS;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Spacecraft
{
	public enum MissionState
	{
		Grounded,
		Launching,
		Underway,
		WaitingToLand,
		Landing,
		Destroyed
	}

	[Serialize]
	public int id = -1;

	[Serialize]
	public string rocketName = UI.STARMAP.DEFAULT_NAME;

	[Serialize]
	public Ref<LaunchConditionManager> refLaunchConditions = new Ref<LaunchConditionManager>();

	[Serialize]
	public MissionState state;

	[Serialize]
	private float missionElapsed = 0f;

	[Serialize]
	private float missionDuration = 0f;

	public LaunchConditionManager launchConditions
	{
		get
		{
			return refLaunchConditions.Get();
		}
		set
		{
			refLaunchConditions.Set(value);
		}
	}

	public Spacecraft(LaunchConditionManager launchConditions)
	{
		this.launchConditions = launchConditions;
	}

	public Spacecraft()
	{
	}

	public void SetRocketName(string newName)
	{
		rocketName = newName;
		UpdateNameOnRocketModules();
	}

	public string GetRocketName()
	{
		return rocketName;
	}

	public void UpdateNameOnRocketModules()
	{
		foreach (GameObject item in AttachableBuilding.GetAttachedNetwork(launchConditions.GetComponent<AttachableBuilding>()))
		{
			RocketModule component = item.GetComponent<RocketModule>();
			if (component != null)
			{
				component.SetParentRocketName(rocketName);
			}
		}
	}

	public bool HasInvalidID()
	{
		return id == -1;
	}

	public void SetID(int id)
	{
		this.id = id;
	}

	public void SetState(MissionState state)
	{
		this.state = state;
	}

	public void BeginMission(SpaceDestination destination)
	{
		missionElapsed = 0f;
		missionDuration = (float)destination.OneBasedDistance * ROCKETRY.MISSION_DURATION_SCALE / GetPilotNavigationEfficiency();
		SetState(MissionState.Launching);
	}

	private float GetPilotNavigationEfficiency()
	{
		MinionStorage component = launchConditions.GetComponent<MinionStorage>();
		List<MinionStorage.Info> storedMinionInfo = component.GetStoredMinionInfo();
		if (storedMinionInfo.Count < 1)
		{
			return 1f;
		}
		StoredMinionIdentity component2 = storedMinionInfo[0].serializedMinion.Get().GetComponent<StoredMinionIdentity>();
		string b = Db.Get().Attributes.SpaceNavigation.Id;
		float num = 1f;
		foreach (KeyValuePair<string, bool> item in component2.MasteryBySkillID)
		{
			foreach (SkillPerk perk in Db.Get().Skills.Get(item.Key).perks)
			{
				SkillAttributePerk skillAttributePerk = perk as SkillAttributePerk;
				if (skillAttributePerk != null && skillAttributePerk.modifier.AttributeId == b)
				{
					num += skillAttributePerk.modifier.Value;
				}
			}
		}
		return num;
	}

	public void ForceComplete()
	{
		missionElapsed = missionDuration;
	}

	public void ProgressMission(float deltaTime)
	{
		if (state == MissionState.Underway)
		{
			missionElapsed += deltaTime;
			if (missionElapsed > missionDuration)
			{
				CompleteMission();
			}
		}
	}

	public float GetTimeLeft()
	{
		return missionDuration - missionElapsed;
	}

	public float GetDuration()
	{
		return missionDuration;
	}

	public void CompleteMission()
	{
		SpacecraftManager.instance.PushReadyToLandNotification(this);
		SetState(MissionState.WaitingToLand);
		Land();
	}

	private void Land()
	{
		launchConditions.Trigger(-1165815793, SpacecraftManager.instance.GetSpacecraftDestination(id));
		foreach (GameObject item in AttachableBuilding.GetAttachedNetwork(launchConditions.GetComponent<AttachableBuilding>()))
		{
			if (item != launchConditions.gameObject)
			{
				item.Trigger(-1165815793, SpacecraftManager.instance.GetSpacecraftDestination(id));
			}
		}
	}

	public void TemporallyTear()
	{
		SpacecraftManager.instance.hasVisitedWormHole = true;
		LaunchConditionManager launchConditions = this.launchConditions;
		for (int num = launchConditions.rocketModules.Count - 1; num >= 0; num--)
		{
			Storage component = launchConditions.rocketModules[num].GetComponent<Storage>();
			if (component != null)
			{
				component.ConsumeAllIgnoringDisease();
			}
			MinionStorage component2 = launchConditions.rocketModules[num].GetComponent<MinionStorage>();
			if (component2 != null)
			{
				List<MinionStorage.Info> storedMinionInfo = component2.GetStoredMinionInfo();
				for (int num2 = storedMinionInfo.Count - 1; num2 >= 0; num2--)
				{
					component2.DeleteStoredMinion(storedMinionInfo[num2].id);
				}
			}
			Util.KDestroyGameObject(launchConditions.rocketModules[num].gameObject);
		}
	}

	public void GenerateName()
	{
		SetRocketName(GameUtil.GenerateRandomRocketName());
	}
}