using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
[AddComponentMenu("KMonoBehaviour/scripts/GraphedBar")]
public class GraphedBar : KMonoBehaviour
{
	public GameObject segments_container;

	public GameObject prefab_segment;

	private List<GameObject> segments = new List<GameObject>();

	private GraphedBarFormatting format;

	public void SetFormat(GraphedBarFormatting format)
	{
		this.format = format;
	}

	public void SetValues(int[] values, float x_position)
	{
		ClearValues();
		base.gameObject.rectTransform().anchorMin = new Vector2(x_position, 0f);
		base.gameObject.rectTransform().anchorMax = new Vector2(x_position, 1f);
		base.gameObject.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, format.width);
		for (int i = 0; i < values.Length; i++)
		{
			GameObject gameObject = Util.KInstantiateUI(prefab_segment, segments_container, force_active: true);
			LayoutElement component = gameObject.GetComponent<LayoutElement>();
			component.preferredHeight = values[i];
			component.minWidth = format.width;
			Image component2 = gameObject.GetComponent<Image>();
			component2.color = format.colors[i % format.colors.Length];
			segments.Add(gameObject);
		}
	}

	public void ClearValues()
	{
		foreach (GameObject segment in segments)
		{
			UnityEngine.Object.DestroyImmediate(segment);
		}
		segments.Clear();
	}
}
