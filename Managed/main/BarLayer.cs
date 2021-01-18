using System.Collections.Generic;
using UnityEngine;

public class BarLayer : GraphLayer
{
	public GameObject bar_container;

	public GameObject prefab_bar;

	public GraphedBarFormatting[] bar_formats;

	private List<GraphedBar> bars = new List<GraphedBar>();

	public int bar_count => bars.Count;

	public void NewBar(int[] values, float x_position, string ID = "")
	{
		GameObject gameObject = Util.KInstantiateUI(prefab_bar, bar_container, force_active: true);
		if (ID == "")
		{
			ID = bars.Count.ToString();
		}
		gameObject.name = $"bar_{ID}";
		GraphedBar component = gameObject.GetComponent<GraphedBar>();
		component.SetFormat(bar_formats[bars.Count % bar_formats.Length]);
		int[] array = new int[values.Length];
		for (int i = 0; i < values.Length; i++)
		{
			array[i] = (int)(base.graph.rectTransform().rect.height * base.graph.GetRelativeSize(new Vector2(0f, values[i])).y);
		}
		component.SetValues(array, base.graph.GetRelativePosition(new Vector2(x_position, 0f)).x);
		bars.Add(component);
	}

	public void ClearBars()
	{
		foreach (GraphedBar bar in bars)
		{
			if (bar != null && bar.gameObject != null)
			{
				Object.DestroyImmediate(bar.gameObject);
			}
		}
		bars.Clear();
	}
}
