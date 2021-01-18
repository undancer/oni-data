using System;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/ProgressBar")]
public class ProgressBar : KMonoBehaviour
{
	public Image bar;

	private Func<float> updatePercentFull;

	private int overlayUpdateHandle = -1;

	public bool autoHide = true;

	public Color barColor
	{
		get
		{
			return bar.color;
		}
		set
		{
			bar.color = value;
		}
	}

	public float PercentFull
	{
		get
		{
			return bar.fillAmount;
		}
		set
		{
			bar.fillAmount = value;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (autoHide)
		{
			overlayUpdateHandle = Game.Instance.Subscribe(1798162660, OnOverlayChanged);
			if (OverlayScreen.Instance != null && OverlayScreen.Instance.GetMode() != OverlayModes.None.ID)
			{
				base.gameObject.SetActive(value: false);
			}
		}
		base.enabled = updatePercentFull != null;
	}

	public void SetUpdateFunc(Func<float> func)
	{
		updatePercentFull = func;
		base.enabled = updatePercentFull != null;
	}

	public virtual void Update()
	{
		if (updatePercentFull != null)
		{
			PercentFull = updatePercentFull();
		}
	}

	public virtual void OnOverlayChanged(object data = null)
	{
		if (!autoHide)
		{
			return;
		}
		if ((HashedString)data == OverlayModes.None.ID)
		{
			if (!base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(value: true);
			}
		}
		else if (base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	protected override void OnCleanUp()
	{
		if (overlayUpdateHandle != -1)
		{
			Game.Instance.Unsubscribe(overlayUpdateHandle);
		}
		base.OnCleanUp();
	}

	private void OnBecameInvisible()
	{
		base.enabled = false;
	}

	private void OnBecameVisible()
	{
		base.enabled = true;
	}

	public static ProgressBar CreateProgressBar(GameObject entity, Func<float> updateFunc)
	{
		ProgressBar progressBar = Util.KInstantiateUI<ProgressBar>(ProgressBarsConfig.Instance.progressBarPrefab);
		progressBar.SetUpdateFunc(updateFunc);
		progressBar.transform.SetParent(GameScreenManager.Instance.worldSpaceCanvas.transform);
		progressBar.name = ((entity != null) ? (entity.name + "_") : "") + " ProgressBar";
		progressBar.transform.Find("Bar").GetComponent<Image>().color = ProgressBarsConfig.Instance.GetBarColor("ProgressBar");
		progressBar.Update();
		Vector3 vector = entity.transform.GetPosition() + Vector3.down * 0.5f;
		Building component = entity.GetComponent<Building>();
		if (component != null)
		{
			vector = vector - Vector3.right * 0.5f * (component.Def.WidthInCells % 2) + component.Def.placementPivot;
		}
		else
		{
			vector -= Vector3.right * 0.5f;
		}
		progressBar.transform.SetPosition(vector);
		return progressBar;
	}
}
