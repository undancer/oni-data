using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

public class HighEnergyParticleStorage : KMonoBehaviour, IStorage
{
	[Serialize]
	[SerializeField]
	private float particles;

	public float capacity;

	public bool showInUI = true;

	public bool autoStore;

	[Serialize]
	public bool receiverOpen = true;

	[MyCmpGet]
	private LogicPorts _logicPorts;

	public string PORT_ID = "";

	public float Particles => particles;

	public bool allowUIItemRemoval { get; set; }

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (autoStore)
		{
			HighEnergyParticlePort component = base.gameObject.GetComponent<HighEnergyParticlePort>();
			component.onParticleCapture = (HighEnergyParticlePort.OnParticleCapture)Delegate.Combine(component.onParticleCapture, new HighEnergyParticlePort.OnParticleCapture(OnParticleCapture));
			component.onParticleCaptureAllowed = (HighEnergyParticlePort.OnParticleCaptureAllowed)Delegate.Combine(component.onParticleCaptureAllowed, new HighEnergyParticlePort.OnParticleCaptureAllowed(OnParticleCaptureAllowed));
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		UpdateLogicPorts();
	}

	private void UpdateLogicPorts()
	{
		if (_logicPorts != null)
		{
			bool value = IsFull();
			_logicPorts.SendSignal(PORT_ID, Convert.ToInt32(value));
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if (autoStore)
		{
			HighEnergyParticlePort component = base.gameObject.GetComponent<HighEnergyParticlePort>();
			component.onParticleCapture = (HighEnergyParticlePort.OnParticleCapture)Delegate.Remove(component.onParticleCapture, new HighEnergyParticlePort.OnParticleCapture(OnParticleCapture));
		}
	}

	private void OnParticleCapture(HighEnergyParticle particle)
	{
		float num = Mathf.Min(particle.payload, capacity - particles);
		Store(num);
		particle.payload -= num;
		if (particle.payload > 0f)
		{
			base.gameObject.GetComponent<HighEnergyParticlePort>().Uncapture(particle);
		}
	}

	private bool OnParticleCaptureAllowed(HighEnergyParticle particle)
	{
		if (particles < capacity)
		{
			return receiverOpen;
		}
		return false;
	}

	public void DeltaParticles(float delta)
	{
		particles += delta;
		if (particles <= 0f)
		{
			Trigger(155636535, base.transform.gameObject);
		}
		Trigger(-1837862626, base.transform.gameObject);
		UpdateLogicPorts();
	}

	public void Store(float amount)
	{
		DebugUtil.Assert(amount >= 0f, $"Storing negative amount ({amount}) of particles");
		DeltaParticles(amount);
	}

	public float ConsumeAndGet(float amount)
	{
		amount = Mathf.Min(Particles, amount);
		DeltaParticles(0f - amount);
		return amount;
	}

	[ContextMenu("Trigger Stored Event")]
	public void DEBUG_TriggerStorageEvent()
	{
		Trigger(-1837862626, base.transform.gameObject);
	}

	[ContextMenu("Trigger Zero Event")]
	public void DEBUG_TriggerZeroEvent()
	{
		ConsumeAndGet(particles + 1f);
	}

	public float ConsumeAll()
	{
		return ConsumeAndGet(particles);
	}

	public bool HasRadiation()
	{
		return Particles > 0f;
	}

	public GameObject Drop(GameObject go, bool do_disease_transfer = true)
	{
		return null;
	}

	public List<GameObject> GetItems()
	{
		return new List<GameObject> { base.gameObject };
	}

	public bool IsFull()
	{
		return RemainingCapacity() <= 0f;
	}

	public bool IsEmpty()
	{
		return Particles == 0f;
	}

	public float Capacity()
	{
		return capacity;
	}

	public float RemainingCapacity()
	{
		return Mathf.Max(capacity - Particles, 0f);
	}

	public bool ShouldShowInUI()
	{
		return showInUI;
	}

	public float GetAmountAvailable(Tag tag)
	{
		if (tag != GameTags.HighEnergyParticle)
		{
			return 0f;
		}
		return Particles;
	}

	public void ConsumeIgnoringDisease(Tag tag, float amount)
	{
		DebugUtil.DevAssert(tag == GameTags.HighEnergyParticle, "Consuming non-particle tag as amount");
		ConsumeAndGet(amount);
	}
}
