using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/InfoDescription")]
public class InfoDescription : KMonoBehaviour
{
	public string nameLocString = "";

	private string descriptionLocString = "";

	public string description;

	public string displayName;

	public string DescriptionLocString
	{
		get
		{
			return descriptionLocString;
		}
		set
		{
			descriptionLocString = value;
			if (descriptionLocString != null)
			{
				description = Strings.Get(descriptionLocString);
			}
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (!string.IsNullOrEmpty(nameLocString))
		{
			displayName = Strings.Get(nameLocString);
		}
		if (!string.IsNullOrEmpty(descriptionLocString))
		{
			description = Strings.Get(descriptionLocString);
		}
	}
}
