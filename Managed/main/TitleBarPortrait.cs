using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/TitleBarPortrait")]
public class TitleBarPortrait : KMonoBehaviour
{
	public GameObject FaceObject;

	public GameObject ImageObject;

	public GameObject PortraitShadow;

	public GameObject AnimControllerObject;

	public Material DefaultMaterial;

	public Material DesatMaterial;

	public void SetSaturation(bool saturated)
	{
		ImageObject.GetComponent<Image>().material = (saturated ? DefaultMaterial : DesatMaterial);
	}

	public void SetPortrait(GameObject selectedTarget)
	{
		MinionIdentity component = selectedTarget.GetComponent<MinionIdentity>();
		if (component != null)
		{
			SetPortrait(component);
			return;
		}
		Building component2 = selectedTarget.GetComponent<Building>();
		if (component2 != null)
		{
			SetPortrait(component2.Def.GetUISprite());
			return;
		}
		MeshRenderer componentInChildren = selectedTarget.GetComponentInChildren<MeshRenderer>();
		if ((bool)componentInChildren)
		{
			SetPortrait(Sprite.Create((Texture2D)componentInChildren.material.mainTexture, new Rect(0f, 0f, componentInChildren.material.mainTexture.width, componentInChildren.material.mainTexture.height), new Vector2(0.5f, 0.5f)));
		}
	}

	public void SetPortrait(Sprite image)
	{
		if ((bool)PortraitShadow)
		{
			PortraitShadow.SetActive(value: true);
		}
		if ((bool)FaceObject)
		{
			FaceObject.SetActive(value: false);
		}
		if ((bool)ImageObject)
		{
			ImageObject.SetActive(value: true);
		}
		if ((bool)AnimControllerObject)
		{
			AnimControllerObject.SetActive(value: false);
		}
		if (image == null)
		{
			ClearPortrait();
		}
		else
		{
			ImageObject.GetComponent<Image>().sprite = image;
		}
	}

	private void SetPortrait(MinionIdentity identity)
	{
		if ((bool)PortraitShadow)
		{
			PortraitShadow.SetActive(value: true);
		}
		if ((bool)FaceObject)
		{
			FaceObject.SetActive(value: false);
		}
		if ((bool)ImageObject)
		{
			ImageObject.SetActive(value: false);
		}
		CrewPortrait component = GetComponent<CrewPortrait>();
		if (component != null)
		{
			component.SetIdentityObject(identity);
		}
		else if ((bool)AnimControllerObject)
		{
			AnimControllerObject.SetActive(value: true);
			CrewPortrait.SetPortraitData(identity, AnimControllerObject.GetComponent<KBatchedAnimController>());
		}
	}

	public void ClearPortrait()
	{
		if ((bool)PortraitShadow)
		{
			PortraitShadow.SetActive(value: false);
		}
		if ((bool)FaceObject)
		{
			FaceObject.SetActive(value: false);
		}
		if ((bool)ImageObject)
		{
			ImageObject.SetActive(value: false);
		}
		if ((bool)AnimControllerObject)
		{
			AnimControllerObject.SetActive(value: false);
		}
	}
}
