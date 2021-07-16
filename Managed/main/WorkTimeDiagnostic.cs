using STRINGS;

public class WorkTimeDiagnostic : ColonyDiagnostic
{
	public ChoreGroup choreGroup;

	public WorkTimeDiagnostic(int worldID, ChoreGroup choreGroup)
		: base(worldID, UI.COLONY_DIAGNOSTICS.WORKTIMEDIAGNOSTIC.ALL_NAME)
	{
		this.choreGroup = choreGroup;
		tracker = TrackerTool.Instance.GetWorkTimeTracker(worldID, choreGroup);
		trackerSampleCountSeconds = 100f;
		name = choreGroup.Name;
		id = "WorkTimeDiagnostic_" + choreGroup.Id;
		colors[DiagnosticResult.Opinion.Good] = Constants.NEUTRAL_COLOR;
	}

	public override DiagnosticResult Evaluate()
	{
		DiagnosticResult result = new DiagnosticResult(DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS);
		result.opinion = ((tracker.GetAverageValue(trackerSampleCountSeconds) > 0f) ? DiagnosticResult.Opinion.Good : DiagnosticResult.Opinion.Normal);
		result.Message = string.Format(UI.COLONY_DIAGNOSTICS.ALLWORKTIMEDIAGNOSTIC.NORMAL, tracker.FormatValueString(tracker.GetAverageValue(trackerSampleCountSeconds)));
		return result;
	}
}
