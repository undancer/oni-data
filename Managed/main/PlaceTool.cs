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

	private bool active = false;

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
		visualizer = GameUtil.KInstantiate(Assets.GetPrefab(source.previewTag), Grid.SceneLayer.Front, null, LayerMask.NameToLayer("Place"));
		KBatchedAnimController component = visualizer.GetComponent<KBatchedAnimController>();
		if (component != null)
		{
			component.visibilityType = KAnimControllerBase.VisibilityType.Always;
			component.isMovable = true;
		}
		visualizer.SetActive(value: true);
		ShowToolTip();
		BuildToolHoverTextCard component2 = GetComponent<BuildToolHoverTextCard>();
		component2.currentDef = null;
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
			EntityPreview component = visualizer.GetComponent<EntityPreview>();
			if (component.Valid)
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
