using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlatTagFilterSideScreen : SideScreenContent
{
	private FlatTagFilterable tagFilterable;

	[SerializeField]
	private GameObject rowPrefab;

	[SerializeField]
	private GameObject listContainer;

	[SerializeField]
	private LocText headerLabel;

	private Dictionary<Tag, GameObject> rows = new Dictionary<Tag, GameObject>();

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<FlatTagFilterable>() != null;
	}

	public override void SetTarget(GameObject target)
	{
		base.SetTarget(target);
		tagFilterable = target.GetComponent<FlatTagFilterable>();
		Build();
	}

	private void Build()
	{
		headerLabel.SetText(tagFilterable.GetHeaderText());
		foreach (KeyValuePair<Tag, GameObject> row in rows)
		{
			Util.KDestroyGameObject(row.Value);
		}
		rows.Clear();
		foreach (Tag tagOption in tagFilterable.tagOptions)
		{
			GameObject gameObject = Util.KInstantiateUI(rowPrefab, listContainer);
			gameObject.gameObject.name = tagOption.ProperName();
			rows.Add(tagOption, gameObject);
		}
		Refresh();
	}

	private void Refresh()
	{
		foreach (KeyValuePair<Tag, GameObject> kvp in rows)
		{
			kvp.Value.GetComponent<HierarchyReferences>().GetReference<LocText>("Label").SetText(kvp.Key.ProperNameStripLink());
			kvp.Value.GetComponent<HierarchyReferences>().GetReference<Image>("Icon").sprite = Def.GetUISprite(kvp.Key).first;
			kvp.Value.GetComponent<HierarchyReferences>().GetReference<Image>("Icon").color = Def.GetUISprite(kvp.Key).second;
			kvp.Value.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("Toggle").onClick = delegate
			{
				tagFilterable.ToggleTag(kvp.Key);
				Refresh();
			};
			kvp.Value.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("Toggle").ChangeState(tagFilterable.selectedTags.Contains(kvp.Key) ? 1 : 0);
			kvp.Value.SetActive(DiscoveredResources.Instance.IsDiscovered(kvp.Key));
		}
	}
}
