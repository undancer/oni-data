using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class ClusterDestinationSideScreen : SideScreenContent
{
	public Image destinationImage;

	public LocText destinationLabel;

	public KButton changeDestinationButton;

	public KButton clearDestinationButton;

	public DropDown launchPadDropDown;

	public KButton repeatButton;

	public ColorStyleSetting repeatOff;

	public ColorStyleSetting repeatOn;

	public ColorStyleSetting defaultButton;

	public ColorStyleSetting highlightButton;

	private int m_refreshHandle = -1;

	private ClusterDestinationSelector targetSelector { get; set; }

	private RocketClusterDestinationSelector targetRocketSelector { get; set; }

	protected override void OnSpawn()
	{
		changeDestinationButton.onClick += OnClickChangeDestination;
		clearDestinationButton.onClick += OnClickClearDestination;
		launchPadDropDown.targetDropDownContainer = GameScreenManager.Instance.ssOverlayCanvas;
		launchPadDropDown.CustomizeEmptyRow(UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.FIRSTAVAILABLE, null);
		repeatButton.onClick += OnRepeatClicked;
	}

	public override int GetSideScreenSortOrder()
	{
		return 300;
	}

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		if (show)
		{
			Refresh();
			m_refreshHandle = targetSelector.Subscribe(543433792, delegate
			{
				Refresh();
			});
		}
		else if (m_refreshHandle != -1)
		{
			targetSelector.Unsubscribe(m_refreshHandle);
			m_refreshHandle = -1;
			launchPadDropDown.Close();
		}
	}

	public override bool IsValidForTarget(GameObject target)
	{
		ClusterDestinationSelector component = target.GetComponent<ClusterDestinationSelector>();
		if ((!(component != null) || !component.assignable) && (!(target.GetComponent<RocketModule>() != null) || !target.HasTag(GameTags.LaunchButtonRocketModule)))
		{
			if (target.GetComponent<RocketControlStation>() != null)
			{
				return target.GetComponent<RocketControlStation>().GetMyWorld().GetComponent<Clustercraft>()
					.Status != Clustercraft.CraftStatus.Launching;
			}
			return false;
		}
		return true;
	}

	public override void SetTarget(GameObject target)
	{
		targetSelector = target.GetComponent<ClusterDestinationSelector>();
		if (targetSelector == null)
		{
			if (target.GetComponent<RocketModuleCluster>() != null)
			{
				targetSelector = target.GetComponent<RocketModuleCluster>().CraftInterface.GetClusterDestinationSelector();
			}
			else if (target.GetComponent<RocketControlStation>() != null)
			{
				targetSelector = target.GetMyWorld().GetComponent<Clustercraft>().ModuleInterface.GetClusterDestinationSelector();
			}
		}
		targetRocketSelector = targetSelector as RocketClusterDestinationSelector;
	}

	private void Refresh(object data = null)
	{
		if (!targetSelector.IsAtDestination())
		{
			ClusterGrid.Instance.GetLocationDescription(targetSelector.GetDestination(), out var sprite, out var label, out var _);
			destinationImage.sprite = sprite;
			destinationLabel.text = string.Concat(UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.TITLE, ": ", label);
			clearDestinationButton.isInteractable = true;
		}
		else
		{
			destinationImage.sprite = Assets.GetSprite("hex_unknown");
			destinationLabel.text = string.Concat(UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.TITLE, ": ", UI.SPACEDESTINATIONS.NONE.NAME);
			clearDestinationButton.isInteractable = false;
		}
		if (targetRocketSelector != null)
		{
			List<LaunchPad> launchPadsForDestination = LaunchPad.GetLaunchPadsForDestination(targetRocketSelector.GetDestination());
			launchPadDropDown.gameObject.SetActive(value: true);
			repeatButton.gameObject.SetActive(value: true);
			launchPadDropDown.Initialize(launchPadsForDestination, OnLaunchPadEntryClick, PadDropDownSort, PadDropDownEntryRefreshAction, displaySelectedValueWhenClosed: true, targetRocketSelector);
			if (!targetRocketSelector.IsAtDestination() && launchPadsForDestination.Count > 0)
			{
				launchPadDropDown.openButton.isInteractable = true;
				LaunchPad destinationPad = targetRocketSelector.GetDestinationPad();
				if (destinationPad != null)
				{
					launchPadDropDown.selectedLabel.text = destinationPad.GetProperName();
				}
				else
				{
					launchPadDropDown.selectedLabel.text = UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.FIRSTAVAILABLE;
				}
			}
			else
			{
				launchPadDropDown.selectedLabel.text = UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.FIRSTAVAILABLE;
				launchPadDropDown.openButton.isInteractable = false;
			}
			StyleRepeatButton();
		}
		else
		{
			launchPadDropDown.gameObject.SetActive(value: false);
			repeatButton.gameObject.SetActive(value: false);
		}
		StyleChangeDestinationButton();
	}

	private void OnClickChangeDestination()
	{
		if (targetSelector.assignable)
		{
			ClusterMapScreen.Instance.ShowInSelectDestinationMode(targetSelector);
		}
		StyleChangeDestinationButton();
	}

	private void StyleChangeDestinationButton()
	{
	}

	private void OnClickClearDestination()
	{
		targetSelector.SetDestination(targetSelector.GetMyWorldLocation());
	}

	private void OnLaunchPadEntryClick(IListableOption option, object data)
	{
		LaunchPad destinationPad = (LaunchPad)option;
		targetRocketSelector.SetDestinationPad(destinationPad);
	}

	private void PadDropDownEntryRefreshAction(DropDownEntry entry, object targetData)
	{
		LaunchPad launchPad = (LaunchPad)entry.entryData;
		Clustercraft component = targetRocketSelector.GetComponent<Clustercraft>();
		if (launchPad != null)
		{
			if (component.CanLandAtPad(launchPad, out var failReason) == Clustercraft.PadLandingStatus.CanNeverLand)
			{
				entry.button.isInteractable = false;
				entry.image.sprite = Assets.GetSprite("iconWarning");
				entry.tooltip.SetSimpleTooltip(failReason);
			}
			else
			{
				entry.button.isInteractable = true;
				entry.image.sprite = launchPad.GetComponent<Building>().Def.GetUISprite();
				entry.tooltip.SetSimpleTooltip(string.Format(UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.DROPDOWN_TOOLTIP_VALID_SITE, launchPad.GetProperName()));
			}
		}
		else
		{
			entry.button.isInteractable = true;
			entry.image.sprite = Assets.GetBuildingDef("LaunchPad").GetUISprite();
			entry.tooltip.SetSimpleTooltip(UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.DROPDOWN_TOOLTIP_FIRST_AVAILABLE);
		}
	}

	private int PadDropDownSort(IListableOption a, IListableOption b, object targetData)
	{
		return 0;
	}

	private void OnRepeatClicked()
	{
		targetRocketSelector.Repeat = !targetRocketSelector.Repeat;
		StyleRepeatButton();
	}

	private void StyleRepeatButton()
	{
		repeatButton.bgImage.colorStyleSetting = (targetRocketSelector.Repeat ? repeatOn : repeatOff);
		repeatButton.bgImage.ApplyColorStyleSetting();
	}
}
