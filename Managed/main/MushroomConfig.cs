using STRINGS;
using TUNING;
using UnityEngine;

public class MushroomConfig : IEntityConfig
{
	public static float SEEDS_PER_FRUIT_CHANCE = 0.05f;

	public static string ID = "Mushroom";

	private static readonly EventSystem.IntraObjectHandler<Edible> OnEatCompleteDelegate = new EventSystem.IntraObjectHandler<Edible>(delegate(Edible component, object data)
	{
		OnEatComplete(component);
	});

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		GameObject template = EntityTemplates.CreateLooseEntity(ID, ITEMS.FOOD.MUSHROOM.NAME, ITEMS.FOOD.MUSHROOM.DESC, 1f, unitMass: false, Assets.GetAnim("funguscap_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.77f, 0.48f, isPickupable: true);
		return EntityTemplates.ExtendEntityToFood(template, FOOD.FOOD_TYPES.MUSHROOM);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
		inst.Subscribe(-10536414, OnEatCompleteDelegate);
	}

	private static void OnEatComplete(Edible edible)
	{
		if (!(edible != null))
		{
			return;
		}
		int num = 0;
		float unitsConsumed = edible.unitsConsumed;
		int num2 = Mathf.FloorToInt(unitsConsumed);
		float num3 = unitsConsumed % 1f;
		if (Random.value < num3)
		{
			num2++;
		}
		for (int i = 0; i < num2; i++)
		{
			if (Random.value < SEEDS_PER_FRUIT_CHANCE)
			{
				num++;
			}
		}
		if (num > 0)
		{
			Vector3 pos = edible.transform.GetPosition() + new Vector3(0f, 0.05f, 0f);
			pos = Grid.CellToPosCCC(Grid.PosToCell(pos), Grid.SceneLayer.Ore);
			GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab(new Tag("MushroomSeed")), pos, Grid.SceneLayer.Ore);
			PrimaryElement component = edible.GetComponent<PrimaryElement>();
			PrimaryElement component2 = gameObject.GetComponent<PrimaryElement>();
			component2.Temperature = component.Temperature;
			component2.Units = num;
			gameObject.SetActive(value: true);
		}
	}
}
