using UnityEngine;

public class EightDirectionController
{
	public enum Offset
	{
		Infront,
		Behind,
		UserSpecified
	}

	public GameObject gameObject;

	private string defaultAnim;

	private KAnimLink link;

	public KBatchedAnimController controller
	{
		get;
		private set;
	}

	public EightDirectionController(KAnimControllerBase buildingController, string targetSymbol, string defaultAnim, Offset frontBank)
	{
		Initialize(buildingController, targetSymbol, defaultAnim, frontBank, Grid.SceneLayer.NoLayer);
	}

	private void Initialize(KAnimControllerBase buildingController, string targetSymbol, string defaultAnim, Offset frontBack, Grid.SceneLayer userSpecifiedRenderLayer)
	{
		string name = buildingController.name + ".eight_direction";
		gameObject = new GameObject(name);
		gameObject.SetActive(value: false);
		gameObject.transform.parent = buildingController.transform;
		KPrefabID kPrefabID = gameObject.AddComponent<KPrefabID>();
		kPrefabID.PrefabTag = new Tag(name);
		this.defaultAnim = defaultAnim;
		controller = gameObject.AddOrGet<KBatchedAnimController>();
		controller.AnimFiles = new KAnimFile[1]
		{
			buildingController.AnimFiles[0]
		};
		controller.initialAnim = defaultAnim;
		controller.isMovable = true;
		controller.sceneLayer = Grid.SceneLayer.NoLayer;
		if (Offset.UserSpecified == frontBack)
		{
			controller.sceneLayer = userSpecifiedRenderLayer;
		}
		buildingController.SetSymbolVisiblity(targetSymbol, is_visible: false);
		bool symbolVisible;
		Vector4 column = buildingController.GetSymbolTransform(new HashedString(targetSymbol), out symbolVisible).GetColumn(3);
		Vector3 position = column;
		switch (frontBack)
		{
		case Offset.Behind:
			position.z = buildingController.transform.GetPosition().z + 0.1f;
			break;
		case Offset.Infront:
			position.z = buildingController.transform.GetPosition().z - 0.1f;
			break;
		case Offset.UserSpecified:
			position.z = Grid.GetLayerZ(userSpecifiedRenderLayer);
			break;
		}
		gameObject.transform.SetPosition(position);
		gameObject.SetActive(value: true);
		link = new KAnimLink(buildingController, controller);
	}

	public void SetPositionPercent(float percent_full)
	{
		if (!(controller == null))
		{
			controller.SetPositionPercent(percent_full);
		}
	}

	public void SetSymbolTint(KAnimHashedString symbol, Color32 colour)
	{
		if (controller != null)
		{
			controller.SetSymbolTint(symbol, colour);
		}
	}

	public void SetRotation(float rot)
	{
		if (!(controller == null))
		{
			controller.Rotation = rot;
		}
	}

	public void PlayAnim(string anim, KAnim.PlayMode mode = KAnim.PlayMode.Once)
	{
		controller.Play(anim, mode);
	}
}
