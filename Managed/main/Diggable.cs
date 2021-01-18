using System.Collections;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/Workable/Diggable")]
public class Diggable : Workable
{
	private HandleVector<int>.Handle partitionerEntry;

	private HandleVector<int>.Handle unstableEntry;

	private MeshRenderer childRenderer;

	private bool isReachable;

	private Element originalDigElement;

	[MyCmpAdd]
	private Prioritizable prioritizable;

	[SerializeField]
	public HashedString choreTypeIdHash;

	[SerializeField]
	public Material[] materials;

	[SerializeField]
	public MeshRenderer materialDisplay;

	private bool isDigComplete;

	private static List<Tuple<string, Tag>> lasersForHardness = new List<Tuple<string, Tag>>
	{
		new Tuple<string, Tag>("dig", "fx_dig_splash"),
		new Tuple<string, Tag>("specialistdig", "fx_dig_splash")
	};

	private int handle;

	private static readonly EventSystem.IntraObjectHandler<Diggable> OnReachableChangedDelegate = new EventSystem.IntraObjectHandler<Diggable>(delegate(Diggable component, object data)
	{
		component.OnReachableChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Diggable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Diggable>(delegate(Diggable component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	public Chore chore;

	public bool Reachable => isReachable;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		workerStatusItem = Db.Get().DuplicantStatusItems.Digging;
		readyForSkillWorkStatusItem = Db.Get().BuildingStatusItems.DigRequiresSkillPerk;
		faceTargetWhenWorking = true;
		Subscribe(-1432940121, OnReachableChangedDelegate);
		attributeConverter = Db.Get().AttributeConverters.DiggingSpeed;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
		skillExperienceSkillGroup = Db.Get().SkillGroups.Mining.Id;
		skillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
		multitoolContext = "dig";
		multitoolHitEffectTag = "fx_dig_splash";
		workingPstComplete = null;
		workingPstFailed = null;
		Prioritizable.AddRef(base.gameObject);
	}

	private Diggable()
	{
		SetOffsetTable(OffsetGroups.InvertedStandardTableWithCorners);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		int num = Grid.PosToCell(this);
		originalDigElement = Grid.Element[num];
		GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().MiscStatusItems.WaitingForDig);
		UpdateColor(isReachable);
		Grid.Objects[num, 7] = base.gameObject;
		ChoreType chore_type = Db.Get().ChoreTypes.Dig;
		if (choreTypeIdHash.IsValid)
		{
			chore_type = Db.Get().ChoreTypes.GetByHash(choreTypeIdHash);
		}
		chore = new WorkChore<Diggable>(chore_type, this, null, run_until_complete: true, null, null, null, allow_in_red_alert: true, null, ignore_schedule_block: false, only_when_operational: true, null, is_preemptable: true);
		SetWorkTime(float.PositiveInfinity);
		partitionerEntry = GameScenePartitioner.Instance.Add("Diggable.OnSpawn", base.gameObject, Grid.PosToCell(this), GameScenePartitioner.Instance.solidChangedLayer, OnSolidChanged);
		OnSolidChanged(null);
		new ReachabilityMonitor.Instance(this).StartSM();
		Subscribe(493375141, OnRefreshUserMenuDelegate);
		handle = Game.Instance.Subscribe(-1523247426, UpdateStatusItem);
		Components.Diggables.Add(this);
	}

	public override AnimInfo GetAnim(Worker worker)
	{
		AnimInfo result = default(AnimInfo);
		if (overrideAnims != null && overrideAnims.Length != 0)
		{
			result.overrideAnims = overrideAnims;
		}
		if (multitoolContext.IsValid && multitoolHitEffectTag.IsValid)
		{
			result.smi = new MultitoolController.Instance(this, worker, multitoolContext, Assets.GetPrefab(multitoolHitEffectTag));
		}
		return result;
	}

	private static bool IsCellBuildable(int cell)
	{
		bool result = false;
		GameObject gameObject = Grid.Objects[cell, 1];
		if (gameObject != null && gameObject.GetComponent<Constructable>() != null)
		{
			result = true;
		}
		return result;
	}

	private IEnumerator PeriodicUnstableFallingRecheck()
	{
		yield return new WaitForSeconds(2f);
		OnSolidChanged(null);
	}

	private void OnSolidChanged(object data)
	{
		if (this == null || base.gameObject == null)
		{
			return;
		}
		GameScenePartitioner.Instance.Free(ref unstableEntry);
		int num = Grid.PosToCell(this);
		int num2 = -1;
		UpdateColor(isReachable);
		if (Grid.Element[num].hardness >= 200)
		{
			bool flag = false;
			foreach (Chore.PreconditionInstance precondition in chore.GetPreconditions())
			{
				if (precondition.id == ChorePreconditions.instance.HasSkillPerk.id)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				chore.AddPrecondition(ChorePreconditions.instance.HasSkillPerk, Db.Get().SkillPerks.CanDigSupersuperhard);
			}
			requiredSkillPerk = Db.Get().SkillPerks.CanDigSupersuperhard.Id;
			materialDisplay.sharedMaterial = materials[3];
		}
		else if (Grid.Element[num].hardness >= 150)
		{
			bool flag2 = false;
			foreach (Chore.PreconditionInstance precondition2 in chore.GetPreconditions())
			{
				if (precondition2.id == ChorePreconditions.instance.HasSkillPerk.id)
				{
					flag2 = true;
					break;
				}
			}
			if (!flag2)
			{
				chore.AddPrecondition(ChorePreconditions.instance.HasSkillPerk, Db.Get().SkillPerks.CanDigNearlyImpenetrable);
			}
			requiredSkillPerk = Db.Get().SkillPerks.CanDigNearlyImpenetrable.Id;
			materialDisplay.sharedMaterial = materials[2];
		}
		else if (Grid.Element[num].hardness >= 50)
		{
			bool flag3 = false;
			foreach (Chore.PreconditionInstance precondition3 in chore.GetPreconditions())
			{
				if (precondition3.id == ChorePreconditions.instance.HasSkillPerk.id)
				{
					flag3 = true;
					break;
				}
			}
			if (!flag3)
			{
				chore.AddPrecondition(ChorePreconditions.instance.HasSkillPerk, Db.Get().SkillPerks.CanDigVeryFirm);
			}
			requiredSkillPerk = Db.Get().SkillPerks.CanDigVeryFirm.Id;
			materialDisplay.sharedMaterial = materials[1];
		}
		else
		{
			requiredSkillPerk = null;
			chore.GetPreconditions().Remove(chore.GetPreconditions().Find((Chore.PreconditionInstance o) => o.id == ChorePreconditions.instance.HasSkillPerk.id));
		}
		UpdateStatusItem();
		bool flag4 = false;
		if (!Grid.Solid[num])
		{
			num2 = GetUnstableCellAbove(num);
			if (num2 == -1)
			{
				flag4 = true;
			}
			else
			{
				StartCoroutine("PeriodicUnstableFallingRecheck");
			}
		}
		else if (Grid.Foundation[num])
		{
			flag4 = true;
		}
		if (flag4)
		{
			isDigComplete = true;
			if (chore == null || !chore.InProgress())
			{
				Util.KDestroyGameObject(base.gameObject);
			}
			else
			{
				GetComponentInChildren<MeshRenderer>().enabled = false;
			}
		}
		else if (num2 != -1)
		{
			Extents extents = default(Extents);
			Grid.CellToXY(num, out extents.x, out extents.y);
			extents.width = 1;
			extents.height = (num2 - num + Grid.WidthInCells - 1) / Grid.WidthInCells + 1;
			unstableEntry = GameScenePartitioner.Instance.Add("Diggable.OnSolidChanged", base.gameObject, extents, GameScenePartitioner.Instance.solidChangedLayer, OnSolidChanged);
		}
	}

	public Element GetTargetElement()
	{
		int num = Grid.PosToCell(base.transform.GetPosition());
		return Grid.Element[num];
	}

	public override string GetConversationTopic()
	{
		return originalDigElement.tag.Name;
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		DoDigTick(Grid.PosToCell(this), dt);
		return isDigComplete;
	}

	protected override void OnStopWork(Worker worker)
	{
		if (isDigComplete)
		{
			Util.KDestroyGameObject(base.gameObject);
		}
	}

	public override bool InstantlyFinish(Worker worker)
	{
		float approximateDigTime = GetApproximateDigTime(Grid.PosToCell(this));
		worker.Work(approximateDigTime);
		return true;
	}

	public static void DoDigTick(int cell, float dt)
	{
		float approximateDigTime = GetApproximateDigTime(cell);
		float amount = dt / approximateDigTime;
		WorldDamage.Instance.ApplyDamage(cell, amount, -1);
	}

	public static float GetApproximateDigTime(int cell)
	{
		float num = (int)Grid.Element[cell].hardness;
		if (num == 255f)
		{
			return float.MaxValue;
		}
		Element element = ElementLoader.FindElementByHash(SimHashes.Ice);
		float num2 = num / (float)(int)element.hardness;
		float num3 = Mathf.Min(Grid.Mass[cell], 400f) / 400f;
		float num4 = 4f * num3;
		return num4 + num2 * num4;
	}

	public static Diggable GetDiggable(int cell)
	{
		GameObject gameObject = Grid.Objects[cell, 7];
		if (gameObject != null)
		{
			return gameObject.GetComponent<Diggable>();
		}
		return null;
	}

	public static bool IsDiggable(int cell)
	{
		if (Grid.Solid[cell])
		{
			return !Grid.Foundation[cell];
		}
		return GetUnstableCellAbove(cell) != Grid.InvalidCell;
	}

	private static int GetUnstableCellAbove(int cell)
	{
		Vector2I cellXY = Grid.CellToXY(cell);
		List<int> cellsContainingFallingAbove = World.Instance.GetComponent<UnstableGroundManager>().GetCellsContainingFallingAbove(cellXY);
		if (cellsContainingFallingAbove.Contains(cell))
		{
			return cell;
		}
		int num = Grid.CellAbove(cell);
		while (Grid.IsValidCell(num))
		{
			if (Grid.Foundation[num])
			{
				return Grid.InvalidCell;
			}
			if (Grid.Solid[num])
			{
				if (Grid.Element[num].IsUnstable)
				{
					return num;
				}
				return Grid.InvalidCell;
			}
			if (cellsContainingFallingAbove.Contains(num))
			{
				return num;
			}
			num = Grid.CellAbove(num);
		}
		return Grid.InvalidCell;
	}

	public static bool RequiresTool(Element e)
	{
		return false;
	}

	public static bool Undiggable(Element e)
	{
		return e.id == SimHashes.Unobtanium;
	}

	private void OnReachableChanged(object data)
	{
		if (childRenderer == null)
		{
			childRenderer = GetComponentInChildren<MeshRenderer>();
		}
		Material material = childRenderer.material;
		isReachable = (bool)data;
		if (material.color == Game.Instance.uiColours.Dig.invalidLocation)
		{
			return;
		}
		UpdateColor(isReachable);
		KSelectable component = GetComponent<KSelectable>();
		if (isReachable)
		{
			component.RemoveStatusItem(Db.Get().BuildingStatusItems.DigUnreachable);
			return;
		}
		component.AddStatusItem(Db.Get().BuildingStatusItems.DigUnreachable, this);
		GameScheduler.Instance.Schedule("Locomotion Tutorial", 2f, delegate
		{
			Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Locomotion);
		});
	}

	private void UpdateColor(bool reachable)
	{
		if (!(childRenderer != null))
		{
			return;
		}
		Material material = childRenderer.material;
		if (RequiresTool(Grid.Element[Grid.PosToCell(base.gameObject)]) || Undiggable(Grid.Element[Grid.PosToCell(base.gameObject)]))
		{
			material.color = Game.Instance.uiColours.Dig.invalidLocation;
		}
		else if (Grid.Element[Grid.PosToCell(base.gameObject)].hardness >= 50)
		{
			if (reachable)
			{
				material.color = Game.Instance.uiColours.Dig.validLocation;
			}
			else
			{
				material.color = Game.Instance.uiColours.Dig.unreachable;
			}
			multitoolContext = lasersForHardness[1].first;
			multitoolHitEffectTag = lasersForHardness[1].second;
		}
		else
		{
			if (reachable)
			{
				material.color = Game.Instance.uiColours.Dig.validLocation;
			}
			else
			{
				material.color = Game.Instance.uiColours.Dig.unreachable;
			}
			multitoolContext = lasersForHardness[0].first;
			multitoolHitEffectTag = lasersForHardness[0].second;
		}
	}

	public override float GetPercentComplete()
	{
		return Grid.Damage[Grid.PosToCell(this)];
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		GameScenePartitioner.Instance.Free(ref partitionerEntry);
		GameScenePartitioner.Instance.Free(ref unstableEntry);
		Game.Instance.Unsubscribe(handle);
		int cell = Grid.PosToCell(this);
		GameScenePartitioner.Instance.TriggerEvent(cell, GameScenePartitioner.Instance.digDestroyedLayer, null);
		Components.Diggables.Remove(this);
	}

	private void OnCancel()
	{
		DetailsScreen.Instance.Show(show: false);
		base.gameObject.Trigger(2127324410);
	}

	private void OnRefreshUserMenu(object data)
	{
		Game.Instance.userMenu.AddButton(base.gameObject, new KIconButtonMenu.ButtonInfo("icon_cancel", UI.USERMENUACTIONS.CANCELDIG.NAME, OnCancel, Action.NumActions, null, null, null, UI.USERMENUACTIONS.CANCELDIG.TOOLTIP));
	}
}
