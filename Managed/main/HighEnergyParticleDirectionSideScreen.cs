using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class HighEnergyParticleDirectionSideScreen : SideScreenContent
{
	private IHighEnergyParticleDirection target;

	public List<KButton> Buttons;

	private KButton activeButton;

	public LocText directionLabel;

	private string[] directionStrings = new string[8]
	{
		UI.UISIDESCREENS.HIGHENERGYPARTICLEDIRECTIONSIDESCREEN.DIRECTION_N,
		UI.UISIDESCREENS.HIGHENERGYPARTICLEDIRECTIONSIDESCREEN.DIRECTION_NW,
		UI.UISIDESCREENS.HIGHENERGYPARTICLEDIRECTIONSIDESCREEN.DIRECTION_W,
		UI.UISIDESCREENS.HIGHENERGYPARTICLEDIRECTIONSIDESCREEN.DIRECTION_SW,
		UI.UISIDESCREENS.HIGHENERGYPARTICLEDIRECTIONSIDESCREEN.DIRECTION_S,
		UI.UISIDESCREENS.HIGHENERGYPARTICLEDIRECTIONSIDESCREEN.DIRECTION_SE,
		UI.UISIDESCREENS.HIGHENERGYPARTICLEDIRECTIONSIDESCREEN.DIRECTION_E,
		UI.UISIDESCREENS.HIGHENERGYPARTICLEDIRECTIONSIDESCREEN.DIRECTION_NE
	};

	public override string GetTitle()
	{
		return UI.UISIDESCREENS.HIGHENERGYPARTICLEDIRECTIONSIDESCREEN.TITLE;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		for (int i = 0; i < Buttons.Count; i++)
		{
			KButton button = Buttons[i];
			button.onClick += delegate
			{
				int num = Buttons.IndexOf(button);
				if (activeButton != null)
				{
					activeButton.isInteractable = true;
				}
				button.isInteractable = false;
				activeButton = button;
				if (target != null)
				{
					target.Direction = EightDirectionUtil.AngleToDirection(num * 45);
					Game.Instance.ForceOverlayUpdate(clearLastMode: true);
					Refresh();
				}
			};
		}
	}

	public override int GetSideScreenSortOrder()
	{
		return 10;
	}

	public override bool IsValidForTarget(GameObject target)
	{
		HighEnergyParticleRedirector component = target.GetComponent<HighEnergyParticleRedirector>();
		bool flag = component != null;
		if (flag)
		{
			flag = flag && component.directionControllable;
		}
		bool flag2 = target.GetComponent<HighEnergyParticleSpawner>() != null;
		if (flag || flag2)
		{
			return target.GetComponent<IHighEnergyParticleDirection>() != null;
		}
		return false;
	}

	public override void SetTarget(GameObject new_target)
	{
		if (new_target == null)
		{
			Debug.LogError("Invalid gameObject received");
			return;
		}
		target = new_target.GetComponent<IHighEnergyParticleDirection>();
		if (target == null)
		{
			Debug.LogError("The gameObject received does not contain IHighEnergyParticleDirection component");
		}
		else
		{
			Refresh();
		}
	}

	private void Refresh()
	{
		int directionIndex = EightDirectionUtil.GetDirectionIndex(target.Direction);
		if (directionIndex >= 0 && directionIndex < Buttons.Count)
		{
			Buttons[directionIndex].SignalClick(KKeyCode.Mouse0);
		}
		else
		{
			if ((bool)activeButton)
			{
				activeButton.isInteractable = true;
			}
			activeButton = null;
		}
		directionLabel.SetText(string.Format(UI.UISIDESCREENS.HIGHENERGYPARTICLEDIRECTIONSIDESCREEN.SELECTED_DIRECTION, directionStrings[directionIndex]));
	}
}
