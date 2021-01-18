using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/AsteroidDescriptorPanel")]
public class AsteroidDescriptorPanel : KMonoBehaviour
{
	[SerializeField]
	private GameObject customLabelPrefab;

	private List<GameObject> labels = new List<GameObject>();

	public bool HasDescriptors()
	{
		return labels.Count > 0;
	}

	public void SetDescriptors(IList<AsteroidDescriptor> descriptors)
	{
		int i;
		for (i = 0; i < descriptors.Count; i++)
		{
			GameObject gameObject = null;
			if (i >= labels.Count)
			{
				GameObject original = ((customLabelPrefab != null) ? customLabelPrefab : ScreenPrefabs.Instance.DescriptionLabel);
				gameObject = Util.KInstantiate(original, base.gameObject);
				gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
				labels.Add(gameObject);
			}
			else
			{
				gameObject = labels[i];
			}
			HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
			component.GetReference<LocText>("Label").text = descriptors[i].text;
			component.GetReference<ToolTip>("ToolTip").toolTip = descriptors[i].tooltip;
			if (descriptors[i].bands != null)
			{
				Transform reference = component.GetReference<Transform>("BandContainer");
				Transform reference2 = component.GetReference<Transform>("BarBitPrefab");
				int j;
				for (j = 0; j < descriptors[i].bands.Count; j++)
				{
					Transform transform = ((j < reference.childCount) ? reference.GetChild(j) : Util.KInstantiateUI<Transform>(reference2.gameObject, reference.gameObject));
					Image component2 = transform.GetComponent<Image>();
					LayoutElement component3 = transform.GetComponent<LayoutElement>();
					component2.color = descriptors[i].bands[j].second;
					component3.flexibleWidth = descriptors[i].bands[j].third;
					transform.GetComponent<ToolTip>().toolTip = descriptors[i].bands[j].first;
					transform.gameObject.SetActive(value: true);
				}
				for (; j < reference.childCount; j++)
				{
					reference.GetChild(j).gameObject.SetActive(value: false);
				}
			}
			gameObject.SetActive(value: true);
		}
		for (; i < labels.Count; i++)
		{
			labels[i].SetActive(value: false);
		}
	}
}
