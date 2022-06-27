using System;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class ClusterCategorySelectionScreen : NewGameFlowScreen
{
	[SerializeField]
	private MultiToggle spacedOutButton;

	private Image spacedOutButtonHeader;

	private Image spacedOutButtonSelectionFrame;

	[SerializeField]
	private MultiToggle vanillaButton;

	private Image vanillaButtonHeader;

	private Image vanillalButtonSelectionFrame;

	[SerializeField]
	private LocText descriptionArea;

	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private KBatchedAnimController nosweatAnim;

	[SerializeField]
	private KBatchedAnimController survivalAnim;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		HierarchyReferences component = vanillaButton.GetComponent<HierarchyReferences>();
		vanillaButtonHeader = component.GetReference<RectTransform>("HeaderBackground").GetComponent<Image>();
		vanillalButtonSelectionFrame = component.GetReference<RectTransform>("SelectionFrame").GetComponent<Image>();
		MultiToggle multiToggle = vanillaButton;
		multiToggle.onEnter = (System.Action)Delegate.Combine(multiToggle.onEnter, new System.Action(OnHoverEnterVanilla));
		MultiToggle multiToggle2 = vanillaButton;
		multiToggle2.onExit = (System.Action)Delegate.Combine(multiToggle2.onExit, new System.Action(OnHoverExitVanilla));
		MultiToggle multiToggle3 = vanillaButton;
		multiToggle3.onClick = (System.Action)Delegate.Combine(multiToggle3.onClick, new System.Action(OnClickVanilla));
		HierarchyReferences component2 = spacedOutButton.GetComponent<HierarchyReferences>();
		spacedOutButtonHeader = component2.GetReference<RectTransform>("HeaderBackground").GetComponent<Image>();
		spacedOutButtonSelectionFrame = component2.GetReference<RectTransform>("SelectionFrame").GetComponent<Image>();
		MultiToggle multiToggle4 = spacedOutButton;
		multiToggle4.onEnter = (System.Action)Delegate.Combine(multiToggle4.onEnter, new System.Action(OnHoverEnterSpacedOut));
		MultiToggle multiToggle5 = spacedOutButton;
		multiToggle5.onExit = (System.Action)Delegate.Combine(multiToggle5.onExit, new System.Action(OnHoverExitSpacedOut));
		MultiToggle multiToggle6 = spacedOutButton;
		multiToggle6.onClick = (System.Action)Delegate.Combine(multiToggle6.onClick, new System.Action(OnClickSpacedOut));
		closeButton.onClick += base.NavigateBackward;
	}

	private void OnHoverEnterVanilla()
	{
		KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Mouseover"));
		vanillalButtonSelectionFrame.SetAlpha(1f);
		vanillaButtonHeader.color = new Color(0.7019608f, 31f / 85f, 8f / 15f, 1f);
		descriptionArea.text = UI.FRONTEND.CLUSTERCATEGORYSELECTSCREEN.VANILLA_DESC;
	}

	private void OnHoverExitVanilla()
	{
		KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Mouseover"));
		vanillalButtonSelectionFrame.SetAlpha(0f);
		vanillaButtonHeader.color = new Color(0.30980393f, 29f / 85f, 0.38431373f, 1f);
		descriptionArea.text = UI.FRONTEND.CLUSTERCATEGORYSELECTSCREEN.BLANK_DESC;
	}

	private void OnClickVanilla()
	{
		Deactivate();
		DestinationSelectPanel.ChosenClusterCategorySetting = 1;
		NavigateForward();
	}

	private void OnHoverEnterSpacedOut()
	{
		KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Mouseover"));
		spacedOutButtonSelectionFrame.SetAlpha(1f);
		spacedOutButtonHeader.color = new Color(0.7019608f, 31f / 85f, 8f / 15f, 1f);
		descriptionArea.text = UI.FRONTEND.CLUSTERCATEGORYSELECTSCREEN.SPACEDOUT_DESC;
	}

	private void OnHoverExitSpacedOut()
	{
		KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Mouseover"));
		spacedOutButtonSelectionFrame.SetAlpha(0f);
		spacedOutButtonHeader.color = new Color(0.30980393f, 29f / 85f, 0.38431373f, 1f);
		descriptionArea.text = UI.FRONTEND.CLUSTERCATEGORYSELECTSCREEN.BLANK_DESC;
	}

	private void OnClickSpacedOut()
	{
		Deactivate();
		DestinationSelectPanel.ChosenClusterCategorySetting = 2;
		NavigateForward();
	}
}
