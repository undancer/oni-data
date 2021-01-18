using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class ReportScreen : KScreen
{
	[SerializeField]
	private LocText title;

	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private KButton prevButton;

	[SerializeField]
	private KButton nextButton;

	[SerializeField]
	private KButton summaryButton;

	[SerializeField]
	private GameObject lineItem;

	[SerializeField]
	private GameObject lineItemSpacer;

	[SerializeField]
	private GameObject lineItemHeader;

	[SerializeField]
	private GameObject contentFolder;

	private Dictionary<string, GameObject> lineItems = new Dictionary<string, GameObject>();

	private ReportManager.DailyReport currentReport;

	public static ReportScreen Instance
	{
		get;
		private set;
	}

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
		closeButton.onClick += delegate
		{
			ManagementMenu.Instance.CloseAll();
		};
		prevButton.onClick += delegate
		{
			ShowReport(currentReport.day - 1);
		};
		nextButton.onClick += delegate
		{
			ShowReport(currentReport.day + 1);
		};
		summaryButton.onClick += delegate
		{
			RetiredColonyData currentColonyRetiredColonyData = RetireColonyUtility.GetCurrentColonyRetiredColonyData();
			MainMenu.ActivateRetiredColoniesScreenFromData(PauseScreen.Instance.transform.parent.gameObject, currentColonyRetiredColonyData);
		};
		base.ConsumeMouseScroll = true;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	protected override void OnShow(bool bShow)
	{
		base.OnShow(bShow);
		if (ReportManager.Instance != null)
		{
			currentReport = ReportManager.Instance.TodaysReport;
		}
	}

	public void SetTitle(string title)
	{
		this.title.text = title;
	}

	public override void ScreenUpdate(bool b)
	{
		base.ScreenUpdate(b);
		Refresh();
	}

	private void Refresh()
	{
		Debug.Assert(currentReport != null);
		if (currentReport.day == ReportManager.Instance.TodaysReport.day)
		{
			SetTitle(string.Format(UI.ENDOFDAYREPORT.DAY_TITLE_TODAY, currentReport.day));
		}
		else if (currentReport.day == ReportManager.Instance.TodaysReport.day - 1)
		{
			SetTitle(string.Format(UI.ENDOFDAYREPORT.DAY_TITLE_YESTERDAY, currentReport.day));
		}
		else
		{
			SetTitle(string.Format(UI.ENDOFDAYREPORT.DAY_TITLE, currentReport.day));
		}
		bool flag = currentReport.day < ReportManager.Instance.TodaysReport.day;
		nextButton.isInteractable = flag;
		if (flag)
		{
			nextButton.GetComponent<ToolTip>().toolTip = string.Format(UI.ENDOFDAYREPORT.DAY_TITLE, currentReport.day + 1);
			nextButton.GetComponent<ToolTip>().enabled = true;
		}
		else
		{
			nextButton.GetComponent<ToolTip>().enabled = false;
		}
		flag = currentReport.day > 1;
		prevButton.isInteractable = flag;
		if (flag)
		{
			prevButton.GetComponent<ToolTip>().toolTip = string.Format(UI.ENDOFDAYREPORT.DAY_TITLE, currentReport.day - 1);
			prevButton.GetComponent<ToolTip>().enabled = true;
		}
		else
		{
			prevButton.GetComponent<ToolTip>().enabled = false;
		}
		AddSpacer(0);
		int num = 1;
		foreach (KeyValuePair<ReportManager.ReportType, ReportManager.ReportGroup> reportGroup in ReportManager.Instance.ReportGroups)
		{
			ReportManager.ReportEntry entry = currentReport.GetEntry(reportGroup.Key);
			if (num != reportGroup.Value.group)
			{
				num = reportGroup.Value.group;
				AddSpacer(num);
			}
			bool flag2 = entry.accumulate != 0f || reportGroup.Value.reportIfZero;
			if (reportGroup.Value.isHeader)
			{
				CreateHeader(reportGroup.Value);
			}
			else if (flag2)
			{
				CreateOrUpdateLine(entry, reportGroup.Value, flag2);
			}
		}
	}

	public void ShowReport(int day)
	{
		currentReport = ReportManager.Instance.FindReport(day);
		Debug.Assert(currentReport != null, "Can't find report for day: " + day);
		Refresh();
	}

	private GameObject AddSpacer(int group)
	{
		GameObject gameObject = null;
		if (lineItems.ContainsKey(group.ToString()))
		{
			gameObject = lineItems[group.ToString()];
		}
		else
		{
			gameObject = Util.KInstantiateUI(lineItemSpacer, contentFolder);
			gameObject.name = "Spacer" + group;
			lineItems[group.ToString()] = gameObject;
		}
		gameObject.SetActive(value: true);
		return gameObject;
	}

	private GameObject CreateHeader(ReportManager.ReportGroup reportGroup)
	{
		GameObject value = null;
		lineItems.TryGetValue(reportGroup.stringKey, out value);
		if (value == null)
		{
			value = Util.KInstantiateUI(lineItemHeader, contentFolder, force_active: true);
			value.name = "LineItemHeader" + lineItems.Count;
			lineItems[reportGroup.stringKey] = value;
		}
		value.SetActive(value: true);
		ReportScreenHeader component = value.GetComponent<ReportScreenHeader>();
		component.SetMainEntry(reportGroup);
		return value;
	}

	private GameObject CreateOrUpdateLine(ReportManager.ReportEntry entry, ReportManager.ReportGroup reportGroup, bool is_line_active)
	{
		GameObject value = null;
		lineItems.TryGetValue(reportGroup.stringKey, out value);
		if (!is_line_active)
		{
			if (value != null && value.activeSelf)
			{
				value.SetActive(value: false);
			}
		}
		else
		{
			if (value == null)
			{
				value = Util.KInstantiateUI(lineItem, contentFolder, force_active: true);
				value.name = "LineItem" + lineItems.Count;
				lineItems[reportGroup.stringKey] = value;
			}
			value.SetActive(value: true);
			ReportScreenEntry component = value.GetComponent<ReportScreenEntry>();
			component.SetMainEntry(entry, reportGroup);
		}
		return value;
	}

	private void OnClickClose()
	{
		PlaySound3D(GlobalAssets.GetSound("HUD_Click_Close"));
		Show(show: false);
	}
}
