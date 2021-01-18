using UnityEngine;

public class PlaceTool : DragTool
{
	[SerializeField]
	private TextStyleSetting tooltipStyle;

	private Tag previewTag;

	private Placeable source;

	private ToolTip tooltip;

	public static PlaceTool Instance;

	private bool active;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
		tooltip = GetComponent<ToolTip>();
	}

	protected override void OnActivateTool()
	{
		active = true;
		base.OnActivateTool();
		visualizer = GameUtil.KInstantiate(Assets.GetPrefab(previewTag), Grid.SceneLayer.Front, null, LayerMask.NameToLayer("Place"));
		KBatchedAnimController component = visualizer.GetComponent<KBatchedAnimController>();
		if (component != null)
		{
			component.visibilityType = KAnimControllerBase.VisibilityType.Always;
			component.isMovable = true;
		}
		visualizer.SetActive(value: true);
		ShowToolTip();
		GetComponent<BuildToolHoverTextCard>().currentDef = null;
		ResourceRemainingDisplayScreen.instance.ActivateDisplay(visualizer);
		if (component == null)
		{
			visualizer.SetLayerRecursively(LayerMask.NameToLayer("Place"));
		}
		else
		{
			component.SetLayer(LayerMask.NameToLayer("Place"));
		}
		GridCompositor.Instance.ToggleMajor(on: true);
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		active = false;
		GridCompositor.Instance.ToggleMajor(on: false);
		HideToolTip();
		ResourceRemainingDisplayScreen.instance.DeactivateDisplay();
		Object.Destroy(visualizer);
		KMonoBehaviour.PlaySound(GlobalAssets.GetSound(GetDeactivateSound()));
		base.OnDeactivateTool(new_tool);
	}

	public void Activate(Placeable source, Tag previewTag)
	{
		this.source = source;
		this.previewTag = previewTag;
		PlayerController.Instance.ActivateTool(this);
	}

	public void Deactivate()
	{
		SelectTool.Instance.Activate();
		source = null;
		previewTag = Tag.Invalid;
		ResourceRemainingDisplayScreen.instance.DeactivateDisplay();
	}

	protected override void OnDragTool(int cell, int distFromOrigin)
	{
		if (visualizer == null)
		{
			return;
		}
		bool flag = false;
		if (visualizer.GetComponent<EntityPreview>().Valid)
		{
			if (DebugHandler.InstantBuildMode)
			{
				source.Place(cell);
			}
			else
			{
				source.QueuePlacement(cell);
			}
			flag = true;
		}
		if (flag)
		{
			Deactivate();
		}
	}

	protected override Mode GetMode()
	{
		return Mode.Brush;
	}

	private void ShowToolTip()
	{
		ToolTipScreen.Instance.SetToolTip(tooltip);
	}

	private void HideToolTip()
	{
		ToolTipScreen.Instance.ClearToolTip(tooltip);
	}

	public void Update()
	{
		if (active)
		{
			KBatchedAnimController component = visualizer.GetComponent<KBatchedAnimController>();
			if (component != null)
			{
				component.SetLayer(LayerMask.NameToLayer("Place"));
			}
		}
	}

	public override string GetDeactivateSound()
	{
		return "HUD_Click_Deselect";
	}
}
