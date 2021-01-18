using System;
using System.Collections.Generic;
using UnityEngine;

public class GeneratedOre
{
	public static void LoadGeneratedOre(List<Type> types)
	{
		Type typeFromHandle = typeof(IOreConfig);
		HashSet<SimHashes> hashSet = new HashSet<SimHashes>();
		foreach (Type type in types)
		{
			if (typeFromHandle.IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
			{
				IOreConfig obj = Activator.CreateInstance(type) as IOreConfig;
				SimHashes elementID = obj.ElementID;
				if (elementID != SimHashes.Void)
				{
					hashSet.Add(elementID);
				}
				Assets.AddPrefab(obj.CreatePrefab().GetComponent<KPrefabID>());
			}
		}
		foreach (Element element in ElementLoader.elements)
		{
			if (element == null || hashSet.Contains(element.id))
			{
				continue;
			}
			if (element.substance != null && element.substance.anim != null)
			{
				GameObject gameObject = null;
				if (element.IsSolid)
				{
					gameObject = EntityTemplates.CreateSolidOreEntity(element.id);
				}
				else if (element.IsLiquid)
				{
					gameObject = EntityTemplates.CreateLiquidOreEntity(element.id);
				}
				else if (element.IsGas)
				{
					gameObject = EntityTemplates.CreateGasOreEntity(element.id);
				}
				if (gameObject != null)
				{
					Assets.AddPrefab(gameObject.GetComponent<KPrefabID>());
				}
			}
			else
			{
				Debug.LogError("Missing substance or anim for element [" + element.name + "]");
			}
		}
	}

	public static SubstanceChunk CreateChunk(Element element, float mass, float temperature, byte diseaseIdx, int diseaseCount, Vector3 position)
	{
		if (temperature <= 0f)
		{
			DebugUtil.LogWarningArgs("GeneratedOre.CreateChunk tried to create a chunk with a temperature <= 0");
		}
		GameObject prefab = Assets.GetPrefab(element.tag);
		if (prefab == null)
		{
			Debug.LogError("Could not find prefab for element " + element.id);
		}
		SubstanceChunk component = GameUtil.KInstantiate(prefab, Grid.SceneLayer.Ore).GetComponent<SubstanceChunk>();
		component.transform.SetPosition(position);
		component.gameObject.SetActive(value: true);
		PrimaryElement component2 = component.GetComponent<PrimaryElement>();
		component2.Mass = mass;
		component2.Temperature = temperature;
		component2.AddDisease(diseaseIdx, diseaseCount, "GeneratedOre.CreateChunk");
		component.GetComponent<KPrefabID>().InitializeTags();
		return component;
	}
}
