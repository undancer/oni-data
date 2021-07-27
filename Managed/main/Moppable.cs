using System;
using STRINGS;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/Moppable")]
public class Moppable : Workable, ISim1000ms, ISim200ms
{
	[MyCmpReq]
	private KSelectable Selectable;

	[MyCmpAdd]
	private Prioritizable prioritizable;

	public float amountMoppedPerTick = 1000f;

	private HandleVector<int>.Handle partitionerEntry;

	private SchedulerHandle destroyHandle;

	private float amountMopped;

	private MeshRenderer childRenderer;

	private CellOffset[] offsets = new CellOffset[3]
	{
		new CellOffset(0, 0),
		new CellOffset(1, 0),
		new CellOffset(-1, 0)
	};

	private static readonly EventSystem.IntraObjectHandler<Moppable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Moppable>(delegate(Moppable component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Moppable> OnReachableChangedDelegate = new EventSystem.IntraObjectHandler<Moppable>(delegate(Moppable component, object data)
	{
		component.OnReachableChanged(data);
	});

	private Moppable()
	{
		showProgressBar = false;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		workerStatusItem = Db.Get().DuplicantStatusItems.Mopping;
		attributeConverter = Db.Get().AttributeConverters.TidyingSpeed;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		skillExperienceSkillGroup = Db.Get().SkillGroups.Basekeeping.Id;
		skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
		childRenderer = GetComponentInChildren<MeshRenderer>();
		Prioritizable.AddRef(base.gameObject);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (!IsThereLiquid())
		{
			base.gameObject.DeleteObject();
			return;
		}
		Grid.Objects[Grid.PosToCell(base.gameObject), 8] = base.gameObject;
		new WorkChore<Moppable>(Db.Get().ChoreTypes.Mop, this);
		SetWorkTime(float.PositiveInfinity);
		GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().MiscStatusItems.WaitingForMop);
		Subscribe(493375141, OnRefreshUserMenuDelegate);
		overrideAnims = new KAnimFile[1] { Assets.GetAnim("anim_mop_dirtywater_kanim") };
		partitionerEntry = GameScenePartitioner.Instance.Add("Moppable.OnSpawn", base.gameObject, new Extents(Grid.PosToCell(this), new CellOffset[1]
		{
			new CellOffset(0, 0)
		}), GameScenePartitioner.Instance.liquidChangedLayer, OnLiquidChanged);
		Refresh();
		Subscribe(-1432940121, OnReachableChangedDelegate);
		new ReachabilityMonitor.Instance(this).StartSM();
		SimAndRenderScheduler.instance.Remove(this);
	}

	private void OnRefreshUserMenu(object data)
	{
		Game.Instance.userMenu.AddButton(base.gameObject, new KIconButtonMenu.ButtonInfo("icon_cancel", UI.USERMENUACTIONS.CANCELMOP.NAME, OnCancel, Action.NumActions, null, null, null, UI.USERMENUACTIONS.CANCELMOP.TOOLTIP));
	}

	private void OnCancel()
	{
		DetailsScreen.Instance.Show(show: false);
		base.gameObject.Trigger(2127324410);
	}

	protected override void OnStartWork(Worker worker)
	{
		SimAndRenderScheduler.instance.Add(this);
		Refresh();
		MopTick(amountMoppedPerTick);
	}

	protected override void OnStopWork(Worker worker)
	{
		SimAndRenderScheduler.instance.Remove(this);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		SimAndRenderScheduler.instance.Remove(this);
	}

	public override bool InstantlyFinish(Worker worker)
	{
		MopTick(1000f);
		return true;
	}

	public void Sim1000ms(float dt)
	{
		if (amountMopped > 0f)
		{
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, GameUtil.GetFormattedMass(0f - amountMopped), base.transform);
			amountMopped = 0f;
		}
	}

	public void Sim200ms(float dt)
	{
		if (base.worker != null)
		{
			Refresh();
			MopTick(amountMoppedPerTick);
		}
	}

	private void OnCellMopped(Sim.MassConsumedCallback mass_cb_info, object data)
	{
		if (!(this == null) && mass_cb_info.mass > 0f)
		{
			amountMopped += mass_cb_info.mass;
			int cell = Grid.PosToCell(this);
			SubstanceChunk substanceChunk = LiquidSourceManager.Instance.CreateChunk(ElementLoader.elements[mass_cb_info.elemIdx], mass_cb_info.mass, mass_cb_info.temperature, mass_cb_info.diseaseIdx, mass_cb_info.diseaseCount, Grid.CellToPosCCC(cell, Grid.SceneLayer.Ore));
			substanceChunk.transform.SetPosition(substanceChunk.transform.GetPosition() + new Vector3((UnityEngine.Random.value - 0.5f) * 0.5f, 0f, 0f));
		}
	}

	public static void MopCell(int cell, float amount, Action<Sim.MassConsumedCallback, object> cb)
	{
		if (Grid.Element[cell].IsLiquid)
		{
			int callbackIdx = -1;
			if (cb != null)
			{
				callbackIdx = Game.Instance.massConsumedCallbackManager.Add(cb, null, "Moppable").index;
			}
			SimMessages.ConsumeMass(cell, Grid.Element[cell].id, amount, 1, callbackIdx);
		}
	}

	private void MopTick(float mopAmount)
	{
		int cell = Grid.PosToCell(this);
		for (int i = 0; i < offsets.Length; i++)
		{
			int num = Grid.OffsetCell(cell, offsets[i]);
			if (Grid.Element[num].IsLiquid)
			{
				MopCell(num, mopAmount, OnCellMopped);
			}
		}
	}

	private bool IsThereLiquid()
	{
		int cell = Grid.PosToCell(this);
		bool result = false;
		for (int i = 0; i < offsets.Length; i++)
		{
			int num = Grid.OffsetCell(cell, offsets[i]);
			if (Grid.Element[num].IsLiquid && Grid.Mass[num] <= MopTool.maxMopAmt)
			{
				result = true;
			}
		}
		return result;
	}

	private void Refresh()
	{
		if (!IsThereLiquid())
		{
			if (!destroyHandle.IsValid)
			{
				destroyHandle = GameScheduler.Instance.Schedule("DestroyMoppable", 1f, delegate
				{
					TryDestroy();
				}, this);
			}
		}
		else if (destroyHandle.IsValid)
		{
			destroyHandle.ClearScheduler();
		}
	}

	private void OnLiquidChanged(object data)
	{
		Refresh();
	}

	private void TryDestroy()
	{
		if (this != null)
		{
			base.gameObject.DeleteObject();
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		GameScenePartitioner.Instance.Free(ref partitionerEntry);
	}

	private void OnReachableChanged(object data)
	{
		if (!(childRenderer != null))
		{
			return;
		}
		Material material = childRenderer.material;
		bool flag = (bool)data;
		if (material.color == Game.Instance.uiColours.Dig.invalidLocation)
		{
			return;
		}
		KSelectable component = GetComponent<KSelectable>();
		if (flag)
		{
			material.color = Game.Instance.uiColours.Dig.validLocation;
			component.RemoveStatusItem(Db.Get().BuildingStatusItems.MopUnreachable);
			return;
		}
		component.AddStatusItem(Db.Get().BuildingStatusItems.MopUnreachable, this);
		GameScheduler.Instance.Schedule("Locomotion Tutorial", 2f, delegate
		{
			Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Locomotion);
		});
		material.color = Game.Instance.uiColours.Dig.unreachable;
	}
}
