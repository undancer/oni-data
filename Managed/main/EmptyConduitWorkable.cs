using System;
using KSerialization;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/EmptyConduitWorkable")]
public class EmptyConduitWorkable : Workable
{
	[MyCmpReq]
	private Conduit conduit;

	private static StatusItem emptyLiquidConduitStatusItem;

	private static StatusItem emptyGasConduitStatusItem;

	private Chore chore;

	private const float RECHECK_PIPE_INTERVAL = 2f;

	private const float TIME_TO_EMPTY_PIPE = 4f;

	private const float NO_EMPTY_SCHEDULED = -1f;

	[Serialize]
	private float elapsedTime = -1f;

	private bool emptiedPipe = true;

	private static readonly EventSystem.IntraObjectHandler<EmptyConduitWorkable> OnEmptyConduitCancelledDelegate = new EventSystem.IntraObjectHandler<EmptyConduitWorkable>(delegate(EmptyConduitWorkable component, object data)
	{
		component.OnEmptyConduitCancelled(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		SetOffsetTable(OffsetGroups.InvertedStandardTable);
		SetWorkTime(float.PositiveInfinity);
		faceTargetWhenWorking = true;
		multitoolContext = "build";
		multitoolHitEffectTag = EffectConfigs.BuildSplashId;
		Subscribe(2127324410, OnEmptyConduitCancelledDelegate);
		if (emptyLiquidConduitStatusItem == null)
		{
			emptyLiquidConduitStatusItem = new StatusItem("EmptyLiquidConduit", BUILDINGS.PREFABS.CONDUIT.STATUS_ITEM.NAME, BUILDINGS.PREFABS.CONDUIT.STATUS_ITEM.TOOLTIP, "status_item_empty_pipe", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.LiquidConduits.ID, 66);
			emptyGasConduitStatusItem = new StatusItem("EmptyGasConduit", BUILDINGS.PREFABS.CONDUIT.STATUS_ITEM.NAME, BUILDINGS.PREFABS.CONDUIT.STATUS_ITEM.TOOLTIP, "status_item_empty_pipe", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.GasConduits.ID, 130);
		}
		requiredSkillPerk = Db.Get().SkillPerks.CanDoPlumbing.Id;
		shouldShowSkillPerkStatusItem = false;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (elapsedTime != -1f)
		{
			MarkForEmptying();
		}
	}

	public void MarkForEmptying()
	{
		if (chore == null)
		{
			StatusItem statusItem = GetStatusItem();
			GetComponent<KSelectable>().ToggleStatusItem(statusItem, on: true);
			CreateWorkChore();
		}
	}

	private void CancelEmptying()
	{
		CleanUpVisualization();
		if (chore != null)
		{
			chore.Cancel("Cancel");
			chore = null;
			shouldShowSkillPerkStatusItem = false;
			UpdateStatusItem();
		}
	}

	private void CleanUpVisualization()
	{
		StatusItem statusItem = GetStatusItem();
		KSelectable component = GetComponent<KSelectable>();
		if (component != null)
		{
			component.ToggleStatusItem(statusItem, on: false);
		}
		elapsedTime = -1f;
		if (chore != null)
		{
			GetComponent<Prioritizable>().RemoveRef();
		}
	}

	protected override void OnCleanUp()
	{
		CancelEmptying();
		base.OnCleanUp();
	}

	private ConduitFlow GetFlowManager()
	{
		if (conduit.type != ConduitType.Gas)
		{
			return Game.Instance.liquidConduitFlow;
		}
		return Game.Instance.gasConduitFlow;
	}

	private void OnEmptyConduitCancelled(object data)
	{
		CancelEmptying();
	}

	private StatusItem GetStatusItem()
	{
		return conduit.type switch
		{
			ConduitType.Gas => emptyGasConduitStatusItem, 
			ConduitType.Liquid => emptyLiquidConduitStatusItem, 
			_ => throw new ArgumentException(), 
		};
	}

	private void CreateWorkChore()
	{
		GetComponent<Prioritizable>().AddRef();
		chore = new WorkChore<EmptyConduitWorkable>(Db.Get().ChoreTypes.EmptyStorage, this, null, run_until_complete: true, null, null, null, allow_in_red_alert: true, null, ignore_schedule_block: false, only_when_operational: false);
		chore.AddPrecondition(ChorePreconditions.instance.HasSkillPerk, Db.Get().SkillPerks.CanDoPlumbing.Id);
		elapsedTime = 0f;
		emptiedPipe = false;
		shouldShowSkillPerkStatusItem = true;
		UpdateStatusItem();
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		if (elapsedTime == -1f)
		{
			return true;
		}
		bool result = false;
		elapsedTime += dt;
		if (!emptiedPipe)
		{
			if (elapsedTime > 4f)
			{
				EmptyPipeContents();
				emptiedPipe = true;
				elapsedTime = 0f;
			}
		}
		else if (elapsedTime > 2f)
		{
			int cell = Grid.PosToCell(base.transform.GetPosition());
			if (GetFlowManager().GetContents(cell).mass > 0f)
			{
				elapsedTime = 0f;
				emptiedPipe = false;
			}
			else
			{
				CleanUpVisualization();
				chore = null;
				result = true;
				shouldShowSkillPerkStatusItem = false;
				UpdateStatusItem();
			}
		}
		return result;
	}

	public override bool InstantlyFinish(Worker worker)
	{
		worker.Work(4f);
		return true;
	}

	public void EmptyPipeContents()
	{
		int cell = Grid.PosToCell(base.transform.GetPosition());
		ConduitFlow.ConduitContents conduitContents = GetFlowManager().RemoveElement(cell, float.PositiveInfinity);
		elapsedTime = 0f;
		if (conduitContents.mass > 0f && conduitContents.element != SimHashes.Vacuum)
		{
			(conduit.type switch
			{
				ConduitType.Gas => GasSourceManager.Instance, 
				ConduitType.Liquid => LiquidSourceManager.Instance, 
				_ => throw new ArgumentException(), 
			}).CreateChunk(conduitContents.element, conduitContents.mass, conduitContents.temperature, conduitContents.diseaseIdx, conduitContents.diseaseCount, Grid.CellToPosCCC(cell, Grid.SceneLayer.Ore));
		}
	}

	public override float GetPercentComplete()
	{
		return Mathf.Clamp01(elapsedTime / 4f);
	}
}
