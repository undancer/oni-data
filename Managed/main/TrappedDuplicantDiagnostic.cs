using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class TrappedDuplicantDiagnostic : ColonyDiagnostic
{
	public TrappedDuplicantDiagnostic(int worldID)
		: base(worldID, UI.COLONY_DIAGNOSTICS.TRAPPEDDUPLICANTDIAGNOSTIC.ALL_NAME)
	{
		icon = "overlay_power";
		AddCriterion("CheckTrapped", new DiagnosticCriterion(CheckTrapped));
	}

	public DiagnosticResult CheckTrapped()
	{
		DiagnosticResult result = new DiagnosticResult(DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS);
		bool flag = false;
		foreach (MinionIdentity worldItem in Components.LiveMinionIdentities.GetWorldItems(base.worldID))
		{
			if (flag)
			{
				break;
			}
			if (ClusterManager.Instance.GetWorld(base.worldID).IsModuleInterior || !CheckMinionBasicallyIdle(worldItem))
			{
				continue;
			}
			Navigator component = worldItem.GetComponent<Navigator>();
			bool flag2 = true;
			foreach (MinionIdentity worldItem2 in Components.LiveMinionIdentities.GetWorldItems(base.worldID))
			{
				if (worldItem == worldItem2 || CheckMinionBasicallyIdle(worldItem2) || !component.CanReach(worldItem2.GetComponent<IApproachable>()))
				{
					continue;
				}
				flag2 = false;
				break;
			}
			List<Telepad> worldItems = Components.Telepads.GetWorldItems(component.GetMyWorld().id);
			if (worldItems != null && worldItems.Count > 0)
			{
				flag2 = flag2 && !component.CanReach(worldItems[0].GetComponent<IApproachable>());
			}
			List<WarpReceiver> worldItems2 = Components.WarpReceivers.GetWorldItems(component.GetMyWorld().id);
			if (worldItems2 != null && worldItems2.Count > 0)
			{
				foreach (WarpReceiver item in worldItems2)
				{
					flag2 = flag2 && !component.CanReach(worldItems2[0].GetComponent<IApproachable>());
				}
			}
			List<Sleepable> worldItems3 = Components.Sleepables.GetWorldItems(component.GetMyWorld().id);
			Assignable assignable = null;
			for (int i = 0; i < worldItems3.Count; i++)
			{
				assignable = worldItems3[i].GetComponent<Assignable>();
				if (assignable != null && assignable.IsAssignedTo(worldItem))
				{
					flag2 = flag2 && !component.CanReach(worldItems3[i].GetComponent<IApproachable>());
				}
			}
			if (flag2)
			{
				result.clickThroughTarget = new Tuple<Vector3, GameObject>(worldItem.transform.position, worldItem.gameObject);
			}
			flag = flag || flag2;
		}
		result.opinion = (flag ? DiagnosticResult.Opinion.Bad : DiagnosticResult.Opinion.Normal);
		result.Message = (flag ? UI.COLONY_DIAGNOSTICS.TRAPPEDDUPLICANTDIAGNOSTIC.STUCK : UI.COLONY_DIAGNOSTICS.TRAPPEDDUPLICANTDIAGNOSTIC.NORMAL);
		return result;
	}

	private bool CheckMinionBasicallyIdle(MinionIdentity minion)
	{
		if (minion.HasTag(GameTags.Idle) || minion.HasTag(GameTags.RecoveringBreath) || minion.HasTag(GameTags.MakingMess))
		{
			return true;
		}
		return false;
	}
}
