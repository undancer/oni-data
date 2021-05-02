using STRINGS;

public class AllWorkTimeDiagnostic : ColonyDiagnostic
{
	public AllWorkTimeDiagnostic(int worldID)
		: base(worldID, UI.COLONY_DIAGNOSTICS.ALLWORKTIMEDIAGNOSTIC.ALL_NAME)
	{
		tracker = TrackerTool.Instance.GetWorldTracker<AllWorkTimeTracker>(worldID);
		colors[DiagnosticResult.Opinion.Good] = Constants.NEUTRAL_COLOR;
	}

	public override DiagnosticResult Evaluate()
	{
		DiagnosticResult result = new DiagnosticResult(DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS);
		result.opinion = DiagnosticResult.Opinion.Normal;
		result.Message = string.Format(UI.COLONY_DIAGNOSTICS.ALLWORKTIMEDIAGNOSTIC.NORMAL, tracker.FormatValueString(tracker.GetCurrentValue()));
		return result;
	}
}
