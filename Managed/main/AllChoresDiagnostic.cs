using STRINGS;

public class AllChoresDiagnostic : ColonyDiagnostic
{
	public AllChoresDiagnostic(int worldID)
		: base(worldID, UI.COLONY_DIAGNOSTICS.ALLCHORESDIAGNOSTIC.ALL_NAME)
	{
		tracker = TrackerTool.Instance.GetWorldTracker<AllChoresCountTracker>(worldID);
		colors[DiagnosticResult.Opinion.Good] = Constants.NEUTRAL_COLOR;
		icon = "icon_errand_operate";
	}

	public override DiagnosticResult Evaluate()
	{
		DiagnosticResult result = new DiagnosticResult(DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS);
		result.opinion = DiagnosticResult.Opinion.Normal;
		result.Message = string.Format(UI.COLONY_DIAGNOSTICS.ALLCHORESDIAGNOSTIC.NORMAL, tracker.FormatValueString(tracker.GetCurrentValue()));
		return result;
	}
}
