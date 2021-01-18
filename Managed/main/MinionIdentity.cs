using System;
using System.Collections.Generic;
using Klei.AI;
using Klei.CustomSettings;
using KSerialization;
using STRINGS;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/MinionIdentity")]
public class MinionIdentity : KMonoBehaviour, ISaveLoadable, IAssignableIdentity, IListableOption, ISim1000ms
{
	private class NameList
	{
		private List<string> names = new List<string>();

		private int idx;

		public NameList(TextAsset file)
		{
			string[] array = file.text.Replace("  ", " ").Replace("\r\n", "\n").Split('\n');
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(' ');
				if (array2[array2.Length - 1] != "" && array2[array2.Length - 1] != null)
				{
					names.Add(array2[array2.Length - 1]);
				}
			}
			names.Shuffle();
		}

		public string Next()
		{
			return names[idx++ % names.Count];
		}
	}

	[MyCmpReq]
	private KSelectable selectable;

	public int femaleVoiceCount;

	public int maleVoiceCount;

	[Serialize]
	private new string name;

	[Serialize]
	public string gender;

	[Serialize]
	public string stickerType;

	[Serialize]
	[ReadOnly]
	public float arrivalTime;

	[Serialize]
	public int voiceIdx;

	[Serialize]
	public KCompBuilder.BodyData bodyData;

	[Serialize]
	public Ref<MinionAssignablesProxy> assignableProxy;

	private Navigator navigator;

	private ChoreDriver choreDriver;

	public float timeLastSpoke;

	private string voiceId;

	private KAnimHashedString overrideExpression;

	private KAnimHashedString expression;

	public bool addToIdentityList = true;

	private static NameList maleNameList;

	private static NameList femaleNameList;

	private static readonly EventSystem.IntraObjectHandler<MinionIdentity> OnDeadTagAddedDelegate = GameUtil.CreateHasTagHandler(GameTags.Dead, delegate(MinionIdentity component, object data)
	{
		component.OnDied(data);
	});

	[Serialize]
	public string genderStringKey
	{
		get;
		set;
	}

	[Serialize]
	public string nameStringKey
	{
		get;
		set;
	}

	public static void DestroyStatics()
	{
		maleNameList = null;
		femaleNameList = null;
	}

	protected override void OnPrefabInit()
	{
		if (name == null)
		{
			name = ChooseRandomName();
		}
		if (GameClock.Instance != null)
		{
			arrivalTime = GameClock.Instance.GetCycle();
		}
		KAnimControllerBase component = GetComponent<KAnimControllerBase>();
		if (component != null)
		{
			component.OnUpdateBounds = (Action<Bounds>)Delegate.Combine(component.OnUpdateBounds, new Action<Bounds>(OnUpdateBounds));
		}
		GameUtil.SubscribeToTags(this, OnDeadTagAddedDelegate, triggerImmediately: true);
	}

	protected override void OnSpawn()
	{
		if (addToIdentityList)
		{
			ValidateProxy();
			CleanupLimboMinions();
		}
		PathProber component = GetComponent<PathProber>();
		if (component != null)
		{
			component.SetGroupProber(MinionGroupProber.Get());
		}
		SetName(name);
		if (nameStringKey == null)
		{
			nameStringKey = name;
		}
		SetGender(gender);
		if (genderStringKey == null)
		{
			genderStringKey = "NB";
		}
		if (addToIdentityList)
		{
			Components.MinionIdentities.Add(this);
			if (!base.gameObject.HasTag(GameTags.Dead))
			{
				Components.LiveMinionIdentities.Add(this);
			}
		}
		SymbolOverrideController component2 = GetComponent<SymbolOverrideController>();
		if (component2 != null)
		{
			Accessorizer component3 = base.gameObject.GetComponent<Accessorizer>();
			if (component3 != null)
			{
				bodyData = default(KCompBuilder.BodyData);
				component3.GetBodySlots(ref bodyData);
				string text = HashCache.Get().Get(component3.GetAccessory(Db.Get().AccessorySlots.HeadShape).symbol.hash);
				string str = text.Replace("headshape", "cheek");
				component2.AddSymbolOverride("snapto_cheek", Assets.GetAnim("head_swap_kanim").GetData().build.GetSymbol(str), 1);
				component2.AddSymbolOverride(Db.Get().AccessorySlots.HairAlways.targetSymbolId, component3.GetAccessory(Db.Get().AccessorySlots.Hair).symbol, 1);
				component2.AddSymbolOverride(Db.Get().AccessorySlots.HatHair.targetSymbolId, Db.Get().AccessorySlots.HatHair.Lookup("hat_" + HashCache.Get().Get(component3.GetAccessory(Db.Get().AccessorySlots.Hair).symbol.hash)).symbol, 1);
			}
		}
		voiceId = "0";
		voiceId += voiceIdx + 1;
		Prioritizable component4 = GetComponent<Prioritizable>();
		if (component4 != null)
		{
			component4.showIcon = false;
		}
		Pickupable component5 = GetComponent<Pickupable>();
		if (component5 != null)
		{
			component5.carryAnimOverride = Assets.GetAnim("anim_incapacitated_carrier_kanim");
		}
		ApplyCustomGameSettings();
	}

	public void ValidateProxy()
	{
		assignableProxy = MinionAssignablesProxy.InitAssignableProxy(assignableProxy, this);
	}

	private void CleanupLimboMinions()
	{
		KPrefabID component = GetComponent<KPrefabID>();
		if (component.InstanceID == -1)
		{
			DebugUtil.LogWarningArgs("Minion with an invalid kpid! Attempting to recover...", name);
			if (KPrefabIDTracker.Get().GetInstance(component.InstanceID) != null)
			{
				KPrefabIDTracker.Get().Unregister(component);
			}
			component.InstanceID = KPrefabID.GetUniqueID();
			KPrefabIDTracker.Get().Register(component);
			DebugUtil.LogWarningArgs("Restored as:", component.InstanceID);
		}
		if (component.conflicted)
		{
			DebugUtil.LogWarningArgs("Minion with a conflicted kpid! Attempting to recover... ", component.InstanceID, name);
			if (KPrefabIDTracker.Get().GetInstance(component.InstanceID) != null)
			{
				KPrefabIDTracker.Get().Unregister(component);
			}
			component.InstanceID = KPrefabID.GetUniqueID();
			KPrefabIDTracker.Get().Register(component);
			DebugUtil.LogWarningArgs("Restored as:", component.InstanceID);
		}
		assignableProxy.Get().SetTarget(this, base.gameObject);
	}

	public string GetProperName()
	{
		return base.gameObject.GetProperName();
	}

	public string GetVoiceId()
	{
		return voiceId;
	}

	public void SetName(string name)
	{
		this.name = name;
		if (selectable != null)
		{
			selectable.SetName(name);
		}
		base.gameObject.name = name;
		NameDisplayScreen.Instance.UpdateName(base.gameObject);
	}

	public void SetStickerType(string stickerType)
	{
		this.stickerType = stickerType;
	}

	public bool IsNull()
	{
		return this == null;
	}

	public void SetGender(string gender)
	{
		this.gender = gender;
		selectable.SetGender(gender);
	}

	public static string ChooseRandomName()
	{
		if (femaleNameList == null)
		{
			maleNameList = new NameList(Game.Instance.maleNamesFile);
			femaleNameList = new NameList(Game.Instance.femaleNamesFile);
		}
		if (UnityEngine.Random.value > 0.5f)
		{
			return maleNameList.Next();
		}
		return femaleNameList.Next();
	}

	protected override void OnCleanUp()
	{
		if (assignableProxy != null)
		{
			MinionAssignablesProxy minionAssignablesProxy = assignableProxy.Get();
			if ((bool)minionAssignablesProxy && minionAssignablesProxy.target == this)
			{
				Util.KDestroyGameObject(minionAssignablesProxy.gameObject);
			}
		}
		Components.MinionIdentities.Remove(this);
		Components.LiveMinionIdentities.Remove(this);
	}

	private void OnUpdateBounds(Bounds bounds)
	{
		KBoxCollider2D component = GetComponent<KBoxCollider2D>();
		component.offset = bounds.center;
		component.size = bounds.extents;
	}

	private void OnDied(object data)
	{
		GetSoleOwner().UnassignAll();
		GetEquipment().UnequipAll();
		Components.LiveMinionIdentities.Remove(this);
	}

	public List<Ownables> GetOwners()
	{
		return assignableProxy.Get().ownables;
	}

	public Ownables GetSoleOwner()
	{
		return assignableProxy.Get().GetComponent<Ownables>();
	}

	public bool HasOwner(Assignables owner)
	{
		return GetOwners().Contains(owner as Ownables);
	}

	public int NumOwners()
	{
		return GetOwners().Count;
	}

	public Equipment GetEquipment()
	{
		return assignableProxy.Get().GetComponent<Equipment>();
	}

	public void Sim1000ms(float dt)
	{
		if (this == null)
		{
			return;
		}
		if (navigator == null)
		{
			navigator = GetComponent<Navigator>();
		}
		if (navigator != null && !navigator.IsMoving())
		{
			return;
		}
		if (choreDriver == null)
		{
			choreDriver = GetComponent<ChoreDriver>();
		}
		if (!(choreDriver != null))
		{
			return;
		}
		Chore currentChore = choreDriver.GetCurrentChore();
		if (currentChore != null && currentChore is FetchAreaChore)
		{
			MinionResume component = GetComponent<MinionResume>();
			if (component != null)
			{
				component.AddExperienceWithAptitude(Db.Get().SkillGroups.Hauling.Id, dt, SKILLS.ALL_DAY_EXPERIENCE);
			}
		}
	}

	private void ApplyCustomGameSettings()
	{
		SettingLevel currentQualitySetting = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.ImmuneSystem);
		if (currentQualitySetting.id == "Compromised")
		{
			Db.Get().Attributes.DiseaseCureSpeed.Lookup(this).Add(new AttributeModifier(Db.Get().Attributes.DiseaseCureSpeed.Id, -0.3333f, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.COMPROMISED.ATTRIBUTE_MODIFIER_NAME));
			Db.Get().Attributes.GermResistance.Lookup(this).Add(new AttributeModifier(Db.Get().Attributes.GermResistance.Id, -2f, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.COMPROMISED.ATTRIBUTE_MODIFIER_NAME));
		}
		else if (currentQualitySetting.id == "Weak")
		{
			Db.Get().Attributes.GermResistance.Lookup(this).Add(new AttributeModifier(Db.Get().Attributes.GermResistance.Id, -1f, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.WEAK.ATTRIBUTE_MODIFIER_NAME));
		}
		else if (currentQualitySetting.id == "Strong")
		{
			Db.Get().Attributes.DiseaseCureSpeed.Lookup(this).Add(new AttributeModifier(Db.Get().Attributes.DiseaseCureSpeed.Id, 2f, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.STRONG.ATTRIBUTE_MODIFIER_NAME));
			Db.Get().Attributes.GermResistance.Lookup(this).Add(new AttributeModifier(Db.Get().Attributes.GermResistance.Id, 2f, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.STRONG.ATTRIBUTE_MODIFIER_NAME));
		}
		else if (currentQualitySetting.id == "Invincible")
		{
			Db.Get().Attributes.DiseaseCureSpeed.Lookup(this).Add(new AttributeModifier(Db.Get().Attributes.DiseaseCureSpeed.Id, 100000000f, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.INVINCIBLE.ATTRIBUTE_MODIFIER_NAME));
			Db.Get().Attributes.GermResistance.Lookup(this).Add(new AttributeModifier(Db.Get().Attributes.GermResistance.Id, 200f, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.INVINCIBLE.ATTRIBUTE_MODIFIER_NAME));
		}
		SettingLevel currentQualitySetting2 = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.Stress);
		if (currentQualitySetting2.id == "Doomed")
		{
			Db.Get().Amounts.Stress.deltaAttribute.Lookup(this).Add(new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, 71f / (678f * (float)Math.PI), UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.LEVELS.DOOMED.ATTRIBUTE_MODIFIER_NAME));
		}
		else if (currentQualitySetting2.id == "Pessimistic")
		{
			Db.Get().Amounts.Stress.deltaAttribute.Lookup(this).Add(new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, 0.016666668f, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.LEVELS.PESSIMISTIC.ATTRIBUTE_MODIFIER_NAME));
		}
		else if (currentQualitySetting2.id == "Optimistic")
		{
			Db.Get().Amounts.Stress.deltaAttribute.Lookup(this).Add(new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, -0.016666668f, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.LEVELS.OPTIMISTIC.ATTRIBUTE_MODIFIER_NAME));
		}
		else if (currentQualitySetting2.id == "Indomitable")
		{
			Db.Get().Amounts.Stress.deltaAttribute.Lookup(this).Add(new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, float.NegativeInfinity, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.LEVELS.INDOMITABLE.ATTRIBUTE_MODIFIER_NAME));
		}
		SettingLevel currentQualitySetting3 = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.CalorieBurn);
		if (currentQualitySetting3.id == "VeryHard")
		{
			Db.Get().Amounts.Calories.deltaAttribute.Lookup(this).Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -1666.6666f, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.CALORIE_BURN.LEVELS.VERYHARD.ATTRIBUTE_MODIFIER_NAME));
		}
		else if (currentQualitySetting3.id == "Hard")
		{
			Db.Get().Amounts.Calories.deltaAttribute.Lookup(this).Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -833.3333f, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.CALORIE_BURN.LEVELS.HARD.ATTRIBUTE_MODIFIER_NAME));
		}
		else if (currentQualitySetting3.id == "Easy")
		{
			Db.Get().Amounts.Calories.deltaAttribute.Lookup(this).Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, 833.3333f, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.CALORIE_BURN.LEVELS.EASY.ATTRIBUTE_MODIFIER_NAME));
		}
		else if (currentQualitySetting3.id == "Disabled")
		{
			Db.Get().Amounts.Calories.deltaAttribute.Lookup(this).Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, float.PositiveInfinity, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.CALORIE_BURN.LEVELS.DISABLED.ATTRIBUTE_MODIFIER_NAME));
		}
	}
}
