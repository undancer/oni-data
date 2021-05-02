using System;

public class DiagnosticCriterion
{
	private string id;

	private Func<ColonyDiagnostic.DiagnosticResult> action;

	public DiagnosticCriterion(Func<ColonyDiagnostic.DiagnosticResult> action)
	{
		this.action = action;
	}

	public void SetName(string id)
	{
		this.id = id;
	}

	public ColonyDiagnostic.DiagnosticResult Evaluate()
	{
		return action();
	}
}
