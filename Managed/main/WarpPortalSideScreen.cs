using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class WarpPortalSideScreen : SideScreenContent
{
	[SerializeField]
	private LocText label;

	[SerializeField]
	private KButton button;

	[SerializeField]
	private LocText buttonLabel;

	[SerializeField]
	private KButton cancelButton;

	[SerializeField]
	private LocText cancelButtonLabel;

	[SerializeField]
	private WarpPortal target;

	[SerializeField]
	private GameObject contents;

	[SerializeField]
	private GameObject progressBar;

	[SerializeField]
	private LocText progressLabel;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		buttonLabel.SetText(UI.UISIDESCREENS.WARPPORTALSIDESCREEN.BUTTON);
		cancelButtonLabel.SetText(UI.UISIDESCREENS.WARPPORTALSIDESCREEN.CANCELBUTTON);
		button.onClick += OnButtonClick;
		cancelButton.onClick += OnCancelClick;
		Refresh();
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<WarpPortal>() != null;
	}

	public override void SetTarget(GameObject target)
	{
		WarpPortal component = target.GetComponent<WarpPortal>();
		if (component == null)
		{
			Debug.LogError("Target doesn't have a WarpPortal associated with it.");
			return;
		}
		this.target = component;
		target.GetComponent<Assignable>().OnAssign += Refresh;
		Refresh();
	}

	private void Update()
	{
		if (progressBar.activeSelf)
		{
			RectTransform rectTransform = progressBar.GetComponentsInChildren<Image>()[1].rectTransform;
			float num = target.rechargeProgress / 3000f;
			rectTransform.sizeDelta = new Vector2(rectTransform.transform.parent.GetComponent<LayoutElement>().minWidth * num, 24f);
			progressLabel.text = GameUtil.GetFormattedPercent(num * 100f);
		}
	}

	private void OnButtonClick()
	{
		if (target.ReadyToWarp)
		{
			target.StartWarpSequence();
			Refresh();
		}
	}

	private void OnCancelClick()
	{
		target.CancelAssignment();
		Refresh();
	}

	private void Refresh(object data = null)
	{
		progressBar.SetActive(value: false);
		cancelButton.gameObject.SetActive(value: false);
		if (target != null)
		{
			if (target.ReadyToWarp)
			{
				label.text = UI.UISIDESCREENS.WARPPORTALSIDESCREEN.WAITING;
				button.gameObject.SetActive(value: true);
				cancelButton.gameObject.SetActive(value: true);
			}
			else if (target.IsConsumed)
			{
				button.gameObject.SetActive(value: false);
				progressBar.SetActive(value: true);
				label.text = UI.UISIDESCREENS.WARPPORTALSIDESCREEN.CONSUMED;
			}
			else if (target.IsWorking)
			{
				label.text = UI.UISIDESCREENS.WARPPORTALSIDESCREEN.UNDERWAY;
				button.gameObject.SetActive(value: false);
				cancelButton.gameObject.SetActive(value: true);
			}
			else
			{
				label.text = UI.UISIDESCREENS.WARPPORTALSIDESCREEN.IDLE;
				button.gameObject.SetActive(value: false);
			}
		}
		else
		{
			label.text = UI.UISIDESCREENS.WARPPORTALSIDESCREEN.IDLE;
			button.gameObject.SetActive(value: false);
		}
	}
}
