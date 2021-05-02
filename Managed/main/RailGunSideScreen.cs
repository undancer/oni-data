using STRINGS;
using UnityEngine;

public class RailGunSideScreen : SideScreenContent
{
	public GameObject content;

	private RailGun selectedGun;

	public LocText DescriptionText;

	[Header("Slider")]
	[SerializeField]
	private KSlider slider;

	[Header("Number Input")]
	[SerializeField]
	private KNumberInputField numberInput;

	[SerializeField]
	private LocText unitsLabel;

	[SerializeField]
	private LocText hepStorageInfo;

	private int targetRailgunHEPStorageSubHandle = -1;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		unitsLabel.text = GameUtil.GetCurrentMassUnit();
		slider.onDrag += delegate
		{
			ReceiveValueFromSlider(slider.value);
		};
		slider.onPointerDown += delegate
		{
			ReceiveValueFromSlider(slider.value);
		};
		slider.onMove += delegate
		{
			ReceiveValueFromSlider(slider.value);
		};
		numberInput.onEndEdit += delegate
		{
			ReceiveValueFromInput(numberInput.currentValue);
		};
		numberInput.decimalPlaces = 1;
	}

	protected override void OnCmpDisable()
	{
		base.OnCmpDisable();
		if ((bool)selectedGun)
		{
			selectedGun = null;
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if ((bool)selectedGun)
		{
			selectedGun = null;
		}
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<RailGun>() != null;
	}

	public override void SetTarget(GameObject new_target)
	{
		if (new_target == null)
		{
			Debug.LogError("Invalid gameObject received");
			return;
		}
		selectedGun = new_target.GetComponent<RailGun>();
		if (selectedGun == null)
		{
			Debug.LogError("The gameObject received does not contain a RailGun component");
			return;
		}
		if (targetRailgunHEPStorageSubHandle != -1)
		{
			Unsubscribe(targetRailgunHEPStorageSubHandle);
		}
		targetRailgunHEPStorageSubHandle = selectedGun.Subscribe(-1837862626, UpdateHEPLabels);
		slider.minValue = selectedGun.MinLaunchMass;
		slider.maxValue = selectedGun.MaxLaunchMass;
		slider.value = selectedGun.launchMass;
		unitsLabel.text = GameUtil.GetCurrentMassUnit();
		numberInput.minValue = selectedGun.MinLaunchMass;
		numberInput.maxValue = selectedGun.MaxLaunchMass;
		numberInput.currentValue = Mathf.Max(selectedGun.MinLaunchMass, Mathf.Min(selectedGun.MaxLaunchMass, selectedGun.launchMass));
		UpdateMaxCapacityLabel();
		numberInput.Activate();
		UpdateHEPLabels();
	}

	public void UpdateHEPLabels(object data = null)
	{
		if (!(selectedGun == null))
		{
			string text = BUILDINGS.PREFABS.RAILGUN.SIDESCREEN_HEP_REQUIRED;
			text = text.Replace("{current}", selectedGun.CurrentEnergy.ToString());
			text = text.Replace("{required}", selectedGun.EnergyCost.ToString());
			hepStorageInfo.text = text;
		}
	}

	private void ReceiveValueFromSlider(float newValue)
	{
		UpdateMaxCapacity(newValue);
	}

	private void ReceiveValueFromInput(float newValue)
	{
		UpdateMaxCapacity(newValue);
	}

	private void UpdateMaxCapacity(float newValue)
	{
		selectedGun.launchMass = newValue;
		slider.value = newValue;
		UpdateMaxCapacityLabel();
	}

	private void UpdateMaxCapacityLabel()
	{
		numberInput.SetDisplayValue(selectedGun.launchMass.ToString());
	}
}
