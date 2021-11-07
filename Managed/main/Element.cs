using System;
using System.Collections.Generic;
using System.Diagnostics;
using Klei.AI;
using STRINGS;

[Serializable]
[DebuggerDisplay("{name}")]
public class Element : IComparable<Element>
{
	[Serializable]
	public enum State : byte
	{
		Vacuum = 0,
		Gas = 1,
		Liquid = 2,
		Solid = 3,
		Unbreakable = 4,
		Unstable = 8,
		TemperatureInsulated = 0x10
	}

	public const int INVALID_ID = 0;

	public SimHashes id;

	public Tag tag;

	public byte idx;

	public float specificHeatCapacity;

	public float thermalConductivity = 1f;

	public float molarMass = 1f;

	public float strength;

	public float flow;

	public float maxCompression;

	public float viscosity;

	public float minHorizontalFlow = float.PositiveInfinity;

	public float minVerticalFlow = float.PositiveInfinity;

	public float maxMass = 10000f;

	public float solidSurfaceAreaMultiplier;

	public float liquidSurfaceAreaMultiplier;

	public float gasSurfaceAreaMultiplier;

	public State state;

	public byte hardness;

	public float lowTemp;

	public SimHashes lowTempTransitionTarget;

	public Element lowTempTransition;

	public float highTemp;

	public SimHashes highTempTransitionTarget;

	public Element highTempTransition;

	public SimHashes highTempTransitionOreID = SimHashes.Vacuum;

	public float highTempTransitionOreMassConversion;

	public SimHashes lowTempTransitionOreID = SimHashes.Vacuum;

	public float lowTempTransitionOreMassConversion;

	public SimHashes sublimateId;

	public SimHashes convertId;

	public SpawnFXHashes sublimateFX;

	public float sublimateRate;

	public float sublimateEfficiency;

	public float sublimateProbability;

	public float offGasPercentage;

	public float lightAbsorptionFactor;

	public float radiationAbsorptionFactor;

	public float radiationPer1000Mass;

	public Sim.PhysicsData defaultValues;

	public float toxicity;

	public Substance substance;

	public Tag materialCategory;

	public int buildMenuSort;

	public ElementLoader.ElementComposition[] elementComposition;

	public Tag[] oreTags = new Tag[0];

	public List<AttributeModifier> attributeModifiers = new List<AttributeModifier>();

	public bool disabled;

	public string dlcId;

	public const byte StateMask = 3;

	public bool IsUnstable => HasTag(GameTags.Unstable);

	public bool IsLiquid => (state & State.Solid) == State.Liquid;

	public bool IsGas => (state & State.Solid) == State.Gas;

	public bool IsSolid => (state & State.Solid) == State.Solid;

	public bool IsVacuum => (state & State.Solid) == 0;

	public bool IsTemperatureInsulated => (state & State.TemperatureInsulated) != 0;

	public bool HasTransitionUp
	{
		get
		{
			if (highTempTransitionTarget != 0 && highTempTransitionTarget != SimHashes.Unobtanium && highTempTransition != null)
			{
				return highTempTransition != this;
			}
			return false;
		}
	}

	public string name { get; set; }

	public string nameUpperCase { get; set; }

	public string description { get; set; }

	public float PressureToMass(float pressure)
	{
		return pressure / defaultValues.pressure;
	}

	public bool IsState(State expected_state)
	{
		return (state & State.Solid) == expected_state;
	}

	public string GetStateString()
	{
		return GetStateString(state);
	}

	public static string GetStateString(State state)
	{
		if ((state & State.Solid) == State.Solid)
		{
			return ELEMENTS.STATE.SOLID;
		}
		if ((state & State.Solid) == State.Liquid)
		{
			return ELEMENTS.STATE.LIQUID;
		}
		if ((state & State.Solid) == State.Gas)
		{
			return ELEMENTS.STATE.GAS;
		}
		return ELEMENTS.STATE.VACUUM;
	}

	public string FullDescription(bool addHardnessColor = true)
	{
		string text = Description();
		if (IsSolid)
		{
			text += "\n\n";
			text += string.Format(ELEMENTS.ELEMENTDESCSOLID, GetMaterialCategoryTag().ProperName(), GameUtil.GetFormattedTemperature(highTemp), GameUtil.GetHardnessString(this, addHardnessColor));
		}
		else if (IsLiquid)
		{
			text += "\n\n";
			text += string.Format(ELEMENTS.ELEMENTDESCLIQUID, GetMaterialCategoryTag().ProperName(), GameUtil.GetFormattedTemperature(lowTemp), GameUtil.GetFormattedTemperature(highTemp));
		}
		else if (!IsVacuum)
		{
			text += "\n\n";
			text += string.Format(ELEMENTS.ELEMENTDESCGAS, GetMaterialCategoryTag().ProperName(), GameUtil.GetFormattedTemperature(lowTemp));
		}
		string text2 = ELEMENTS.THERMALPROPERTIES;
		text2 = text2.Replace("{SPECIFIC_HEAT_CAPACITY}", GameUtil.GetFormattedSHC(specificHeatCapacity));
		text2 = text2.Replace("{THERMAL_CONDUCTIVITY}", GameUtil.GetFormattedThermalConductivity(thermalConductivity));
		text = text + "\n" + text2;
		if (DlcManager.FeatureRadiationEnabled())
		{
			text = text + "\n" + string.Format(ELEMENTS.RADIATIONPROPERTIES, radiationAbsorptionFactor, GameUtil.GetFormattedRads(radiationPer1000Mass * 1.1f / 600f, GameUtil.TimeSlice.PerCycle));
		}
		if (oreTags.Length != 0 && !IsVacuum)
		{
			text += "\n\n";
			string text3 = "";
			for (int i = 0; i < oreTags.Length; i++)
			{
				Tag tag = new Tag(oreTags[i]);
				text3 += tag.ProperName();
				if (i < oreTags.Length - 1)
				{
					text3 += ", ";
				}
			}
			text += string.Format(ELEMENTS.ELEMENTPROPERTIES, text3);
		}
		if (attributeModifiers.Count > 0)
		{
			foreach (AttributeModifier attributeModifier in attributeModifiers)
			{
				string arg = Db.Get().BuildingAttributes.Get(attributeModifier.AttributeId).Name;
				string formattedString = attributeModifier.GetFormattedString();
				text = text + "\n" + string.Format(DUPLICANTS.MODIFIERS.MODIFIER_FORMAT, arg, formattedString);
			}
			return text;
		}
		return text;
	}

	public string Description()
	{
		return description;
	}

	public bool HasTag(Tag search_tag)
	{
		if (tag == search_tag)
		{
			return true;
		}
		return Array.IndexOf(oreTags, search_tag) != -1;
	}

	public Tag GetMaterialCategoryTag()
	{
		return materialCategory;
	}

	public int CompareTo(Element other)
	{
		return id - other.id;
	}
}
