using System;

public class DiagnosticCriterion
{
	private Func<ColonyDiagnostic.DiagnosticResult> evaluateAction;

	public string id
	{
		get;
		private set;
	}

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

	public void SetID(string id)
	{
		this.id = id;
	}

	public ColonyDiagnostic.DiagnosticResult Evaluate()
	{
		return evaluateAction();
	}
}
