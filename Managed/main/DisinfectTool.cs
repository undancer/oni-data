using UnityEngine;

public class DisinfectTool : DragTool
{
	public static DisinfectTool Instance;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
		interceptNumberKeysForPriority = true;
		viewMode = OverlayModes.Disease.ID;
	}

	public void Activate()
	{
		PlayerController.Instance.ActivateTool(this);
	}

	protected override void OnDragTool(int cell, int distFromOrigin)
	{
		for (int i = 0; i < 42; i++)
		{
			GameObject gameObject = Grid.Objects[cell, i];
			if (gameObject != null)
			{
				Disinfectable component = gameObject.GetComponent<Disinfectable>();
				if (component != null && component.GetComponent<PrimaryElement>().DiseaseCount > 0)
				{
					component.MarkForDisinfect();
				}
			}
		}
	}
}
