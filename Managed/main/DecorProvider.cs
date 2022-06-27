using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/DecorProvider")]
public class DecorProvider : KMonoBehaviour, IGameObjectEffectDescriptor
{
	public const string ID = "DecorProvider";

	public float baseRadius;

	public float baseDecor;

	public string overrideName;

	public System.Action refreshCallback;

	public Action<object> refreshPartionerCallback;

	public Action<object> onCollectDecorProvidersCallback;

	public AttributeInstance decor;

	public AttributeInstance decorRadius;

	private AttributeModifier baseDecorModifier;

	private AttributeModifier baseDecorRadiusModifier;

	[MyCmpReq]
	public OccupyArea occupyArea;

	[MyCmpGet]
	public Rotatable rotatable;

	[MyCmpGet]
	public SimCellOccupier simCellOccupier;

	private int[] cells;

	private int cellCount;

	public float currDecor;

	private HandleVector<int>.Handle partitionerEntry;

	private HandleVector<int>.Handle solidChangedPartitionerEntry;

	private void AddDecor()
	{
		currDecor = 0f;
		if (decor != null)
		{
			currDecor = decor.GetTotalValue();
		}
		if (base.gameObject.HasTag(GameTags.Stored))
		{
			currDecor = 0f;
		}
		int num = Grid.PosToCell(base.gameObject);
		if (!Grid.IsValidCell(num))
		{
			return;
		}
		if (!Grid.Transparent[num] && Grid.Solid[num] && simCellOccupier == null)
		{
			currDecor = 0f;
		}
		if (currDecor == 0f)
		{
			return;
		}
		cellCount = 0;
		int num2 = 5;
		if (decorRadius != null)
		{
			num2 = (int)decorRadius.GetTotalValue();
		}
		Orientation orientation = Orientation.Neutral;
		if ((bool)rotatable)
		{
			orientation = rotatable.GetOrientation();
		}
		Extents extents = occupyArea.GetExtents(orientation);
		extents.x = Mathf.Max(extents.x - num2, 0);
		extents.y = Mathf.Max(extents.y - num2, 0);
		extents.width = Mathf.Min(extents.width + num2 * 2, Grid.WidthInCells - 1);
		extents.height = Mathf.Min(extents.height + num2 * 2, Grid.HeightInCells - 1);
		partitionerEntry = GameScenePartitioner.Instance.Add("DecorProvider.SplatCollectDecorProviders", base.gameObject, extents, GameScenePartitioner.Instance.decorProviderLayer, onCollectDecorProvidersCallback);
		solidChangedPartitionerEntry = GameScenePartitioner.Instance.Add("DecorProvider.SplatSolidCheck", base.gameObject, extents, GameScenePartitioner.Instance.solidChangedLayer, refreshPartionerCallback);
		int val = extents.x + extents.width;
		int val2 = extents.y + extents.height;
		int x = extents.x;
		int y = extents.y;
		Grid.CellToXY(num, out var x2, out var y2);
		val = Math.Min(val, Grid.WidthInCells);
		val2 = Math.Min(val2, Grid.HeightInCells);
		x = Math.Max(0, x);
		y = Math.Max(0, y);
		int num3 = (val - x) * (val2 - y);
		if (cells == null || cells.Length != num3)
		{
			cells = new int[num3];
		}
		for (int i = x; i < val; i++)
		{
			for (int j = y; j < val2; j++)
			{
				if (Grid.VisibilityTest(x2, y2, i, j))
				{
					int num4 = Grid.XYToCell(i, j);
					if (Grid.IsValidCell(num4))
					{
						Grid.Decor[num4] += currDecor;
						cells[cellCount++] = num4;
					}
				}
			}
		}
	}

	public void Clear()
	{
		if (currDecor != 0f)
		{
			RemoveDecor();
			GameScenePartitioner.Instance.Free(ref partitionerEntry);
			GameScenePartitioner.Instance.Free(ref solidChangedPartitionerEntry);
		}
	}

	private void RemoveDecor()
	{
		if (currDecor == 0f)
		{
			return;
		}
		for (int i = 0; i < cellCount; i++)
		{
			int num = cells[i];
			if (Grid.IsValidCell(num))
			{
				Grid.Decor[num] -= currDecor;
			}
		}
	}

	public void Refresh()
	{
		Clear();
		AddDecor();
		KPrefabID component = GetComponent<KPrefabID>();
		bool num = component.HasTag(RoomConstraints.ConstraintTags.Decor20);
		bool flag = decor.GetTotalValue() >= 20f;
		if (num != flag)
		{
			if (flag)
			{
				component.AddTag(RoomConstraints.ConstraintTags.Decor20);
			}
			else
			{
				component.RemoveTag(RoomConstraints.ConstraintTags.Decor20);
			}
			int cell = Grid.PosToCell(this);
			if (Grid.IsValidCell(cell))
			{
				Game.Instance.roomProber.SolidChangedEvent(cell, ignoreDoors: true);
			}
		}
	}

