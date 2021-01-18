using System.Collections.Generic;
using Klei.AI;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/HarvestableOverlayWidget")]
public class HarvestableOverlayWidget : KMonoBehaviour
{
	[SerializeField]
	private GameObject vertical_container;

	[SerializeField]
	private GameObject bar;

	private const int icons_per_row = 2;

	private const float bar_fill_range = 19f;

	private const float bar_fill_offset = 3f;

	private static Color growing_color = new Color(251f / 255f, 176f / 255f, 59f / 255f, 1f);

	private static Color wilting_color = new Color(48f / 85f, 48f / 85f, 48f / 85f, 1f);

	[SerializeField]
	private Sprite sprite_liquid;

	[SerializeField]
	private Sprite sprite_atmosphere;

	[SerializeField]
	private Sprite sprite_pressure;

	[SerializeField]
	private Sprite sprite_temperature;

	[SerializeField]
	private Sprite sprite_resource;

	[SerializeField]
	private Sprite sprite_light;

	[SerializeField]
	private Sprite sprite_receptacle;

	[SerializeField]
	private GameObject horizontal_container_prefab;

	private GameObject[] horizontal_containers = new GameObject[6];

	[SerializeField]
	private GameObject icon_gameobject_prefab;

	private Dictionary<WiltCondition.Condition, GameObject> condition_icons = new Dictionary<WiltCondition.Condition, GameObject>();

	private Dictionary<WiltCondition.Condition, Sprite> condition_sprites = new Dictionary<WiltCondition.Condition, Sprite>();

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		condition_sprites.Add(WiltCondition.Condition.AtmosphereElement, sprite_atmosphere);
		condition_sprites.Add(WiltCondition.Condition.Darkness, sprite_light);
		condition_sprites.Add(WiltCondition.Condition.Drowning, sprite_liquid);
		condition_sprites.Add(WiltCondition.Condition.DryingOut, sprite_liquid);
		condition_sprites.Add(WiltCondition.Condition.Fertilized, sprite_resource);
		condition_sprites.Add(WiltCondition.Condition.IlluminationComfort, sprite_light);
		condition_sprites.Add(WiltCondition.Condition.Irrigation, sprite_liquid);
		condition_sprites.Add(WiltCondition.Condition.Pressure, sprite_pressure);
		condition_sprites.Add(WiltCondition.Condition.Temperature, sprite_temperature);
		condition_sprites.Add(WiltCondition.Condition.Receptacle, sprite_receptacle);
		for (int i = 0; i < horizontal_containers.Length; i++)
		{
			GameObject gameObject = Util.KInstantiateUI(horizontal_container_prefab, vertical_container);
			horizontal_containers[i] = gameObject;
		}
		for (int j = 0; j < 12; j++)
		{
			if (condition_sprites.ContainsKey((WiltCondition.Condition)j))
			{
				GameObject gameObject2 = Util.KInstantiateUI(icon_gameobject_prefab, base.gameObject);
				gameObject2.GetComponent<Image>().sprite = condition_sprites[(WiltCondition.Condition)j];
				condition_icons.Add((WiltCondition.Condition)j, gameObject2);
			}
		}
	}

	public void Refresh(HarvestDesignatable target_harvestable)
	{
		Image image = bar.GetComponent<HierarchyReferences>().GetReference("Fill") as Image;
		AmountInstance amountInstance = Db.Get().Amounts.Maturity.Lookup(target_harvestable);
		if (amountInstance != null)
		{
			float num = amountInstance.value / amountInstance.GetMax();
			image.rectTransform.offsetMin = new Vector2(image.rectTransform.offsetMin.x, 3f);
			if (bar.activeSelf != !target_harvestable.CanBeHarvested())
			{
				bar.SetActive(!target_harvestable.CanBeHarvested());
			}
			float num2 = (target_harvestable.CanBeHarvested() ? 3f : (19f - 19f * num + 3f));
			image.rectTransform.offsetMax = new Vector2(image.rectTransform.offsetMax.x, 0f - num2);
		}
		else if (bar.activeSelf)
		{
			bar.SetActive(value: false);
		}
		WiltCondition component = target_harvestable.GetComponent<WiltCondition>();
		if (component != null)
		{
			for (int i = 0; i < horizontal_containers.Length; i++)
			{
				horizontal_containers[i].SetActive(value: false);
			}
			foreach (KeyValuePair<WiltCondition.Condition, GameObject> condition_icon in condition_icons)
			{
				condition_icon.Value.SetActive(value: false);
			}
			if (component.IsWilting())
			{
				vertical_container.SetActive(value: true);
				image.color = wilting_color;
				List<WiltCondition.Condition> list = component.CurrentWiltSources();
				if (list.Count <= 0)
				{
					return;
				}
				for (int j = 0; j < list.Count; j++)
				{
					if (condition_icons.ContainsKey(list[j]))
					{
						condition_icons[list[j]].SetActive(value: true);
						horizontal_containers[j / 2].SetActive(value: true);
						condition_icons[list[j]].transform.SetParent(horizontal_containers[j / 2].transform);
					}
				}
			}
			else
			{
				vertical_container.SetActive(value: false);
				image.color = growing_color;
			}
		}
		else
		{
			image.color = growing_color;
			vertical_container.SetActive(value: false);
		}
	}
}
