using System;
using UnityEngine;
using UnityEngine.UI;

public class SparkLayer : LineLayer
{
	[Serializable]
	public struct ColorRules
	{
		public bool setOwnColor;

		public bool positiveIsGood;

		public bool zeroIsBad;
	}

	public Image subZeroAreaFill;

	public ColorRules colorRules;

	public bool debugMark = false;

	public bool scaleHeightToData = true;

	public bool scaleWidthToData = true;

	public void SetColor(ColonyDiagnosticUtility.ColonyDiagnostic.DiagnosticResult result)
	{
		switch (result.opinion)
		{
		case ColonyDiagnosticUtility.ColonyDiagnostic.DiagnosticResult.Opinion.Bad:
			SetColor(Constants.NEGATIVE_COLOR);
			break;
		case ColonyDiagnosticUtility.ColonyDiagnostic.DiagnosticResult.Opinion.Warning:
			SetColor(Constants.WARNING_COLOR);
			break;
		case ColonyDiagnosticUtility.ColonyDiagnostic.DiagnosticResult.Opinion.Normal:
			SetColor(Constants.NEUTRAL_COLOR);
			break;
		case ColonyDiagnosticUtility.ColonyDiagnostic.DiagnosticResult.Opinion.Good:
			SetColor(Constants.POSITIVE_COLOR);
			break;
		}
	}

	public void SetColor(Color color)
	{
		line_formatting[0].color = color;
	}

	public override GraphedLine NewLine(Tuple<float, float>[] points, string ID = "")
	{
		Color pOSITIVE_COLOR = Constants.POSITIVE_COLOR;
		Color nEUTRAL_COLOR = Constants.NEUTRAL_COLOR;
		Color nEGATIVE_COLOR = Constants.NEGATIVE_COLOR;
		if (colorRules.setOwnColor)
		{
			if (points.Length > 2)
			{
				if (colorRules.zeroIsBad && points[points.Length - 1].second == 0f)
				{
					line_formatting[0].color = nEGATIVE_COLOR;
				}
				else if (points[points.Length - 1].second > points[points.Length - 2].second)
				{
					line_formatting[0].color = (colorRules.positiveIsGood ? pOSITIVE_COLOR : nEGATIVE_COLOR);
				}
				else if (points[points.Length - 1].second < points[points.Length - 2].second)
				{
					line_formatting[0].color = (colorRules.positiveIsGood ? nEGATIVE_COLOR : pOSITIVE_COLOR);
				}
				else
				{
					line_formatting[0].color = nEUTRAL_COLOR;
				}
			}
			else
			{
				line_formatting[0].color = nEUTRAL_COLOR;
			}
		}
		ScaleToData(points);
		if (subZeroAreaFill != null)
		{
			subZeroAreaFill.color = new Color(line_formatting[0].color.r, line_formatting[0].color.g, line_formatting[0].color.b, fillAlphaMin);
		}
		return base.NewLine(points, ID);
	}

	public override void RefreshLine(Tuple<float, float>[] points, string ID)
	{
		SetColor(points);
		ScaleToData(points);
		base.RefreshLine(points, ID);
	}

	private void SetColor(Tuple<float, float>[] points)
	{
		Color pOSITIVE_COLOR = Constants.POSITIVE_COLOR;
		Color nEUTRAL_COLOR = Constants.NEUTRAL_COLOR;
		Color nEGATIVE_COLOR = Constants.NEGATIVE_COLOR;
		if (colorRules.setOwnColor)
		{
			if (points.Length > 2)
			{
				if (colorRules.zeroIsBad && points[points.Length - 1].second == 0f)
				{
					line_formatting[0].color = nEGATIVE_COLOR;
				}
				else if (points[points.Length - 1].second > points[points.Length - 2].second)
				{
					line_formatting[0].color = (colorRules.positiveIsGood ? pOSITIVE_COLOR : nEGATIVE_COLOR);
				}
				else if (points[points.Length - 1].second < points[points.Length - 2].second)
				{
					line_formatting[0].color = (colorRules.positiveIsGood ? nEGATIVE_COLOR : pOSITIVE_COLOR);
				}
				else
				{
					line_formatting[0].color = nEUTRAL_COLOR;
				}
			}
			else
			{
				line_formatting[0].color = nEUTRAL_COLOR;
			}
		}
		if (subZeroAreaFill != null)
		{
			subZeroAreaFill.color = new Color(line_formatting[0].color.r, line_formatting[0].color.g, line_formatting[0].color.b, fillAlphaMin);
		}
	}

	private void ScaleToData(Tuple<float, float>[] points)
	{
		if (scaleWidthToData || scaleHeightToData)
		{
			Vector2 vector = CalculateMin(points);
			Vector2 vector2 = CalculateMax(points);
			if (scaleHeightToData)
			{
				base.graph.ClearHorizontalGuides();
				base.graph.axis_y.max_value = vector2.y;
				base.graph.axis_y.min_value = vector.y;
				base.graph.RefreshHorizontalGuides();
			}
			if (scaleWidthToData)
			{
				base.graph.ClearVerticalGuides();
				base.graph.axis_x.max_value = vector2.x;
				base.graph.axis_x.min_value = vector.x;
				base.graph.RefreshVerticalGuides();
			}
		}
	}
}
