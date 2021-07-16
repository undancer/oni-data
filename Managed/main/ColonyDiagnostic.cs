using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public abstract class ColonyDiagnostic : ISim4000ms
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
				switch (opinion)
				{
				case Opinion.Bad:
					return "<color=" + Constants.NEGATIVE_COLOR_STR + ">" + message + "</color>";
				case Opinion.Warning:
					return "<color=" + Constants.NEGATIVE_COLOR_STR + ">" + message + "</color>";
				case Opinion.Concern:
					return "<color=" + Constants.WARNING_COLOR_STR + ">" + message + "</color>";
				case Opinion.Suggestion:
				case Opinion.Normal:
					return "<color=" + Constants.WHITE_COLOR_STR + ">" + message + "</color>";
				case Opinion.Good:
					return "<color=" + Constants.POSITIVE_COLOR_STR + ">" + message + "</color>";
				default:
					return message;
				}
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

	public PresentationSetting presentationSetting;

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
		colors.Add(DiagnosticResult.Opinion.Suggestion, Constants.NEUTRAL_COLOR);
		colors.Add(DiagnosticResult.Opinion.Tutorial, Constants.NEUTRAL_COLOR);
		colors.Add(DiagnosticResult.Opinion.Good, Constants.POSITIVE_COLOR);
		SimAndRenderScheduler.instance.Add(this, load_balance: true);
	}

	public virtual string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public void OnCleanUp()
	{
		SimAndRenderScheduler.instance.Remove(this);
	}

	public void Sim4000ms(float dt)
	{
		SetResult(ColonyDiagnosticUtility.IgnoreFirstUpdate ? ColonyDiagnosticUtility.NoDataResult : Evaluate());
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
			criterion.SetID(id);
			criteria.Add(id, criterion);
		}
	}

	public virtual DiagnosticResult Evaluate()
	{
		DiagnosticResult result = new DiagnosticResult(DiagnosticResult.Opinion.Normal, "");
		foreach (KeyValuePair<string, DiagnosticCriterion> criterion in criteria)
		{
			if (ColonyDiagnosticUtility.Instance.IsCriteriaEnabled(worldID, id, criterion.Key))
			{
				DiagnosticResult diagnosticResult = criterion.Value.Evaluate();
				if (diagnosticResult.opinion < result.opinion)
				{
					result.opinion = diagnosticResult.opinion;
					result.Message = diagnosticResult.Message;
					result.clickThroughTarget = diagnosticResult.clickThroughTarget;
				}
			}
		}
		return result;
	}

	public void SetResult(DiagnosticResult result)
	{
		LatestResult = result;
	}
}
