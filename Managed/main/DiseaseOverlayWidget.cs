using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/DiseaseOverlayWidget")]
public class DiseaseOverlayWidget : KMonoBehaviour
{
	[SerializeField]
	private Image progressFill;

	[SerializeField]
	private ToolTip progressToolTip;

	[SerializeField]
	private Image germsImage;

	[SerializeField]
	private Vector3 offset;

	[SerializeField]
	private Image diseasedImage;

	private List<Image> displayedDiseases = new List<Image>();

	public void Refresh(AmountInstance value_src)
	{
		GameObject gameObject = value_src.gameObject;
		if (gameObject == null)
		{
			return;
		}
		KAnimControllerBase component = gameObject.GetComponent<KAnimControllerBase>();
		Vector3 a = ((component != null) ? component.GetWorldPivot() : (gameObject.transform.GetPosition() + Vector3.down));
		base.transform.SetPosition(a + offset);
		if (value_src != null)
		{
			progressFill.transform.parent.gameObject.SetActive(value: true);
			float num = value_src.value / value_src.GetMax();
			Vector3 localScale = progressFill.rectTransform.localScale;
			localScale.y = num;
			progressFill.rectTransform.localScale = localScale;
			progressToolTip.toolTip = string.Concat(DUPLICANTS.ATTRIBUTES.IMMUNITY.NAME, " ", GameUtil.GetFormattedPercent(num * 100f));
		}
		else
		{
			progressFill.transform.parent.gameObject.SetActive(value: false);
		}
		int num2 = 0;
		Amounts amounts = gameObject.GetComponent<Modifiers>().GetAmounts();
		foreach (Disease resource in Db.Get().Diseases.resources)
		{
			float value = amounts.Get(resource.amount).value;
			if (value > 0f)
			{
				Image image = null;
				if (num2 < displayedDiseases.Count)
				{
					image = displayedDiseases[num2];
				}
				else
				{
					image = Util.KInstantiateUI(germsImage.gameObject, germsImage.transform.parent.gameObject, force_active: true).GetComponent<Image>();
					displayedDiseases.Add(image);
				}
				image.color = GlobalAssets.Instance.colorSet.GetColorByName(resource.overlayColourName);
				image.GetComponent<ToolTip>().toolTip = resource.Name + " " + GameUtil.GetFormattedDiseaseAmount((int)value);
				num2++;
			}
		}
		for (int num3 = displayedDiseases.Count - 1; num3 >= num2; num3--)
		{
			Util.KDestroyGameObject(displayedDiseases[num3].gameObject);
			displayedDiseases.RemoveAt(num3);
		}
		diseasedImage.enabled = false;
		progressFill.transform.parent.gameObject.SetActive(displayedDiseases.Count > 0);
		germsImage.transform.parent.gameObject.SetActive(displayedDiseases.Count > 0);
	}
}
