using STRINGS;

public class ChoreGroupDiagnostic : ColonyDiagnostic
{
	public ChoreGroup choreGroup;

	public ChoreGroupDiagnostic(int worldID, ChoreGroup choreGroup)
		: base(worldID, UI.COLONY_DIAGNOSTICS.CHOREGROUPDIAGNOSTIC.ALL_NAME)
	{
		this.choreGroup = choreGroup;
		tracker = TrackerTool.Instance.GetChoreGroupTracker(worldID, choreGroup);
		name = choreGroup.Name;
		colors[DiagnosticResult.Opinion.Good] = Constants.NEUTRAL_COLOR;
		id = "ChoreGroupDiagnostic_" + choreGroup.Id;
	}

	public override DiagnosticResult Evaluate()
	{
		DiagnosticResult result = new DiagnosticResult(DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS);
		result.opinion = ((tracker.GetCurrentValue() > 0f) ? DiagnosticResult.Opinion.Good : DiagnosticResult.Opinion.Normal);
		result.Message = string.Format(UI.COLONY_DIAGNOSTICS.ALLCHORESDIAGNOSTIC.NORMAL, tracker.FormatValueString(tracker.GetCurrentValue()));
		return result;
	}
}
