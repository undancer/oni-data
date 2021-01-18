using System.Collections.Generic;
using UnityEngine;

public class HighEnergyParticleDirectionSideScreen : SideScreenContent
{
	private IHighEnergyParticleDirection target;

	public List<KButton> Buttons;

	private KButton activeButton = null;

	public override string GetTitle()
	{
		return Strings.Get("STRINGS.BUILDINGS.PREFABS.HIGHENERGYPARTICLEREDIRECTOR.NAME");
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
					Game.Instance.ForceOverlayUpdate();
				}
			};
		}
	}

	public override bool IsValidForTarget(GameObject target)
	{
		bool flag = target.GetComponent<HighEnergyParticleRedirector>() != null;
		bool flag2 = target.GetComponent<HighEnergyParticleSpawner>() != null;
		return (flag || flag2) && target.GetComponent<IHighEnergyParticleDirection>() != null;
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
			return;
		}
		int directionIndex = EightDirectionUtil.GetDirectionIndex(target.Direction);
		if (directionIndex >= 0 && directionIndex < Buttons.Count)
		{
			KButton kButton = Buttons[directionIndex];
			kButton.SignalClick(KKeyCode.Mouse0);
			return;
		}
		if ((bool)activeButton)
		{
			activeButton.isInteractable = true;
		}
		activeButton = null;
	}
}