	public float GetDecorForCell(int cell)
	{
		for (int i = 0; i < cellCount; i++)
		{
			if (cells[i] == cell)
			{
				return currDecor;
			}
		}
		return 0f;
	}

	public void SetValues(EffectorValues values)
	{
		baseDecor = values.amount;
		baseRadius = values.radius;
		if (IsInitialized())
		{
			UpdateBaseDecorModifiers();
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		decor = this.GetAttributes().Add(Db.Get().BuildingAttributes.Decor);
		decorRadius = this.GetAttributes().Add(Db.Get().BuildingAttributes.DecorRadius);
		UpdateBaseDecorModifiers();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		refreshCallback = Refresh;
		refreshPartionerCallback = delegate
		{
			Refresh();
		};
		onCollectDecorProvidersCallback = OnCollectDecorProviders;
		Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(base.transform, OnCellChange, "DecorProvider.OnSpawn");
		AttributeInstance attributeInstance = decor;
		attributeInstance.OnDirty = (System.Action)Delegate.Combine(attributeInstance.OnDirty, refreshCallback);
		AttributeInstance attributeInstance2 = decorRadius;
		attributeInstance2.OnDirty = (System.Action)Delegate.Combine(attributeInstance2.OnDirty, refreshCallback);
		Refresh();
	}

	private void UpdateBaseDecorModifiers()
	{
		Attributes attributes = this.GetAttributes();
		if (baseDecorModifier != null)
		{
			attributes.Remove(baseDecorModifier);
			attributes.Remove(baseDecorRadiusModifier);
			baseDecorModifier = null;
			baseDecorRadiusModifier = null;
		}
		if (baseDecor != 0f)
		{
			baseDecorModifier = new AttributeModifier(Db.Get().BuildingAttributes.Decor.Id, baseDecor, UI.TOOLTIPS.BASE_VALUE);
			baseDecorRadiusModifier = new AttributeModifier(Db.Get().BuildingAttributes.DecorRadius.Id, baseRadius, UI.TOOLTIPS.BASE_VALUE);
			attributes.Add(baseDecorModifier);
			attributes.Add(baseDecorRadiusModifier);
		}
	}

	private void OnCellChange()
	{
		Refresh();
	}

	private void OnCollectDecorProviders(object data)
	{
		((List<DecorProvider>)data).Add(this);
	}

	public string GetName()
	{
		if (string.IsNullOrEmpty(overrideName))
		{
			return GetComponent<KSelectable>().GetName();
		}
		return overrideName;
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if (base.isSpawned)
		{
			AttributeInstance attributeInstance = decor;
			attributeInstance.OnDirty = (System.Action)Delegate.Remove(attributeInstance.OnDirty, refreshCallback);
			AttributeInstance attributeInstance2 = decorRadius;
			attributeInstance2.OnDirty = (System.Action)Delegate.Remove(attributeInstance2.OnDirty, refreshCallback);
			Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(base.transform, OnCellChange);
		}
		Clear();
	}

	public List<Descriptor> GetEffectDescriptions()
	{
		List<Descriptor> list = new List<Descriptor>();
		if (decor != null && decorRadius != null)
		{
			float totalValue = decor.GetTotalValue();
			float totalValue2 = decorRadius.GetTotalValue();
			string arg = ((baseDecor > 0f) ? "produced" : "consumed");
			string text = ((baseDecor > 0f) ? UI.BUILDINGEFFECTS.TOOLTIPS.DECORPROVIDED : UI.BUILDINGEFFECTS.TOOLTIPS.DECORDECREASED);
			text = text + "\n\n" + decor.GetAttributeValueTooltip();
			string text2 = GameUtil.AddPositiveSign(totalValue.ToString(), totalValue > 0f);
			Descriptor item = new Descriptor(string.Format(UI.BUILDINGEFFECTS.DECORPROVIDED, arg, text2, totalValue2), string.Format(text, text2, totalValue2));
			list.Add(item);
		}
		else if (baseDecor != 0f)
		{
			string arg2 = ((baseDecor >= 0f) ? "produced" : "consumed");
			string format = ((baseDecor >= 0f) ? UI.BUILDINGEFFECTS.TOOLTIPS.DECORPROVIDED : UI.BUILDINGEFFECTS.TOOLTIPS.DECORDECREASED);
			string text3 = GameUtil.AddPositiveSign(baseDecor.ToString(), baseDecor > 0f);
			Descriptor item2 = new Descriptor(string.Format(UI.BUILDINGEFFECTS.DECORPROVIDED, arg2, text3, baseRadius), string.Format(format, text3, baseRadius));
			list.Add(item2);
		}
		return list;
	}

	public static int GetLightDecorBonus(int cell)
	{
		if (Grid.LightIntensity[cell] > 0)
		{
			return DECOR.LIT_BONUS;
		}
		return 0;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		return GetEffectDescriptions();
	}
}
