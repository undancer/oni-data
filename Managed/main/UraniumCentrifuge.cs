using System;
using UnityEngine;

public class UraniumCentrifuge : ComplexFabricator
{
	private Guid statusHandle;

	private static readonly EventSystem.IntraObjectHandler<UraniumCentrifuge> CheckPipesDelegate = new EventSystem.IntraObjectHandler<UraniumCentrifuge>(delegate(UraniumCentrifuge component, object data)
	{
		component.CheckPipes(data);
	});

	private static readonly EventSystem.IntraObjectHandler<UraniumCentrifuge> DropEnrichedProductDelegate = new EventSystem.IntraObjectHandler<UraniumCentrifuge>(delegate(UraniumCentrifuge component, object data)
	{
		component.DropEnrichedProducts(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-1697596308, DropEnrichedProductDelegate);
		Subscribe(-2094018600, CheckPipesDelegate);
	}

	private void DropEnrichedProducts(object data)
	{
		Storage[] components = GetComponents<Storage>();
		foreach (Storage storage in components)
		{
			storage.Drop(ElementLoader.FindElementByHash(SimHashes.EnrichedUranium).tag);
		}
	}

	private void CheckPipes(object data)
	{
		KSelectable component = GetComponent<KSelectable>();
		int cell = Grid.OffsetCell(Grid.PosToCell(this), UraniumCentrifugeConfig.outPipeOffset);
		GameObject gameObject = Grid.Objects[cell, 16];
		if (gameObject != null)
		{
			PrimaryElement component2 = gameObject.GetComponent<PrimaryElement>();
			if (component2.Element.highTemp > ElementLoader.FindElementByHash(SimHashes.MoltenUranium).lowTemp)
			{
				component.RemoveStatusItem(statusHandle);
			}
			else
			{
				statusHandle = component.AddStatusItem(Db.Get().BuildingStatusItems.PipeMayMelt);
			}
		}
		else
		{
			component.RemoveStatusItem(statusHandle);
		}
	}
}
