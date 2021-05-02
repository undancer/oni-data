using System.Collections.Generic;
using UnityEngine;

public class ClusterNameDisplayScreen : KScreen
{
	private class Entry
	{
		public string Name;

		public ClusterGridEntity grid_entity;

		public GameObject display_go;

		public GameObject bars_go;

		public HierarchyReferences refs;
	}

	public static ClusterNameDisplayScreen Instance;

	public GameObject nameAndBarsPrefab;

	[SerializeField]
	private Color selectedColor;

	[SerializeField]
	private Color defaultColor;

	private List<Entry> m_entries = new List<Entry>();

	private List<KCollider2D> workingList = new List<KCollider2D>();

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	public void AddNewEntry(ClusterGridEntity representedObject)
	{
		if (GetEntry(representedObject) == null)
		{
			Entry entry = new Entry();
			entry.grid_entity = representedObject;
			GameObject original = nameAndBarsPrefab;
			GameObject gameObject = (entry.display_go = Util.KInstantiateUI(original, base.gameObject, force_active: true));
			gameObject.name = representedObject.name + " cluster overlay";
			entry.Name = representedObject.name;
			entry.refs = gameObject.GetComponent<HierarchyReferences>();
			entry.bars_go = entry.refs.GetReference<RectTransform>("Bars").gameObject;
			m_entries.Add(entry);
			KSelectable component = representedObject.GetComponent<KSelectable>();
			if (component != null)
			{
				UpdateName(representedObject);
				UpdateBars(representedObject);
			}
		}
	}

	private void LateUpdate()
	{
		if (App.isLoading || App.IsExiting)
		{
			return;
		}
		int num = m_entries.Count;
		int num2 = 0;
		while (num2 < num)
		{
			if (m_entries[num2].grid_entity != null && ClusterMapScreen.GetRevealLevel(m_entries[num2].grid_entity) == ClusterRevealLevel.Visible && m_entries[num2].grid_entity.ShowName())
			{
				Transform gridEntityNameTarget = ClusterMapScreen.Instance.GetGridEntityNameTarget(m_entries[num2].grid_entity);
				if (gridEntityNameTarget != null)
				{
					Vector3 position = gridEntityNameTarget.GetPosition();
					RectTransform component = m_entries[num2].display_go.GetComponent<RectTransform>();
					component.SetPositionAndRotation(position, Quaternion.identity);
					m_entries[num2].display_go.SetActive(m_entries[num2].grid_entity.IsVisible);
				}
				else if (m_entries[num2].display_go.activeSelf)
				{
					m_entries[num2].display_go.SetActive(value: false);
				}
				UpdateBars(m_entries[num2].grid_entity);
				if (m_entries[num2].bars_go != null)
				{
					GameObject bars_go = m_entries[num2].bars_go;
					bars_go.GetComponentsInChildren(includeInactive: false, workingList);
					foreach (KCollider2D working in workingList)
					{
						working.MarkDirty();
					}
				}
				num2++;
			}
			else
			{
				Object.Destroy(m_entries[num2].display_go);
				num--;
				m_entries[num2] = m_entries[num];
			}
		}
		m_entries.RemoveRange(num, m_entries.Count - num);
	}

	private void UpdateName(ClusterGridEntity representedObject)
	{
		Entry entry = GetEntry(representedObject);
		if (entry != null)
		{
			KSelectable component = representedObject.GetComponent<KSelectable>();
			entry.display_go.name = component.GetProperName() + " cluster overlay";
			LocText componentInChildren = entry.display_go.GetComponentInChildren<LocText>();
			if (componentInChildren != null)
			{
				componentInChildren.text = component.GetProperName();
			}
		}
	}

	private void UpdateBars(ClusterGridEntity representedObject)
	{
		Entry entry = GetEntry(representedObject);
		if (entry == null)
		{
			return;
		}
		GenericUIProgressBar componentInChildren = entry.bars_go.GetComponentInChildren<GenericUIProgressBar>(includeInactive: true);
		if (entry.grid_entity.ShowProgressBar())
		{
			if (!componentInChildren.gameObject.activeSelf)
			{
				componentInChildren.gameObject.SetActive(value: true);
			}
			componentInChildren.SetFillPercentage(entry.grid_entity.GetProgress());
		}
		else if (componentInChildren.gameObject.activeSelf)
		{
			componentInChildren.gameObject.SetActive(value: false);
		}
	}

	private Entry GetEntry(ClusterGridEntity entity)
	{
		return m_entries.Find((Entry entry) => entry.grid_entity == entity);
	}
}
