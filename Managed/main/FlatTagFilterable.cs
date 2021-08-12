using System.Collections.Generic;
using KSerialization;

public class FlatTagFilterable : KMonoBehaviour
{
	[Serialize]
	public List<Tag> selectedTags = new List<Tag>();

	public List<Tag> tagOptions = new List<Tag>();

	public string headerText;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		TreeFilterable component = GetComponent<TreeFilterable>();
		component.filterByStorageCategoriesOnSpawn = false;
		component.UpdateFilters(selectedTags);
	}

	public void SelectTag(Tag tag, bool state)
	{
		Debug.Assert(tagOptions.Contains(tag), "The tag " + tag.Name + " is not valid for this filterable - it must be added to tagOptions");
		if (state)
		{
			if (!selectedTags.Contains(tag))
			{
				selectedTags.Add(tag);
			}
		}
		else if (selectedTags.Contains(tag))
		{
			selectedTags.Remove(tag);
		}
		GetComponent<TreeFilterable>().UpdateFilters(selectedTags);
	}

	public void ToggleTag(Tag tag)
	{
		SelectTag(tag, !selectedTags.Contains(tag));
	}

	public string GetHeaderText()
	{
		return headerText;
	}
}
