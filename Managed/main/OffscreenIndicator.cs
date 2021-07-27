using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/OffscreenIndicator")]
public class OffscreenIndicator : KMonoBehaviour
{
	public GameObject IndicatorPrefab;

	public GameObject IndicatorContainer;

	private Dictionary<GameObject, GameObject> targets = new Dictionary<GameObject, GameObject>();

	public static OffscreenIndicator Instance;

	[SerializeField]
	private float edgeInset = 25f;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Instance = this;
	}

	private void Update()
	{
		foreach (KeyValuePair<GameObject, GameObject> target in targets)
		{
			UpdateArrow(target.Value, target.Key);
		}
	}

	public void ActivateIndicator(GameObject target)
	{
		if (!targets.ContainsKey(target))
		{
			Tuple<Sprite, Color> uISprite = Def.GetUISprite(target);
			if (uISprite != null)
			{
				ActivateIndicator(target, uISprite);
			}
		}
	}

	public void ActivateIndicator(GameObject target, GameObject iconSource)
	{
		if (!targets.ContainsKey(target))
		{
			MinionIdentity component = iconSource.GetComponent<MinionIdentity>();
			if (component != null)
			{
				GameObject gameObject = Util.KInstantiateUI(IndicatorPrefab, IndicatorContainer, force_active: true);
				gameObject.GetComponent<HierarchyReferences>().GetReference<Image>("icon").gameObject.SetActive(value: false);
				CrewPortrait reference = gameObject.GetComponent<HierarchyReferences>().GetReference<CrewPortrait>("Portrait");
				reference.gameObject.SetActive(value: true);
				reference.SetIdentityObject(component);
				targets.Add(target, gameObject);
			}
		}
	}

	public void ActivateIndicator(GameObject target, Tuple<Sprite, Color> icon)
	{
		if (!targets.ContainsKey(target))
		{
			GameObject gameObject = Util.KInstantiateUI(IndicatorPrefab, IndicatorContainer, force_active: true);
			Image reference = gameObject.GetComponent<HierarchyReferences>().GetReference<Image>("icon");
			if (icon != null)
			{
				reference.sprite = icon.first;
				reference.color = icon.second;
				targets.Add(target, gameObject);
			}
		}
	}

	public void DeactivateIndicator(GameObject target)
	{
		if (targets.ContainsKey(target))
		{
			Object.Destroy(targets[target]);
			targets.Remove(target);
		}
	}

	private void UpdateArrow(GameObject arrow, GameObject target)
	{
		if (target == null)
		{
			Object.Destroy(arrow);
			targets.Remove(target);
			return;
		}
		Vector3 vector = Camera.main.WorldToViewportPoint(target.transform.position);
		if ((double)vector.x > 0.3 && (double)vector.x < 0.7 && (double)vector.y > 0.3 && (double)vector.y < 0.7)
		{
			arrow.GetComponent<HierarchyReferences>().GetReference<CrewPortrait>("Portrait").SetIdentityObject(null);
			arrow.SetActive(value: false);
			return;
		}
		arrow.SetActive(value: true);
		arrow.rectTransform().SetLocalPosition(Vector3.zero);
		Vector3 vector2 = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));
		vector2.z = target.transform.position.z;
		Vector3 normalized = (target.transform.position - vector2).normalized;
		arrow.transform.up = normalized;
		UpdateTargetIconPosition(target, arrow);
	}

	private void UpdateTargetIconPosition(GameObject goTarget, GameObject indicator)
	{
		Vector3 vector = goTarget.transform.position;
		vector = Camera.main.WorldToViewportPoint(vector);
		if (vector.z < 0f)
		{
			vector.x = 1f - vector.x;
			vector.y = 1f - vector.y;
			vector.z = 0f;
			vector = Vector3Maxamize(vector);
		}
		vector = Camera.main.ViewportToScreenPoint(vector);
		vector.x = Mathf.Clamp(vector.x, edgeInset, (float)Screen.width - edgeInset);
		vector.y = Mathf.Clamp(vector.y, edgeInset, (float)Screen.height - edgeInset);
		indicator.transform.position = vector;
		indicator.GetComponent<HierarchyReferences>().GetReference<Image>("icon").rectTransform.up = Vector3.up;
		indicator.GetComponent<HierarchyReferences>().GetReference<CrewPortrait>("Portrait").transform.up = Vector3.up;
	}

	public Vector3 Vector3Maxamize(Vector3 vector)
	{
		Vector3 vector2 = vector;
		float num = 0f;
		num = ((vector.x > num) ? vector.x : num);
		num = ((vector.y > num) ? vector.y : num);
		num = ((vector.z > num) ? vector.z : num);
		return vector2 / num;
	}
}
