using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/StarmapPlanet")]
public class StarmapPlanet : KMonoBehaviour
{
	public List<StarmapPlanetVisualizer> visualizers;

	public void SetSprite(Sprite sprite, Color color)
	{
		foreach (StarmapPlanetVisualizer visualizer in visualizers)
		{
			visualizer.image.sprite = sprite;
			visualizer.image.color = color;
		}
	}

	public void SetFillAmount(float amount)
	{
		foreach (StarmapPlanetVisualizer visualizer in visualizers)
		{
			visualizer.image.fillAmount = amount;
		}
	}

	public void SetUnknownBGActive(bool active, Color color)
	{
		foreach (StarmapPlanetVisualizer visualizer in visualizers)
		{
			visualizer.unknownBG.gameObject.SetActive(active);
			visualizer.unknownBG.color = color;
		}
	}

	public void SetSelectionActive(bool active)
	{
		foreach (StarmapPlanetVisualizer visualizer in visualizers)
		{
			visualizer.selection.gameObject.SetActive(active);
		}
	}

	public void SetAnalysisActive(bool active)
	{
		foreach (StarmapPlanetVisualizer visualizer in visualizers)
		{
			visualizer.analysisSelection.SetActive(active);
		}
	}

	public void SetLabel(string text)
	{
		foreach (StarmapPlanetVisualizer visualizer in visualizers)
		{
			visualizer.label.text = text;
			ShowLabel(show: false);
		}
	}

	public void ShowLabel(bool show)
	{
		foreach (StarmapPlanetVisualizer visualizer in visualizers)
		{
			visualizer.label.gameObject.SetActive(show);
		}
	}

	public void SetOnClick(System.Action del)
	{
		foreach (StarmapPlanetVisualizer visualizer in visualizers)
		{
			visualizer.button.onClick = del;
		}
	}

	public void SetOnEnter(System.Action del)
	{
		foreach (StarmapPlanetVisualizer visualizer in visualizers)
		{
			visualizer.button.onEnter = del;
		}
	}

	public void SetOnExit(System.Action del)
	{
		foreach (StarmapPlanetVisualizer visualizer in visualizers)
		{
			visualizer.button.onExit = del;
		}
	}

	public void AnimateSelector(float time)
	{
		foreach (StarmapPlanetVisualizer visualizer in visualizers)
		{
			visualizer.selection.anchoredPosition = new Vector2(0f, 25f + Mathf.Sin(time * 4f) * 5f);
		}
	}

	public void ShowAsCurrentRocketDestination(bool show)
	{
		foreach (StarmapPlanetVisualizer visualizer in visualizers)
		{
			RectTransform rectTransform = visualizer.rocketIconContainer.rectTransform();
			if (rectTransform.childCount > 0)
			{
				rectTransform.GetChild(rectTransform.childCount - 1).GetComponent<HierarchyReferences>().GetReference<Image>("fg")
					.color = (show ? new Color(0.11764706f, 44f / 51f, 16f / 51f) : Color.white);
			}
		}
	}

	public void SetRocketIcons(int numRockets, GameObject iconPrefab)
	{
		foreach (StarmapPlanetVisualizer visualizer in visualizers)
		{
			RectTransform rectTransform = visualizer.rocketIconContainer.rectTransform();
			for (int i = rectTransform.childCount; i < numRockets; i++)
			{
				Util.KInstantiateUI(iconPrefab, visualizer.rocketIconContainer, force_active: true);
			}
			for (int num = rectTransform.childCount; num > numRockets; num--)
			{
				UnityEngine.Object.Destroy(rectTransform.GetChild(num - 1).gameObject);
			}
			int num2 = 0;
			foreach (RectTransform item in rectTransform)
			{
				item.anchoredPosition = new Vector2((float)num2 * -10f, 0f);
				num2++;
			}
		}
	}
}
