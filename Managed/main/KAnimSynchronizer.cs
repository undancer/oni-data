using System.Collections.Generic;

public class KAnimSynchronizer
{
	private KAnimControllerBase masterController;

	private List<KAnimControllerBase> Targets = new List<KAnimControllerBase>();

	private List<KAnimSynchronizedController> SyncedControllers = new List<KAnimSynchronizedController>();

	public KAnimSynchronizer(KAnimControllerBase master_controller)
	{
		masterController = master_controller;
	}

	private void Clear(KAnimControllerBase controller)
	{
		controller.Play("idle_default", KAnim.PlayMode.Loop);
	}

	public void Add(KAnimControllerBase controller)
	{
		Targets.Add(controller);
	}

	public void Remove(KAnimControllerBase controller)
	{
		Clear(controller);
		Targets.Remove(controller);
	}

	private void Clear(KAnimSynchronizedController controller)
	{
		controller.Play("idle_default" + controller.Postfix, KAnim.PlayMode.Loop);
	}

	public void Add(KAnimSynchronizedController controller)
	{
		SyncedControllers.Add(controller);
	}

	public void Remove(KAnimSynchronizedController controller)
	{
		Clear(controller);
		SyncedControllers.Remove(controller);
	}

	public void Clear()
	{
		foreach (KAnimControllerBase target in Targets)
		{
			Clear(target);
		}
		Targets.Clear();
		foreach (KAnimSynchronizedController syncedController in SyncedControllers)
		{
			Clear(syncedController);
		}
		SyncedControllers.Clear();
	}

	public void Sync(KAnimControllerBase controller)
	{
		if (masterController == null || controller == null)
		{
			return;
		}
		KAnim.Anim currentAnim = masterController.GetCurrentAnim();
		if (currentAnim != null && !string.IsNullOrEmpty(controller.defaultAnim) && !controller.HasAnimation(currentAnim.name))
		{
			controller.Play(controller.defaultAnim, KAnim.PlayMode.Loop);
		}
		else if (currentAnim != null)
		{
			KAnim.PlayMode mode = masterController.GetMode();
			float playSpeed = masterController.GetPlaySpeed();
			float elapsedTime = masterController.GetElapsedTime();
			controller.Play(currentAnim.name, mode, playSpeed, elapsedTime);
			Facing component = controller.GetComponent<Facing>();
			if (component != null)
			{
				float x = component.transform.GetPosition().x;
				x += (masterController.FlipX ? (-0.5f) : 0.5f);
				component.Face(x);
			}
			else
			{
				controller.FlipX = masterController.FlipX;
				controller.FlipY = masterController.FlipY;
			}
		}
	}

	public void SyncController(KAnimSynchronizedController controller)
	{
		if (masterController == null || controller == null)
		{
			return;
		}
		KAnim.Anim currentAnim = masterController.GetCurrentAnim();
		string s = ((currentAnim != null) ? (currentAnim.name + controller.Postfix) : string.Empty);
		if (!string.IsNullOrEmpty(controller.synchronizedController.defaultAnim) && !controller.synchronizedController.HasAnimation(s))
		{
			controller.Play(controller.synchronizedController.defaultAnim, KAnim.PlayMode.Loop);
		}
		else if (currentAnim != null)
		{
			KAnim.PlayMode mode = masterController.GetMode();
			float playSpeed = masterController.GetPlaySpeed();
			float elapsedTime = masterController.GetElapsedTime();
			controller.Play(s, mode, playSpeed, elapsedTime);
			Facing component = controller.synchronizedController.GetComponent<Facing>();
			if (component != null)
			{
				float x = component.transform.GetPosition().x;
				x += (masterController.FlipX ? (-0.5f) : 0.5f);
				component.Face(x);
			}
			else
			{
				controller.synchronizedController.FlipX = masterController.FlipX;
				controller.synchronizedController.FlipY = masterController.FlipY;
			}
		}
	}

	public void Sync()
	{
		for (int i = 0; i < Targets.Count; i++)
		{
			KAnimControllerBase controller = Targets[i];
			Sync(controller);
		}
		for (int j = 0; j < SyncedControllers.Count; j++)
		{
			KAnimSynchronizedController controller2 = SyncedControllers[j];
			SyncController(controller2);
		}
	}

	public void SyncTime()
	{
		float elapsedTime = masterController.GetElapsedTime();
		for (int i = 0; i < Targets.Count; i++)
		{
			Targets[i].SetElapsedTime(elapsedTime);
		}
		for (int j = 0; j < SyncedControllers.Count; j++)
		{
			SyncedControllers[j].synchronizedController.SetElapsedTime(elapsedTime);
		}
	}
}
