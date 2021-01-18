using System.Collections.Generic;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SubmersionMonitor")]
public class SubmersionMonitor : KMonoBehaviour, IGameObjectEffectDescriptor, IWiltCause, ISim1000ms
{
	private int position;

	private bool dry = false;

	protected float cellLiquidThreshold = 0.2f;

	private Extents extents;

	private HandleVector<int>.Handle partitionerEntry;

	public bool Dry => dry;

	WiltCondition.Condition[] IWiltCause.Conditions => new WiltCondition.Condition[1]
	{
		WiltCondition.Condition.DryingOut
	};

	public string WiltStateString
	{
		get
		{
			if (Dry)
			{
				return Db.Get().CreatureStatusItems.DryingOut.resolveStringCallback(CREATURES.STATUSITEMS.DRYINGOUT.NAME, this);
			}
			return "";
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		OnMove();
		CheckDry();
		Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(base.transform, OnMove, "SubmersionMonitor.OnSpawn");
	}

	private void OnMove()
	{
		position = Grid.PosToCell(base.gameObject);
		if (partitionerEntry.IsValid())
		{
			GameScenePartitioner.Instance.UpdatePosition(partitionerEntry, position);
		}
		else
		{
			Vector2I vector2I = Grid.PosToXY(base.transform.GetPosition());
			Extents extents = new Extents(vector2I.x, vector2I.y, 1, 2);
			partitionerEntry = GameScenePartitioner.Instance.Add("DrowningMonitor.OnSpawn", base.gameObject, extents, GameScenePartitioner.Instance.liquidChangedLayer, OnLiquidChanged);
		}
		CheckDry();
	}

	private void OnDrawGizmosSelected()
	{
	}

	protected override void OnCleanUp()
	{
		Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(base.transform, OnMove);
		GameScenePartitioner.Instance.Free(ref partitionerEntry);
		base.OnCleanUp();
	}

	public void Configure(float _maxStamina, float _staminaRegenRate, float _cellLiquidThreshold = 0.95f)
	{
		cellLiquidThreshold = _cellLiquidThreshold;
	}

	public void Sim1000ms(float dt)
	{
		CheckDry();
	}

	private void CheckDry()
	{
		bool flag = true;
		if (!IsCellSafe())
		{
			if (!dry)
			{
				dry = true;
				Trigger(-2057657673);
			}
		}
		else if (dry)
		{
			dry = false;
			Trigger(1555379996);
		}
	}

	public bool IsCellSafe()
	{
		int cell = Grid.PosToCell(base.gameObject);
		if (!Grid.IsValidCell(cell))
		{
			return false;
		}
		if (Grid.IsSubstantialLiquid(cell, cellLiquidThreshold))
		{
			return true;
		}
		return false;
	}

	private void OnLiquidChanged(object data)
	{
		CheckDry();
	}

	public void SetIncapacitated(bool state)
	{
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		return new List<Descriptor>
		{
			new Descriptor(UI.GAMEOBJECTEFFECTS.REQUIRES_SUBMERSION, UI.GAMEOBJECTEFFECTS.TOOLTIPS.REQUIRES_SUBMERSION, Descriptor.DescriptorType.Requirement)
		};
	}
}
