using UnityEngine;

public class InfraredVisualizerComponents : KGameObjectComponentManager<InfraredVisualizerData>
{
	public HandleVector<int>.Handle Add(GameObject go)
	{
		return Add(go, new InfraredVisualizerData(go));
	}

	public void UpdateTemperature()
	{
		GridArea visibleArea = GridVisibleArea.GetVisibleArea();
		for (int i = 0; i < data.Count; i++)
		{
			KAnimControllerBase controller = data[i].controller;
			if (controller != null)
			{
				Vector3 position = controller.transform.GetPosition();
				if (visibleArea.Min <= position && position <= visibleArea.Max)
				{
					data[i].Update();
				}
			}
		}
	}

	public void ClearOverlayColour()
	{
		Color32 c = Color.black;
		for (int i = 0; i < data.Count; i++)
		{
			KAnimControllerBase controller = data[i].controller;
			if (controller != null)
			{
				controller.OverlayColour = c;
			}
		}
	}

	public static void ClearOverlayColour(KBatchedAnimController controller)
	{
		if (controller != null)
		{
			controller.OverlayColour = Color.black;
		}
	}
}
