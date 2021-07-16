using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class EntombedDiagnostic : ColonyDiagnostic
{
	private int m_entombedCount;

	public EntombedDiagnostic(int worldID)
		: base(worldID, UI.COLONY_DIAGNOSTICS.ENTOMBEDDIAGNOSTIC.ALL_NAME)
	{
		icon = "icon_action_dig";
		AddCriterion("CheckEntombed", new DiagnosticCriterion(UI.COLONY_DIAGNOSTICS.ENTOMBEDDIAGNOSTIC.CRITERIA.CHECKENTOMBED, CheckEntombed));
	}

	private DiagnosticResult CheckEntombed()
	{
		List<BuildingComplete> worldItems = Components.EntombedBuildings.GetWorldItems(base.worldID);
		m_entombedCount = 0;
		DiagnosticResult result = new DiagnosticResult(DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS)
		{
			opinion = DiagnosticResult.Opinion.Normal,
			Message = UI.COLONY_DIAGNOSTICS.ENTOMBEDDIAGNOSTIC.NORMAL
		};
		foreach (BuildingComplete item in worldItems)
		{
			if (item.HasTag(GameTags.Entombed))
			{
				result.opinion = DiagnosticResult.Opinion.Bad;
				result.Message = UI.COLONY_DIAGNOSTICS.ENTOMBEDDIAGNOSTIC.BUILDING_ENTOMBED;
				result.clickThroughTarget = new Tuple<Vector3, GameObject>(item.gameObject.transform.position, item.gameObject);
				m_entombedCount++;
			}
		}
		return result;
	}

	public override string GetAverageValueString()
	{
		return m_entombedCount.ToString();
	}
}
