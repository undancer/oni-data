using System;
using UnityEngine;

public class PlaceTool : DragTool
{
	[SerializeField]
	private TextStyleSetting tooltipStyle;

	private Action<Placeable, int> onPlacedCallback;

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
		visualizer = new GameObject("PlaceToolVisualizer");
		visualizer.SetActive(value: false);
		visualizer.SetLayerRecursively(LayerMask.NameToLayer("Place"));
		KBatchedAnimController kBatchedAnimController = visualizer.AddComponent<KBatchedAnimController>();
		kBatchedAnimController.visibilityType = KAnimControllerBase.VisibilityType.Always;
		kBatchedAnimController.isMovable = true;
		kBatchedAnimController.SetLayer(LayerMask.NameToLayer("Place"));
		kBatchedAnimController.AnimFiles = new KAnimFile[1]
		{
			Assets.GetAnim(source.kAnimName)
		};
		kBatchedAnimController.initialAnim = source.animName;
		visualizer.SetActive(value: true);
		ShowToolTip();
		GetComponent<PlaceToolHoverTextCard>().currentPlaceable = source;
		ResourceRemainingDisplayScreen.instance.ActivateDisplay(visualizer);
		GridCompositor.Instance.ToggleMajor(on: true);
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		active = false;
		GridCompositor.Instance.ToggleMajor(on: false);
		HideToolTip();
		ResourceRemainingDisplayScreen.instance.DeactivateDisplay();
		UnityEngine.Object.Destroy(visualizer);
		KMonoBehaviour.PlaySound(GlobalAssets.GetSound(GetDeactivateSound()));
		source = null;
		onPlacedCallback = null;
		base.OnDeactivateTool(new_tool);
	}

	public void Activate(Placeable source, Action<Placeable, int> onPlacedCallback)
	{
		this.source = source;
		this.onPlacedCallback = onPlacedCallback;
		PlayerController.Instance.ActivateTool(this);
	}

	protected override void OnDragTool(int cell, int distFromOrigin)
	{
		if (!(visualizer == null))
		{
			bool flag = false;
			if (source.IsValidPlaceLocation(cell, out var _))
			{
				onPlacedCallback(source, cell);
				flag = true;
			}
			if (flag)
			{
				DeactivateTool();
			}
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

	public override void OnMouseMove(Vector3 cursorPos)
	{
		cursorPos = ClampPositionToWorld(cursorPos, ClusterManager.Instance.activeWorld);
		int cell = Grid.PosToCell(cursorPos);
		KBatchedAnimController component = visualizer.GetComponent<KBatchedAnimController>();
		if (source.IsValidPlaceLocation(cell, out var _))
		{
			component.TintColour = Color.white;
		}
		else
		{
			component.TintColour = Color.red;
		}
		base.OnMouseMove(cursorPos);
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
