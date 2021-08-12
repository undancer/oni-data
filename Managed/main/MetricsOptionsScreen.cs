using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class MetricsOptionsScreen : KModalScreen
{
	public LocText title;

	public KButton dismissButton;

	public KButton closeButton;

	public GameObject enableButton;

	public Button descriptionButton;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		title.SetText(UI.FRONTEND.METRICS_OPTIONS_SCREEN.TITLE);
		GameObject obj = enableButton.GetComponent<HierarchyReferences>().GetReference("Button").gameObject;
		obj.GetComponent<ToolTip>().SetSimpleTooltip(UI.FRONTEND.METRICS_OPTIONS_SCREEN.TOOLTIP);
		obj.transform.GetChild(0).gameObject.SetActive(!KPrivacyPrefs.instance.disableDataCollection);
		obj.GetComponent<KButton>().onClick += delegate
		{
			OnClickToggle();
		};
		enableButton.GetComponent<HierarchyReferences>().GetReference<LocText>("Text").SetText(UI.FRONTEND.METRICS_OPTIONS_SCREEN.ENABLE_BUTTON);
		dismissButton.onClick += delegate
		{
			Deactivate();
		};
		closeButton.onClick += delegate
		{
			Deactivate();
		};
		descriptionButton.onClick.AddListener(delegate
		{
			Application.OpenURL("https://www.kleientertainment.com/privacy-policy");
		});
	}

	private void OnClickToggle()
	{
		KPrivacyPrefs.instance.disableDataCollection = !KPrivacyPrefs.instance.disableDataCollection;
		KPrivacyPrefs.Save();
		KPlayerPrefs.SetString("DisableDataCollection", KPrivacyPrefs.instance.disableDataCollection ? "yes" : "no");
		KPlayerPrefs.Save();
		ThreadedHttps<KleiMetrics>.Instance.SetEnabled(!KPrivacyPrefs.instance.disableDataCollection);
		enableButton.GetComponent<HierarchyReferences>().GetReference("CheckMark").gameObject.SetActive(ThreadedHttps<KleiMetrics>.Instance.enabled);
	}
}
