using STRINGS;
using UnityEngine;

public class GeneticAnalysisStationSideScreen : SideScreenContent
{
	[SerializeField]
	private LocText label;

	[SerializeField]
	private KButton button;

	[SerializeField]
	private LocText buttonLabel;

	[SerializeField]
	private GeneticAnalysisStation target;

	[SerializeField]
	private GameObject contents;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		button.onClick += OnButtonClick;
		Refresh();
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<GeneticAnalysisStation>() != null;
	}

	public override void SetTarget(GameObject target)
	{
		GeneticAnalysisStation component = target.GetComponent<GeneticAnalysisStation>();
		if (component == null)
		{
			Debug.LogError("Target doesn't have a GeneticAnalysisStation associated with it.");
			return;
		}
		this.target = component;
		Refresh();
	}

	private void OnButtonClick()
	{
		if (target != null)
		{
			target.IdentifyMutant();
		}
	}

	private void Refresh()
	{
		if (target != null)
		{
			if (target.WorkComplete)
			{
				contents.SetActive(value: true);
				label.text = UI.UISIDESCREENS.GENESHUFFLERSIDESREEN.COMPLETE;
				button.gameObject.SetActive(value: true);
				buttonLabel.text = UI.UISIDESCREENS.GENESHUFFLERSIDESREEN.BUTTON;
			}
			else
			{
				contents.SetActive(value: false);
			}
		}
		else
		{
			contents.SetActive(value: false);
		}
	}
}
