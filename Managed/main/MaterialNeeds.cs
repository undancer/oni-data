using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/MaterialNeeds")]
public static class MaterialNeeds
{
	public static void UpdateNeed(Tag tag, float amount, int worldId)
	{
		WorldContainer world = ClusterManager.Instance.GetWorld(worldId);
		if (world != null)
		{
			Dictionary<Tag, float> materialNeeds = world.materialNeeds;
			float value = 0f;
			if (!materialNeeds.TryGetValue(tag, out value))
			{
				materialNeeds[tag] = 0f;
			}
			materialNeeds[tag] = value + amount;
		}
		else
		{
			Debug.LogWarning($"MaterialNeeds.UpdateNeed called with invalid worldId {worldId}");
		}
	}

	public static float GetAmount(Tag tag, int worldId, bool includeRelatedWorlds)
	{
		WorldContainer world = ClusterManager.Instance.GetWorld(worldId);
		float num = 0f;
		if (world != null)
		{
			if (!includeRelatedWorlds)
			{
				float value = 0f;
				Dictionary<Tag, float> materialNeeds = ClusterManager.Instance.GetWorld(worldId).materialNeeds;
				materialNeeds.TryGetValue(tag, out value);
				num += value;
			}
			else
			{
				int parentWorldId = world.ParentWorldId;
				foreach (WorldContainer worldContainer in ClusterManager.Instance.WorldContainers)
				{
					if (worldContainer.ParentWorldId == parentWorldId)
					{
						float value2 = 0f;
						if (worldContainer.materialNeeds.TryGetValue(tag, out value2))
						{
							num += value2;
						}
					}
				}
			}
			return num;
		}
		Debug.LogWarning($"MaterialNeeds.GetAmount called with invalid worldId {worldId}");
		return 0f;
	}
}
