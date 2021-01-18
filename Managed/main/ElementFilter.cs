using System;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/ElementFilter")]
public class ElementFilter : KMonoBehaviour, ISaveLoadable, ISecondaryOutput
{
	[SerializeField]
	public ConduitPortInfo portInfo;

	[MyCmpReq]
	private Operational operational;

	[MyCmpReq]
	private Building building;

	[MyCmpReq]
	private KSelectable selectable;

	[MyCmpReq]
	private Filterable filterable;

	private Guid needsConduitStatusItemGuid;

	private Guid conduitBlockedStatusItemGuid;

	private int inputCell = -1;

	private int outputCell = -1;

	private int filteredCell = -1;

	private FlowUtilityNetwork.NetworkItem itemFilter;

	private HandleVector<int>.Handle partitionerEntry;

	private static StatusItem filterStatusItem;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		InitializeStatusItems();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		inputCell = building.GetUtilityInputCell();
		outputCell = building.GetUtilityOutputCell();
		int cell = Grid.PosToCell(base.transform.GetPosition());
		CellOffset rotatedOffset = building.GetRotatedOffset(portInfo.offset);
		filteredCell = Grid.OffsetCell(cell, rotatedOffset);
		IUtilityNetworkMgr obj = ((portInfo.conduitType == ConduitType.Solid) ? SolidConduit.GetFlowManager().networkMgr : Conduit.GetNetworkManager(portInfo.conduitType));
		itemFilter = new FlowUtilityNetwork.NetworkItem(portInfo.conduitType, Endpoint.Source, filteredCell, base.gameObject);
		obj.AddToNetworks(filteredCell, itemFilter, is_endpoint: true);
		if (portInfo.conduitType == ConduitType.Gas || portInfo.conduitType == ConduitType.Liquid)
		{
			GetComponent<ConduitConsumer>().isConsuming = false;
		}
		OnFilterChanged(filterable.SelectedTag);
		filterable.onFilterChanged += OnFilterChanged;
		if (portInfo.conduitType == ConduitType.Solid)
		{
			SolidConduit.GetFlowManager().AddConduitUpdater(OnConduitTick);
		}
		else
		{
			Conduit.GetFlowManager(portInfo.conduitType).AddConduitUpdater(OnConduitTick);
		}
		GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, filterStatusItem, this);
		UpdateConduitExistsStatus();
		UpdateConduitBlockedStatus();
		ScenePartitionerLayer scenePartitionerLayer = null;
		switch (portInfo.conduitType)
		{
		case ConduitType.Gas:
			scenePartitionerLayer = GameScenePartitioner.Instance.gasConduitsLayer;
			break;
		case ConduitType.Liquid:
			scenePartitionerLayer = GameScenePartitioner.Instance.liquidConduitsLayer;
			break;
		case ConduitType.Solid:
			scenePartitionerLayer = GameScenePartitioner.Instance.solidConduitsLayer;
			break;
		}
		if (scenePartitionerLayer != null)
		{
			partitionerEntry = GameScenePartitioner.Instance.Add("ElementFilterConduitExists", base.gameObject, filteredCell, scenePartitionerLayer, delegate
			{
				UpdateConduitExistsStatus();
			});
		}
	}

	protected override void OnCleanUp()
	{
		Conduit.GetNetworkManager(portInfo.conduitType).RemoveFromNetworks(filteredCell, itemFilter, is_endpoint: true);
		if (portInfo.conduitType == ConduitType.Solid)
		{
			SolidConduit.GetFlowManager().RemoveConduitUpdater(OnConduitTick);
		}
		else
		{
			Conduit.GetFlowManager(portInfo.conduitType).RemoveConduitUpdater(OnConduitTick);
		}
		if (partitionerEntry.IsValid() && GameScenePartitioner.Instance != null)
		{
			GameScenePartitioner.Instance.Free(ref partitionerEntry);
		}
		base.OnCleanUp();
	}

	private void OnConduitTick(float dt)
	{
		bool value = false;
		UpdateConduitBlockedStatus();
		if (operational.IsOperational)
		{
			if (portInfo.conduitType == ConduitType.Gas || portInfo.conduitType == ConduitType.Liquid)
			{
				ConduitFlow flowManager = Conduit.GetFlowManager(portInfo.conduitType);
				ConduitFlow.ConduitContents contents = flowManager.GetContents(inputCell);
				int num = ((contents.element.CreateTag() == filterable.SelectedTag) ? filteredCell : outputCell);
				ConduitFlow.ConduitContents contents2 = flowManager.GetContents(num);
				if (contents.mass > 0f && contents2.mass <= 0f)
				{
					value = true;
					float num2 = flowManager.AddElement(num, contents.element, contents.mass, contents.temperature, contents.diseaseIdx, contents.diseaseCount);
					if (num2 > 0f)
					{
						flowManager.RemoveElement(inputCell, num2);
					}
				}
			}
			else
			{
				SolidConduitFlow flowManager2 = SolidConduit.GetFlowManager();
				Pickupable pickupable = flowManager2.GetPickupable(flowManager2.GetContents(inputCell).pickupableHandle);
				if (pickupable != null)
				{
					int num3 = ((pickupable.GetComponent<KPrefabID>().PrefabTag == filterable.SelectedTag) ? filteredCell : outputCell);
					Pickupable pickupable2 = flowManager2.GetPickupable(flowManager2.GetContents(num3).pickupableHandle);
					PrimaryElement primaryElement = null;
					if (pickupable2 != null)
					{
						primaryElement = pickupable2.PrimaryElement;
					}
					if (pickupable.PrimaryElement.Mass > 0f && (pickupable2 == null || primaryElement.Mass <= 0f))
					{
						value = true;
						Pickupable pickupable3 = flowManager2.RemovePickupable(inputCell);
						if (pickupable3 != null)
						{
							flowManager2.AddPickupable(num3, pickupable3);
						}
					}
				}
				else
				{
					flowManager2.RemovePickupable(inputCell);
				}
			}
		}
		operational.SetActive(value);
	}

	private void UpdateConduitExistsStatus()
	{
		bool flag = RequireOutputs.IsConnected(filteredCell, portInfo.conduitType);
		StatusItem status_item = portInfo.conduitType switch
		{
			ConduitType.Gas => Db.Get().BuildingStatusItems.NeedGasOut, 
			ConduitType.Liquid => Db.Get().BuildingStatusItems.NeedLiquidOut, 
			ConduitType.Solid => Db.Get().BuildingStatusItems.NeedSolidOut, 
			_ => throw new ArgumentOutOfRangeException(), 
		};
		bool flag2 = needsConduitStatusItemGuid != Guid.Empty;
		if (flag == flag2)
		{
			needsConduitStatusItemGuid = selectable.ToggleStatusItem(status_item, needsConduitStatusItemGuid, !flag);
		}
	}

	private void UpdateConduitBlockedStatus()
	{
		bool flag = Conduit.GetFlowManager(portInfo.conduitType).IsConduitEmpty(filteredCell);
		StatusItem conduitBlockedMultiples = Db.Get().BuildingStatusItems.ConduitBlockedMultiples;
		bool flag2 = conduitBlockedStatusItemGuid != Guid.Empty;
		if (flag == flag2)
		{
			conduitBlockedStatusItemGuid = selectable.ToggleStatusItem(conduitBlockedMultiples, conduitBlockedStatusItemGuid, !flag);
		}
	}

	private void OnFilterChanged(Tag tag)
	{
		bool flag = true;
		flag = !tag.IsValid || tag == GameTags.Void;
		GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.NoFilterElementSelected, flag);
	}

	private void InitializeStatusItems()
	{
		if (filterStatusItem == null)
		{
			filterStatusItem = new StatusItem("Filter", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.LiquidConduits.ID);
			filterStatusItem.resolveStringCallback = delegate(string str, object data)
			{
				ElementFilter elementFilter = (ElementFilter)data;
				str = ((elementFilter.filterable.SelectedTag.IsValid && !(elementFilter.filterable.SelectedTag == GameTags.Void)) ? string.Format(BUILDINGS.PREFABS.GASFILTER.STATUS_ITEM, elementFilter.filterable.SelectedTag.ProperName()) : string.Format(BUILDINGS.PREFABS.GASFILTER.STATUS_ITEM, BUILDINGS.PREFABS.GASFILTER.ELEMENT_NOT_SPECIFIED));
				return str;
			};
			filterStatusItem.conditionalOverlayCallback = ShowInUtilityOverlay;
		}
	}

	private bool ShowInUtilityOverlay(HashedString mode, object data)
	{
		bool result = false;
		switch (((ElementFilter)data).portInfo.conduitType)
		{
		case ConduitType.Gas:
			result = mode == OverlayModes.GasConduits.ID;
			break;
		case ConduitType.Liquid:
			result = mode == OverlayModes.LiquidConduits.ID;
			break;
		case ConduitType.Solid:
			result = mode == OverlayModes.SolidConveyor.ID;
			break;
		}
		return result;
	}

	public ConduitType GetSecondaryConduitType()
	{
		return portInfo.conduitType;
	}

	public CellOffset GetSecondaryConduitOffset()
	{
		return portInfo.offset;
	}

	public int GetFilteredCell()
	{
		return filteredCell;
	}
}
