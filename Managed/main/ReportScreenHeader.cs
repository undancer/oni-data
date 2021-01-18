using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ReportScreenHeader")]
public class ReportScreenHeader : KMonoBehaviour
{
	[SerializeField]
	private ReportScreenHeaderRow rowTemplate;

	private ReportScreenHeaderRow mainRow;

	public void SetMainEntry(ReportManager.ReportGroup reportGroup)
	{
		if (mainRow == null)
		{
			mainRow = Util.KInstantiateUI(rowTemplate.gameObject, base.gameObject, force_active: true).GetComponent<ReportScreenHeaderRow>();
		}
		mainRow.SetLine(reportGroup);
	}
}
