using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public abstract class ColonyDiagnostic
{
	public enum PresentationSetting
	{
		AverageValue,
		CurrentValue
	}

	public struct DiagnosticResult
	{
		public enum Opinion
		{
			Unset,
			DuplicantThreatening,
			Bad,
			Warning,
			Concern,
			Suggestion,
			Tutorial,
			Normal,
			Good
		}

		public Opinion opinion;

		public Tuple<Vector3, GameObject> clickThroughTarget;

		private string message;

		public string Message
		{
			get
			{
				string text = "";
				return opinion switch
				{
					Opinion.Bad => "<color=" + Constants.NEGATIVE_COLOR_STR + ">" + message + "</color>", 
					Opinion.Warning => "<color=" + Constants.NEGATIVE_COLOR_STR + ">" + message + "</color>", 
					Opinion.Concern => "<color=" + Constants.WARNING_COLOR_STR + ">" + message + "</color>", 
					Opinion.Normal => message, 
					Opinion.Good => string.Concat("<color=", Constants.POSITIVE_COLOR, ">", message, "</color>"), 
					_ => message, 
				};
			}
			set
			{
				message = value;
			}
		}

		public DiagnosticResult(Opinion opinion, string message, Tuple<Vector3, GameObject> clickThroughTarget = null)
		{
			this.message = message;
			this.opinion = opinion;
			this.clickThroughTarget = null;
		}
	}

	public string name;

	public string id;

	public string icon = "icon_errand_operate";

	private Dictionary<string, DiagnosticCriterion> criteria = new Dictionary<string, DiagnosticCriterion>();

	public PresentationSetting presentationSetting = PresentationSetting.AverageValue;

	private DiagnosticResult latestResult = new DiagnosticResult(DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.NO_DATA);

	public Dictionary<DiagnosticResult.Opinion, Color> colors = new Dictionary<DiagnosticResult.Opinion, Color>();

	public Tracker tracker;

	protected float trackerSampleCountSeconds = 4f;

	public int worldID
	{
		get;
		protected set;
	}

	public DiagnosticResult LatestResult
	{
		get
		{
			return latestResult;
		}
		private set
		{
			latestResult = value;
		}
	}

	public ColonyDiagnostic(int worldID, string name)
	{
		this.worldID = worldID;
		this.name = name;
		id = GetType().Name;
		colors = new Dictionary<DiagnosticResult.Opinion, Color>();
		colors.Add(DiagnosticResult.Opinion.DuplicantThreatening, Constants.NEGATIVE_COLOR);
		colors.Add(DiagnosticResult.Opinion.Bad, Constants.NEGATIVE_COLOR);
		colors.Add(DiagnosticResult.Opinion.Warning, Constants.NEGATIVE_COLOR);
		colors.Add(DiagnosticResult.Opinion.Concern, Constants.WARNING_COLOR);
		colors.Add(DiagnosticResult.Opinion.Normal, Constants.NEUTRAL_COLOR);
		colors.Add(DiagnosticResult.Opinion.Suggestion, new Color(62f / 255f, 67f / 255f, 29f / 85f));
		colors.Add(DiagnosticResult.Opinion.Tutorial, new Color(62f / 255f, 67f / 255f, 29f / 85f));
		colors.Add(DiagnosticResult.Opinion.Good, Constants.POSITIVE_COLOR);
	}

	public DiagnosticCriterion[] GetCriteria()
	{
		DiagnosticCriterion[] array = new DiagnosticCriterion[criteria.Values.Count];
		criteria.Values.CopyTo(array, 0);
		return array;
	}

	public virtual string GetAverageValueString()
	{
		if (tracker != null)
		{
			return tracker.FormatValueString(Mathf.Round(tracker.GetAverageValue(trackerSampleCountSeconds)));
		}
		return "";
	}

	public virtual string GetCurrentValueString()
	{
		return "";
	}

	protected void AddCriterion(string id, DiagnosticCriterion criterion)
	{
		if (!criteria.ContainsKey(id))
		{
			criteria.Add(id, criterion);
		}
	}

	public virtual DiagnosticResult Evaluate()
	{
		DiagnosticResult result = new DiagnosticResult(DiagnosticResult.Opinion.Normal, "");
		foreach (KeyValuePair<string, DiagnosticCriterion> criterion in criteria)
		{
			DiagnosticResult diagnosticResult = criterion.Value.Evaluate();
			if (diagnosticResult.opinion < result.opinion)
			{
				result.opinion = diagnosticResult.opinion;
				result.Message = diagnosticResult.Message;
				result.clickThroughTarget = diagnosticResult.clickThroughTarget;
			}
		}
		return result;
	}

	public void SetResult(DiagnosticResult result)
	{
		LatestResult = result;
	}
}
