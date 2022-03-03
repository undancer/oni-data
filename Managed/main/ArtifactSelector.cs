using System.Collections.Generic;
using KSerialization;
using UnityEngine;

public class ArtifactSelector : KMonoBehaviour
{
	public static ArtifactSelector Instance;

	[Serialize]
	private Dictionary<ArtifactType, List<string>> placedArtifacts = new Dictionary<ArtifactType, List<string>>();

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
		placedArtifacts.Add(ArtifactType.Terrestrial, new List<string>());
		placedArtifacts.Add(ArtifactType.Space, new List<string>());
		placedArtifacts.Add(ArtifactType.Any, new List<string>());
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		int num = 0;
		int num2 = 0;
		foreach (string analyzedArtifatID in analyzedArtifatIDs)
		{
			switch (GetArtifactType(analyzedArtifatID))
			{
			case ArtifactType.Space:
				num2++;
				break;
			case ArtifactType.Terrestrial:
				num++;
				break;
			}
		}
		if (num > analyzedArtifactCount)
		{
			analyzedArtifactCount = num;
		}
		if (num2 > analyzedSpaceArtifactCount)
		{
			analyzedSpaceArtifactCount = num2;
		}
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

	public string GetUniqueArtifactID(ArtifactType artifactType = ArtifactType.Any)
	{
		List<string> list = new List<string>();
		foreach (string item in ArtifactConfig.artifactItems[artifactType])
		{
			if (!placedArtifacts[artifactType].Contains(item))
			{
				list.Add(item);
			}
		}
		string text = "artifact_officemug";
		if (list.Count == 0 && artifactType != ArtifactType.Any)
		{
			foreach (string item2 in ArtifactConfig.artifactItems[ArtifactType.Any])
			{
				if (!placedArtifacts[ArtifactType.Any].Contains(item2))
				{
					list.Add(item2);
					artifactType = ArtifactType.Any;
				}
			}
		}
		if (list.Count != 0)
		{
			text = list[Random.Range(0, list.Count)];
		}
		placedArtifacts[artifactType].Add(text);
		return text;
	}

	public void ReserveArtifactID(string artifactID, ArtifactType artifactType = ArtifactType.Any)
	{
		if (placedArtifacts[artifactType].Contains(artifactID))
		{
			DebugUtil.Assert(test: true, $"Tried to add {artifactID} to placedArtifacts but it already exists in the list!");
		}
		placedArtifacts[artifactType].Add(artifactID);
	}

	public ArtifactType GetArtifactType(string artifactID)
	{
		if (placedArtifacts[ArtifactType.Terrestrial].Contains(artifactID))
		{
			return ArtifactType.Terrestrial;
		}
		if (placedArtifacts[ArtifactType.Space].Contains(artifactID))
		{
			return ArtifactType.Space;
		}
		return ArtifactType.Any;
	}
}
