using System;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/KSelectable")]
public class KSelectable : KMonoBehaviour
{
	private const float hoverHighlight = 0.25f;

	private const float selectHighlight = 0.2f;

	public string entityName;

	public string entityGender;

	private bool selected = false;

	[SerializeField]
	private bool selectable = true;

	[SerializeField]
	private bool disableSelectMarker;

	private StatusItemGroup statusItemGroup;

	public bool IsSelected => selected;

	public bool IsSelectable
	{
		get
		{
			return selectable && base.isActiveAndEnabled;
		}
		set
		{
			selectable = value;
		}
	}

	public bool DisableSelectMarker => disableSelectMarker;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		statusItemGroup = new StatusItemGroup(base.gameObject);
		KPrefabID component = GetComponent<KPrefabID>();
		if (component != null)
		{
		}
		if (entityName == null || entityName.Length <= 0)
		{
			SetName(base.name);
		}
		if (entityGender == null)
		{
			entityGender = "NB";
		}
	}

	public virtual string GetName()
	{
		if (entityName == null || entityName == "" || entityName.Length <= 0)
		{
			Debug.Log("Warning Item has blank name!", base.gameObject);
			return base.name;
		}
		return entityName;
	}

	public void SetStatusIndicatorOffset(Vector3 offset)
	{
		if (statusItemGroup != null)
		{
			statusItemGroup.SetOffset(offset);
		}
	}

	public void SetName(string name)
	{
		entityName = name;
	}

	public void SetGender(string Gender)
	{
		entityGender = Gender;
	}

	public float GetZoom()
	{
		float num = 1f;
		Bounds bounds = Util.GetBounds(base.gameObject);
		return 1.05f * Mathf.Max(bounds.extents.x, bounds.extents.y);
	}

	public Vector3 GetPortraitLocation()
	{
		Vector3 vector = default(Vector3);
		return Util.GetBounds(base.gameObject).center;
	}

	private void ClearHighlight()
	{
		Trigger(-1201923725, false);
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		if (component != null)
		{
			component.HighlightColour = new Color(0f, 0f, 0f, 0f);
		}
	}

	private void ApplyHighlight(float highlight)
	{
		Trigger(-1201923725, true);
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		if (component != null)
		{
			component.HighlightColour = new Color(highlight, highlight, highlight, highlight);
		}
	}

	public void Select()
	{
		selected = true;
		ClearHighlight();
		ApplyHighlight(0.2f);
		Trigger(-1503271301, true);
		if (GetComponent<LoopingSounds>() != null)
		{
			GetComponent<LoopingSounds>().UpdateObjectSelection(selected);
		}
		if (base.transform.GetComponentInParent<LoopingSounds>() != null)
		{
			base.transform.GetComponentInParent<LoopingSounds>().UpdateObjectSelection(selected);
		}
		int childCount = base.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			int childCount2 = base.transform.GetChild(i).childCount;
			for (int j = 0; j < childCount2; j++)
			{
				if (base.transform.GetChild(i).transform.GetChild(j).GetComponent<LoopingSounds>() != null)
				{
					base.transform.GetChild(i).transform.GetChild(j).GetComponent<LoopingSounds>().UpdateObjectSelection(selected);
				}
			}
		}
		UpdateWorkerSelection(selected);
		UpdateWorkableSelection(selected);
	}

	public void Unselect()
	{
		if (selected)
		{
			selected = false;
			ClearHighlight();
			Trigger(-1503271301, false);
		}
		if (GetComponent<LoopingSounds>() != null)
		{
			GetComponent<LoopingSounds>().UpdateObjectSelection(selected);
		}
		if (base.transform.GetComponentInParent<LoopingSounds>() != null)
		{
			base.transform.GetComponentInParent<LoopingSounds>().UpdateObjectSelection(selected);
		}
		foreach (Transform item in base.transform)
		{
			if (item.GetComponent<LoopingSounds>() != null)
			{
				item.GetComponent<LoopingSounds>().UpdateObjectSelection(selected);
			}
		}
		UpdateWorkerSelection(selected);
		UpdateWorkableSelection(selected);
	}

	public void Hover(bool playAudio)
	{
		ClearHighlight();
		if (!DebugHandler.HideUI)
		{
			ApplyHighlight(0.25f);
		}
		if (playAudio)
		{
			PlayHoverSound();
		}
	}

	private void PlayHoverSound()
	{
		if (!(GetComponent<CellSelectionObject>() != null))
		{
			UISounds.PlaySound(UISounds.Sound.Object_Mouseover);
		}
	}

	public void Unhover()
	{
		if (!selected)
		{
			ClearHighlight();
		}
	}

	public Guid ToggleStatusItem(StatusItem status_item, bool on, object data = null)
	{
		if (on)
		{
			return AddStatusItem(status_item, data);
		}
		return RemoveStatusItem(status_item);
	}

	public Guid ToggleStatusItem(StatusItem status_item, Guid guid, bool show, object data = null)
	{
		if (show)
		{
			if (guid != Guid.Empty)
			{
				return guid;
			}
			return AddStatusItem(status_item, data);
		}
		if (guid != Guid.Empty)
		{
			return RemoveStatusItem(guid);
		}
		return guid;
	}

	public Guid SetStatusItem(StatusItemCategory category, StatusItem status_item, object data = null)
	{
		if (statusItemGroup == null)
		{
			return Guid.Empty;
		}
		return statusItemGroup.SetStatusItem(category, status_item, data);
	}

	public Guid ReplaceStatusItem(Guid guid, StatusItem status_item, object data = null)
	{
		if (statusItemGroup == null)
		{
			return Guid.Empty;
		}
		if (guid != Guid.Empty)
		{
			statusItemGroup.RemoveStatusItem(guid);
		}
		return AddStatusItem(status_item, data);
	}

	public Guid AddStatusItem(StatusItem status_item, object data = null)
	{
		if (statusItemGroup == null)
		{
			return Guid.Empty;
		}
		return statusItemGroup.AddStatusItem(status_item, data);
	}

	public Guid RemoveStatusItem(StatusItem status_item, bool immediate = false)
	{
		if (statusItemGroup == null)
		{
			return Guid.Empty;
		}
		statusItemGroup.RemoveStatusItem(status_item, immediate);
		return Guid.Empty;
	}

	public Guid RemoveStatusItem(Guid guid, bool immediate = false)
	{
		if (statusItemGroup == null)
		{
			return Guid.Empty;
		}
		statusItemGroup.RemoveStatusItem(guid, immediate);
		return Guid.Empty;
	}

	public bool HasStatusItem(StatusItem status_item)
	{
		if (statusItemGroup == null)
		{
			return false;
		}
		return statusItemGroup.HasStatusItem(status_item);
	}

	public StatusItemGroup.Entry GetStatusItem(StatusItemCategory category)
	{
		return statusItemGroup.GetStatusItem(category);
	}

	public StatusItemGroup GetStatusItemGroup()
	{
		return statusItemGroup;
	}

	public void UpdateWorkerSelection(bool selected)
	{
		Workable[] components = GetComponents<Workable>();
		if (components.Length == 0)
		{
			return;
		}
		for (int i = 0; i < components.Length; i++)
		{
			if (components[i].worker != null && components[i].GetComponent<LoopingSounds>() != null)
			{
				components[i].GetComponent<LoopingSounds>().UpdateObjectSelection(selected);
			}
		}
	}

	public void UpdateWorkableSelection(bool selected)
	{
		Worker component = GetComponent<Worker>();
		if (component != null && component.workable != null)
		{
			Workable workable = GetComponent<Worker>().workable;
			if (workable.GetComponent<LoopingSounds>() != null)
			{
				workable.GetComponent<LoopingSounds>().UpdateObjectSelection(selected);
			}
		}
	}

	protected override void OnLoadLevel()
	{
		OnCleanUp();
		base.OnLoadLevel();
	}

	protected override void OnCleanUp()
	{
		if (statusItemGroup != null)
		{
			statusItemGroup.Destroy();
			statusItemGroup = null;
		}
		if (selected && SelectTool.Instance != null)
		{
			if (SelectTool.Instance.selected == this)
			{
				SelectTool.Instance.Select(null, skipSound: true);
			}
			else
			{
				Unselect();
			}
		}
		base.OnCleanUp();
	}
}
