using System.Collections.Generic;
using KSerialization;
using UnityEngine;

public class ArtifactSelector : KMonoBehaviour
{
	public static ArtifactSelector Instance;

	[Serialize]
	private List<string> placedArtifacts = new List<string>();

	private const string DEFAULT_ARTIFACT_ID = "artifact_officemug";

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
	}

	public string GetUniqueArtifactID()
	{
		List<string> list = new List<string>();
		foreach (string artifactItem in ArtifactConfig.artifactItems)
		{
			if (!placedArtifacts.Contains(artifactItem))
			{
				list.Add(artifactItem);
			}
		}
		string text = "artifact_officemug";
		if (list.Count != 0)
		{
			text = list[Random.Range(0, list.Count)];
		}
		placedArtifacts.Add(text);
		return text;
	}

	public void ReserveArtifactID(string artifactID)
	{
		if (placedArtifacts.Contains(artifactID))
		{
			DebugUtil.Assert(test: true, $"Tried to add {artifactID} to placedArtifacts but it already exists in the list!");
		}
		placedArtifacts.Add(artifactID);
	}
}
