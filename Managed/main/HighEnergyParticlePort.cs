using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class HighEnergyParticlePort : KMonoBehaviour, IGameObjectEffectDescriptor
{
	public delegate void OnParticleCapture(HighEnergyParticle particle);

	public delegate bool OnParticleCaptureAllowed(HighEnergyParticle particle);

	public OnParticleCapture onParticleCapture;

	public OnParticleCaptureAllowed onParticleCaptureAllowed;

	public OnParticleCapture onParticleUncapture;

	public HighEnergyParticle currentParticle = null;

	public bool requireOperational = true;

	public bool particleInputEnabled;

	public bool particleOutputEnabled;

	public CellOffset particleInputOffset;

	public CellOffset particleOutputOffset;

	public int GetHighEnergyParticleInputPortPosition()
	{
		return Grid.OffsetCell(Grid.PosToCell(this), particleInputOffset);
	}

	public int GetHighEnergyParticleOutputPortPosition()
	{
		return Grid.OffsetCell(Grid.PosToCell(this), particleInputOffset);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Components.HighEnergyParticlePorts.Add(this);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Components.HighEnergyParticlePorts.Remove(this);
	}

	public bool InputActive()
	{
		Operational component = GetComponent<Operational>();
		return particleInputEnabled && component != null && component.IsFunctional && (!requireOperational || component.IsOperational);
	}

	public bool AllowCapture(HighEnergyParticle particle)
	{
		if (onParticleCaptureAllowed != null)
		{
			return onParticleCaptureAllowed(particle);
		}
		return true;
	}

	public void Capture(HighEnergyParticle particle)
	{
		currentParticle = particle;
		if (onParticleCapture != null)
		{
			onParticleCapture(particle);
		}
	}

	public void Uncapture(HighEnergyParticle particle)
	{
		if (onParticleUncapture != null)
		{
			onParticleUncapture(particle);
		}
		currentParticle = null;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (particleInputEnabled)
		{
			list.Add(new Descriptor(UI.BUILDINGEFFECTS.PARTICLE_PORT_INPUT, UI.BUILDINGEFFECTS.TOOLTIPS.PARTICLE_PORT_INPUT, Descriptor.DescriptorType.Requirement));
		}
		if (particleOutputEnabled)
		{
			list.Add(new Descriptor(UI.BUILDINGEFFECTS.PARTICLE_PORT_OUTPUT, UI.BUILDINGEFFECTS.TOOLTIPS.PARTICLE_PORT_OUTPUT));
		}
		return list;
	}
}
