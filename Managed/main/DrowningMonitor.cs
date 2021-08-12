using System;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/DrowningMonitor")]
public class DrowningMonitor : KMonoBehaviour, IWiltCause, ISlicedSim1000ms
{
	[MyCmpReq]
	private KSelectable selectable;

	[MyCmpGet]
	private Effects effects;

	private OccupyArea _occupyArea;

	[Serialize]
	[SerializeField]
	private float timeToDrown;

	[Serialize]
	private bool drowned;

	private bool drowning;

	protected const float MaxDrownTime = 75f;

	protected const float RegenRate = 5f;

	protected const float CellLiquidThreshold = 0.95f;

	public bool canDrownToDeath = true;

	public bool livesUnderWater;

	private Guid drowningStatusGuid;

	private Guid saturatedStatusGuid;

	private Extents extents;

	private HandleVector<int>.Handle partitionerEntry;

	public static Effect drowningEffect;

	public static Effect saturatedEffect;

	private static readonly Func<int, object, bool> CellSafeTestDelegate = (int testCell, object data) => CellSafeTest(testCell, data);

	private OccupyArea occupyArea
	{
		get
		{
			if (_occupyArea == null)
			{
				_occupyArea = GetComponent<OccupyArea>();
			}
			return _occupyArea;
		}
	}

	public bool Drowning => drowning;

	WiltCondition.Condition[] IWiltCause.Conditions => new WiltCondition.Condition[1] { WiltCondition.Condition.Drowning };

	public string WiltStateString
	{
		get
		{
			if (livesUnderWater)
			{
				return "    • " + CREATURES.STATUSITEMS.SATURATED.NAME;
			}
			return "    • " + CREATURES.STATUSITEMS.DROWNING.NAME;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		timeToDrown = 75f;
		if (drowningEffect == null)
		{
			drowningEffect = new Effect("Drowning", CREATURES.STATUSITEMS.DROWNING.NAME, CREATURES.STATUSITEMS.DROWNING.TOOLTIP, 0f, show_in_ui: false, trigger_floating_text: false, is_bad: true);
			drowningEffect.Add(new AttributeModifier(Db.Get().CritterAttributes.Happiness.Id, -100f, CREATURES.STATUSITEMS.DROWNING.NAME));
		}
		if (saturatedEffect == null)
		{
			saturatedEffect = new Effect("Saturated", CREATURES.STATUSITEMS.SATURATED.NAME, CREATURES.STATUSITEMS.SATURATED.TOOLTIP, 0f, show_in_ui: false, trigger_floating_text: false, is_bad: true);
			saturatedEffect.Add(new AttributeModifier(Db.Get().CritterAttributes.Happiness.Id, -100f, CREATURES.STATUSITEMS.SATURATED.NAME));
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		SlicedUpdaterSim1000ms<DrowningMonitor>.instance.RegisterUpdate1000ms(this);
		OnMove();
		CheckDrowning();
		Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(base.transform, OnMove, "DrowningMonitor.OnSpawn");
	}

	private void OnMove()
	{
		if (partitionerEntry.IsValid())
		{
			Extents extents = occupyArea.GetExtents();
			GameScenePartitioner.Instance.UpdatePosition(partitionerEntry, extents.x, extents.y);
		}
		else
		{
			partitionerEntry = GameScenePartitioner.Instance.Add("DrowningMonitor.OnSpawn", base.gameObject, occupyArea.GetExtents(), GameScenePartitioner.Instance.liquidChangedLayer, OnLiquidChanged);
		}
		CheckDrowning();
	}

	protected override void OnCleanUp()
	{
		Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(base.transform, OnMove);
		GameScenePartitioner.Instance.Free(ref partitionerEntry);
		SlicedUpdaterSim1000ms<DrowningMonitor>.instance.UnregisterUpdate1000ms(this);
		base.OnCleanUp();
	}

	private void CheckDrowning(object data = null)
	{
		if (drowned)
		{
			return;
		}
		int cell = Grid.PosToCell(base.gameObject.transform.GetPosition());
		if (!IsCellSafe(cell))
		{
			if (!drowning)
			{
				drowning = true;
				GetComponent<KPrefabID>().AddTag(GameTags.Creatures.Drowning);
				Trigger(1949704522);
			}
			if (timeToDrown <= 0f && canDrownToDeath)
			{
				this.GetSMI<DeathMonitor.Instance>()?.Kill(Db.Get().Deaths.Drowned);
				Trigger(-750750377);
				drowned = true;
			}
		}
		else if (drowning)
		{
			drowning = false;
			GetComponent<KPrefabID>().RemoveTag(GameTags.Creatures.Drowning);
			Trigger(99949694);
		}
		if (livesUnderWater)
		{
			saturatedStatusGuid = selectable.ToggleStatusItem(Db.Get().CreatureStatusItems.Saturated, saturatedStatusGuid, drowning, this);
		}
		else
		{
			drowningStatusGuid = selectable.ToggleStatusItem(Db.Get().CreatureStatusItems.Drowning, drowningStatusGuid, drowning, this);
		}
		if (!(effects != null))
		{
			return;
		}
		if (drowning)
		{
			if (livesUnderWater)
			{
				effects.Add(saturatedEffect, should_save: false);
			}
			else
			{
				effects.Add(drowningEffect, should_save: false);
			}
		}
		else if (livesUnderWater)
		{
			effects.Remove(saturatedEffect);
		}
		else
		{
			effects.Remove(drowningEffect);
		}
	}

	private static bool CellSafeTest(int testCell, object data)
	{
		int num = Grid.CellAbove(testCell);
		if (!Grid.IsValidCell(testCell) || !Grid.IsValidCell(num))
		{
			return false;
		}
		if (Grid.IsSubstantialLiquid(testCell, 0.95f))
		{
			return false;
		}
		if (Grid.IsLiquid(testCell))
		{
			if (Grid.Element[num].IsLiquid)
			{
				return false;
			}
			if (Grid.Element[num].IsSolid)
			{
				return false;
			}
		}
		return true;
	}

	public bool IsCellSafe(int cell)
	{
		return occupyArea.TestArea(cell, this, CellSafeTestDelegate);
	}

	private void OnLiquidChanged(object data)
	{
		CheckDrowning();
	}

	public void SlicedSim1000ms(float dt)
	{
		CheckDrowning();
		if (drowning)
		{
			if (!drowned)
			{
				timeToDrown -= dt;
				if (timeToDrown <= 0f)
				{
					CheckDrowning();
				}
			}
		}
		else
		{
			timeToDrown += dt * 5f;
			timeToDrown = Mathf.Clamp(timeToDrown, 0f, 75f);
		}
	}
}
