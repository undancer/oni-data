using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/GoodnessSlider")]
public class GoodnessSlider : KMonoBehaviour
{
	public Image icon;

	public Text text;

	public Slider slider;

	public Image fill;

	public Gradient gradient;

	public string[] names;

	protected override void OnSpawn()
	{
		Spawn();
		UpdateValues();
	}

	public void UpdateValues()
	{
		Color color3 = (text.color = (fill.color = gradient.Evaluate(slider.value)));
		for (int i = 0; i < gradient.colorKeys.Length; i++)
		{
			if (gradient.colorKeys[i].time < slider.value)
			{
				text.text = names[i];
			}
			if (i == gradient.colorKeys.Length - 1 && gradient.colorKeys[i - 1].time < slider.value)
			{
				text.text = names[i];
			}
		}
	}
}
