using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/BatteryUI")]
public class BatteryUI : KMonoBehaviour
{
	[SerializeField]
	private LocText currentKJLabel;

	[SerializeField]
	private Image batteryBG;

	[SerializeField]
	private Image batteryMeter;

	[SerializeField]
	private Sprite regularBatteryBG;

	[SerializeField]
	private Sprite bigBatteryBG;

	[SerializeField]
	private Color energyIncreaseColor = Color.green;

	[SerializeField]
	private Color energyDecreaseColor = Color.red;

	private LocText unitLabel;

	private const float UIUnit = 10f;

	private Dictionary<float, float> sizeMap;

	private void Initialize()
	{
		if (unitLabel == null)
		{
			unitLabel = currentKJLabel.gameObject.GetComponentInChildrenOnly<LocText>();
		}
		if (sizeMap == null || sizeMap.Count == 0)
		{
			sizeMap = new Dictionary<float, float>();
			sizeMap.Add(20000f, 10f);
			sizeMap.Add(40000f, 25f);
			sizeMap.Add(60000f, 40f);
		}
	}

	public void SetContent(Battery bat)
	{
		if (bat == null)
		{
			if (base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(value: false);
			}
			return;
		}
		Initialize();
		RectTransform component = batteryBG.GetComponent<RectTransform>();
		float num = 0f;
		foreach (KeyValuePair<float, float> item in sizeMap)
		{
			if (!(bat.Capacity > item.Key))
			{
				num = item.Value;
				break;
			}
		}
		batteryBG.sprite = ((bat.Capacity >= 40000f) ? bigBatteryBG : regularBatteryBG);
		float y = 25f;
		component.sizeDelta = new Vector2(num, y);
		BuildingEnabledButton component2 = bat.GetComponent<BuildingEnabledButton>();
		Color color = ((!(component2 != null) || component2.IsEnabled) ? ((bat.PercentFull >= bat.PreviousPercentFull) ? energyIncreaseColor : energyDecreaseColor) : Color.gray);
		batteryMeter.color = color;
		batteryBG.color = color;
		float num2 = batteryBG.GetComponent<RectTransform>().rect.height * bat.PercentFull;
		batteryMeter.GetComponent<RectTransform>().sizeDelta = new Vector2(num - 5.5f, num2 - 5.5f);
		color.a = 1f;
		if (currentKJLabel.color != color)
		{
			currentKJLabel.color = color;
			unitLabel.color = color;
		}
		currentKJLabel.text = bat.JoulesAvailable.ToString("F0");
	}
}
