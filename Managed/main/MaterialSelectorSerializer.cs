using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/MaterialSelectorSerializer")]
public class MaterialSelectorSerializer : KMonoBehaviour
{
	[Serialize]
	private List<Dictionary<Tag, Tag>> previouslySelectedElements;

	[Serialize]
	private List<Dictionary<Tag, Tag>>[] previouslySelectedElementsPerWorld;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (previouslySelectedElementsPerWorld != null)
		{
			return;
		}
		previouslySelectedElementsPerWorld = new List<Dictionary<Tag, Tag>>[ClusterManager.INVALID_WORLD_IDX];
		if (previouslySelectedElements == null)
		{
			return;
		}
		foreach (WorldContainer worldContainer in ClusterManager.Instance.WorldContainers)
		{
			List<Dictionary<Tag, Tag>> list = previouslySelectedElements.ConvertAll((Dictionary<Tag, Tag> input) => new Dictionary<Tag, Tag>(input));
			previouslySelectedElementsPerWorld[worldContainer.id] = list;
		}
		previouslySelectedElements = null;
	}

	public void WipeWorldSelectionData(int worldID)
	{
		previouslySelectedElementsPerWorld[worldID] = null;
	}

	public void SetSelectedElement(int worldID, int selectorIndex, Tag recipe, Tag element)
	{
		if (previouslySelectedElementsPerWorld[worldID] == null)
		{
			previouslySelectedElementsPerWorld[worldID] = new List<Dictionary<Tag, Tag>>();
		}
		List<Dictionary<Tag, Tag>> list = previouslySelectedElementsPerWorld[worldID];
		while (list.Count <= selectorIndex)
		{
			list.Add(new Dictionary<Tag, Tag>());
		}
		list[selectorIndex][recipe] = element;
	}

	public Tag GetPreviousElement(int worldID, int selectorIndex, Tag recipe)
	{
		Tag value = Tag.Invalid;
		if (previouslySelectedElementsPerWorld[worldID] == null)
		{
			return value;
		}
		List<Dictionary<Tag, Tag>> list = previouslySelectedElementsPerWorld[worldID];
		if (list.Count <= selectorIndex)
		{
			return value;
		}
		list[selectorIndex].TryGetValue(recipe, out value);
		return value;
	}
}
