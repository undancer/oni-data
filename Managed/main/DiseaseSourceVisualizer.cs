using System;
using Klei.AI;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/DiseaseSourceVisualizer")]
public class DiseaseSourceVisualizer : KMonoBehaviour
{
	[SerializeField]
	private Vector3 offset;

	private GameObject visualizer;

	private bool visible;

	public string alwaysShowDisease;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		UpdateVisibility();
		Components.DiseaseSourceVisualizers.Add(this);
	}

	protected override void OnCleanUp()
	{
		OverlayScreen instance = OverlayScreen.Instance;
		instance.OnOverlayChanged = (Action<HashedString>)Delegate.Remove(instance.OnOverlayChanged, new Action<HashedString>(OnViewModeChanged));
		base.OnCleanUp();
		Components.DiseaseSourceVisualizers.Remove(this);
		if (visualizer != null)
		{
			UnityEngine.Object.Destroy(visualizer);
			visualizer = null;
		}
	}

	private void CreateVisualizer()
	{
		if (!(visualizer != null) && !(GameScreenManager.Instance.worldSpaceCanvas == null))
		{
			visualizer = Util.KInstantiate(Assets.UIPrefabs.ResourceVisualizer, GameScreenManager.Instance.worldSpaceCanvas);
		}
	}

	public void UpdateVisibility()
	{
		CreateVisualizer();
		if (string.IsNullOrEmpty(alwaysShowDisease))
		{
			visible = false;
		}
		else
		{
			Disease disease = Db.Get().Diseases.Get(alwaysShowDisease);
			if (disease != null)
			{
				SetVisibleDisease(disease);
			}
		}
		if (OverlayScreen.Instance != null)
		{
			Show(OverlayScreen.Instance.GetMode());
		}
	}

	private void SetVisibleDisease(Disease disease)
	{
		Sprite overlaySprite = Assets.instance.DiseaseVisualization.overlaySprite;
		Color32 colorByName = GlobalAssets.Instance.colorSet.GetColorByName(disease.overlayColourName);
		Image component = visualizer.transform.GetChild(0).GetComponent<Image>();
		component.sprite = overlaySprite;
		component.color = colorByName;
		visible = true;
	}

	private void Update()
	{
		if (!(visualizer == null))
		{
			visualizer.transform.SetPosition(base.transform.GetPosition() + offset);
		}
	}

	private void OnViewModeChanged(HashedString mode)
	{
		Show(mode);
	}

	public void Show(HashedString mode)
	{
		base.enabled = visible && mode == OverlayModes.Disease.ID;
		if (visualizer != null)
		{
			visualizer.SetActive(base.enabled);
		}
	}
}
