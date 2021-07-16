using System.Collections.Generic;
using KSerialization;
using UnityEngine;

public class ArtifactSelector : KMonoBehaviour
{
	public static ArtifactSelector Instance;

	[Serialize]
	private List<string> placedArtifacts = new List<string>();

	[Serialize]
	private int analyzedArtifactCount;

	[Serialize]
	private int analyzedSpaceArtifactCount;

	[Serialize]
	private List<string> analyzedArtifatIDs = new List<string>();

	private const string DEFAULT_ARTIFACT_ID = "artifact_officemug";

	public int AnalyzedArtifactCount => analyzedArtifactCount;

	public int AnalyzedSpaceArtifactCount => analyzedSpaceArtifactCount;

	public List<string> GetAnalyzedArtifactIDs()
	{
		return analyzedArtifatIDs;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
	}

	public bool RecordArtifactAnalyzed(string id)
	{
		if (analyzedArtifatIDs.Contains(id))
		{
			return false;
		}
		analyzedArtifatIDs.Add(id);
		return true;
	}

	public void IncrementAnalyzedTerrestrialArtifacts()
	{
		analyzedArtifactCount++;
	}

	public void IncrementAnalyzedSpaceArtifacts()
	{
		analyzedSpaceArtifactCount++;
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
