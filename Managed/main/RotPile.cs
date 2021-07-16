using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class RotPile : StateMachineComponent<RotPile.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, RotPile, object>.GameInstance
	{
		public AttributeModifier baseDecomposeRate;

		public StatesInstance(RotPile master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, RotPile>
	{
		public State decomposing;

		public State convertDestroy;

		public FloatParameter decompositionAmount;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = decomposing;
			base.serializable = SerializeType.Both_DEPRECATED;
			decomposing.Enter(delegate(StatesInstance smi)
			{
				smi.master.TryCreateNotification();
			}).Exit(delegate(StatesInstance smi)
			{
				smi.master.TryClearNotification();
			}).ParamTransition(decompositionAmount, convertDestroy, (StatesInstance smi, float p) => p >= 600f)
				.Update("Decomposing", delegate(StatesInstance smi, float dt)
				{
					decompositionAmount.Delta(dt, smi);
				});
			convertDestroy.Enter(delegate(StatesInstance smi)
			{
				smi.master.ConvertToElement();
			});
		}
	}

	private Notification notification;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	protected void ConvertToElement()
	{
		PrimaryElement component = base.smi.master.GetComponent<PrimaryElement>();
		float mass = component.Mass;
		float temperature = component.Temperature;
		if (mass <= 0f)
		{
			Util.KDestroyGameObject(base.gameObject);
			return;
		}
		SimHashes hash = SimHashes.ToxicSand;
		GameObject gameObject = ElementLoader.FindElementByHash(hash).substance.SpawnResource(base.smi.master.transform.GetPosition(), mass, temperature, byte.MaxValue, 0);
		PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, ElementLoader.FindElementByHash(hash).name, gameObject.transform);
		Util.KDestroyGameObject(base.smi.gameObject);
	}

	private static string OnRottenTooltip(List<Notification> notifications, object data)
	{
		string text = "";
		foreach (Notification notification2 in notifications)
		{
			if (notification2.tooltipData != null)
			{
				text = text + "\n• " + (string)notification2.tooltipData + " ";
			}
		}
		return string.Format(MISC.NOTIFICATIONS.FOODROT.TOOLTIP, text);
	}

	public void TryClearNotification()
	{
		if (notification != null)
		{
			base.gameObject.AddOrGet<Notifier>().Remove(notification);
		}
	}

	public void TryCreateNotification()
	{
		WorldContainer myWorld = base.smi.master.GetMyWorld();
		if (myWorld != null && myWorld.worldInventory.IsReachable(base.smi.master.gameObject.GetComponent<Pickupable>()))
		{
			notification = new Notification(MISC.NOTIFICATIONS.FOODROT.NAME, NotificationType.BadMinor, OnRottenTooltip);
			notification.tooltipData = base.smi.master.gameObject.GetProperName();
			base.gameObject.AddOrGet<Notifier>().Add(notification);
		}
	}
}
