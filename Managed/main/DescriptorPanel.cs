using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/DescriptorPanel")]
public class DescriptorPanel : KMonoBehaviour
{
	[SerializeField]
	private GameObject customLabelPrefab;

	private List<GameObject> labels = new List<GameObject>();

	public bool HasDescriptors()
	{
		return labels.Count > 0;
	}

	public void SetDescriptors(IList<Descriptor> descriptors)
	{
		int i;
		for (i = 0; i < descriptors.Count; i++)
		{
			GameObject gameObject = null;
			if (i >= labels.Count)
			{
				gameObject = Util.KInstantiate((customLabelPrefab != null) ? customLabelPrefab : ScreenPrefabs.Instance.DescriptionLabel, base.gameObject);
				gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
				labels.Add(gameObject);
			}
			else
			{
				gameObject = labels[i];
			}
			gameObject.GetComponent<LocText>().text = descriptors[i].IndentedText();
			gameObject.GetComponent<ToolTip>().toolTip = descriptors[i].tooltipText;
			gameObject.SetActive(value: true);
		}
		for (; i < labels.Count; i++)
		{
			labels[i].SetActive(value: false);
		}
	}
}
