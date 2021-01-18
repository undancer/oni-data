using System;
using UnityEngine;

public class GlassForge : ComplexFabricator
{
	private Guid statusHandle;

	private static readonly EventSystem.IntraObjectHandler<GlassForge> CheckPipesDelegate = new EventSystem.IntraObjectHandler<GlassForge>(delegate(GlassForge component, object data)
	{
		component.CheckPipes(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-2094018600, CheckPipesDelegate);
	}

	private void CheckPipes(object data)
	{
		KSelectable component = GetComponent<KSelectable>();
		int cell = Grid.OffsetCell(Grid.PosToCell(this), GlassForgeConfig.outPipeOffset);
		GameObject gameObject = Grid.Objects[cell, 16];
		if (gameObject != null)
		{
			PrimaryElement component2 = gameObject.GetComponent<PrimaryElement>();
			if (component2.Element.highTemp > ElementLoader.FindElementByHash(SimHashes.MoltenGlass).lowTemp)
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
