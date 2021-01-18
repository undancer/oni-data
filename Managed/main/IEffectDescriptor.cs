using System;
using System.Collections.Generic;

[Obsolete("No longer used. Use IGameObjectEffectDescriptor instead", false)]
public interface IEffectDescriptor
{
	List<Descriptor> GetDescriptors(BuildingDef def);
}
