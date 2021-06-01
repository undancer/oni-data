using System;

public class DiagnosticCriterion
{
	private Func<ColonyDiagnostic.DiagnosticResult> evaluateAction;

	public string name
	{
		get;
		private set;
	}

	public DiagnosticCriterion(string name, Func<ColonyDiagnostic.DiagnosticResult> action)
	{
		this.name = name;
		evaluateAction = action;
	}

	public ColonyDiagnostic.DiagnosticResult Evaluate()
	{
		return evaluateAction();
	}
}
