using UnityEngine;

public class KAnimSynchronizedController
{
	private KAnimControllerBase controller;

	public KAnimControllerBase synchronizedController;

	private KAnimLink link;

	private string postfix;

	public string Postfix
	{
		get
		{
			return postfix;
		}
		set
		{
			postfix = value;
		}
	}

	public KAnimSynchronizedController(KAnimControllerBase controller, Grid.SceneLayer layer, string postfix)
	{
		this.controller = controller;
		Postfix = postfix;
		GameObject gameObject = Util.KInstantiate(EntityPrefabs.Instance.ForegroundLayer, controller.gameObject);
		gameObject.name = controller.name + postfix;
		synchronizedController = gameObject.GetComponent<KAnimControllerBase>();
		synchronizedController.AnimFiles = controller.AnimFiles;
		gameObject.SetActive(value: true);
		synchronizedController.initialAnim = controller.initialAnim + postfix;
		TransformExtensions.SetLocalPosition(position: new Vector3(0f, 0f, Grid.GetLayerZ(layer) - 0.1f), transform: gameObject.transform);
		link = new KAnimLink(controller, synchronizedController);
		Dirty();
		KAnimSynchronizer synchronizer = controller.GetSynchronizer();
		synchronizer.Add(this);
		synchronizer.SyncController(this);
	}

	public void Enable(bool enable)
	{
		synchronizedController.enabled = enable;
	}

	public void Play(HashedString anim_name, KAnim.PlayMode mode = KAnim.PlayMode.Once, float speed = 1f, float time_offset = 0f)
	{
		if (synchronizedController.enabled && synchronizedController.HasAnimation(anim_name))
		{
			synchronizedController.Play(anim_name, mode, speed, time_offset);
		}
	}

	public void Dirty()
	{
		if (!(synchronizedController == null))
		{
			synchronizedController.Offset = controller.Offset;
			synchronizedController.Pivot = controller.Pivot;
			synchronizedController.Rotation = controller.Rotation;
			synchronizedController.FlipX = controller.FlipX;
			synchronizedController.FlipY = controller.FlipY;
		}
	}
}
