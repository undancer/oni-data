using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/StarmapPlanetVisualizer")]
public class StarmapPlanetVisualizer : KMonoBehaviour
{
	public Image image;

	public LocText label;

	public MultiToggle button;

	public RectTransform selection;

	public GameObject analysisSelection;

	public Image unknownBG;

	public GameObject rocketIconContainer;
}
