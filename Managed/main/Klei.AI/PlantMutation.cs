using System.Collections.Generic;
using System.Text;
using STRINGS;
using UnityEngine;

namespace Klei.AI
{
	public class PlantMutation : Modifier
	{
		public string desc;

		public bool originalMutation;

		public List<string> requiredPrefabIDs = new List<string>();

		public List<string> restrictedPrefabIDs = new List<string>();

		private Tag bonusCropID;

		private float bonusCropAmount;

		private byte droppedDiseaseID = byte.MaxValue;

		private int droppedDiseaseOnGrowAmount = 0;

		private int droppedDiseaseContinuousAmount = 0;

		private byte harvestDiseaseID = byte.MaxValue;

		private int harvestDiseaseAmount = 0;

		private bool forcePrefersDarkness = false;

		private bool forceSelfHarvestOnGrown = false;

		private PlantElementAbsorber.ConsumeInfo ensureIrrigationInfo;

		public PlantMutation(string id, string name, string desc)
			: base(id, name, desc)
		{
		}

		public void ApplyTo(MutantPlant target)
		{
			if (bonusCropID.IsValid)
			{
				target.Subscribe(-1072826864, OnHarvestBonusCrop);
			}
			if (forcePrefersDarkness || SelfModifiers.Find((AttributeModifier m) => m.AttributeId == Db.Get().PlantAttributes.MinLightLux.Id) != null)
			{
				CropSleepingMonitor.Def def = target.GetDef<CropSleepingMonitor.Def>();
				IlluminationVulnerable illuminationVulnerable = target.GetComponent<IlluminationVulnerable>();
				if (def == null && illuminationVulnerable == null)
				{
					illuminationVulnerable = target.gameObject.AddComponent<IlluminationVulnerable>();
				}
				if (forcePrefersDarkness)
				{
					if (def != null)
					{
						def.prefersDarkness = true;
					}
					if (illuminationVulnerable != null)
					{
						illuminationVulnerable.SetPrefersDarkness(prefersDarkness: true);
					}
				}
				else
				{
					if (def != null)
					{
						def.prefersDarkness = false;
					}
					if (illuminationVulnerable != null)
					{
						illuminationVulnerable.SetPrefersDarkness();
					}
					target.GetComponent<Modifiers>().attributes.Add(Db.Get().PlantAttributes.MinLightLux);
				}
			}
			if (droppedDiseaseID != byte.MaxValue)
			{
			}
			if (harvestDiseaseID != byte.MaxValue)
			{
				target.Subscribe(35625290, OnCropSpawnedAddDisease);
			}
			if (ensureIrrigationInfo.tag.IsValid)
			{
			}
			Attributes attributes = target.GetAttributes();
			AddTo(attributes);
		}

		private void OnHarvestBonusCrop(object data)
		{
			Crop crop = (Crop)data;
			crop.SpawnSomeFruit(bonusCropID, bonusCropAmount);
		}

		private void OnCropSpawnedAddDisease(object data)
		{
			GameObject gameObject = (GameObject)data;
			gameObject.GetComponent<PrimaryElement>().AddDisease(harvestDiseaseID, harvestDiseaseAmount, Name);
		}

		public string GetTooltip()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(desc);
			foreach (AttributeModifier selfModifier in SelfModifiers)
			{
				Attribute attribute = Db.Get().Attributes.TryGet(selfModifier.AttributeId);
				if (attribute == null)
				{
					attribute = Db.Get().PlantAttributes.Get(selfModifier.AttributeId);
				}
				stringBuilder.Append(DUPLICANTS.TRAITS.TRAIT_DESCRIPTION_LIST_ENTRY);
				stringBuilder.Append(string.Format(DUPLICANTS.TRAITS.ATTRIBUTE_MODIFIERS, attribute.Name, selfModifier.GetFormattedString()));
			}
			if (bonusCropID != null)
			{
				string newValue;
				if (!GameTags.DisplayAsCalories.Contains(bonusCropID))
				{
					newValue = ((!GameTags.DisplayAsUnits.Contains(bonusCropID)) ? GameUtil.GetFormattedMass(bonusCropAmount) : GameUtil.GetFormattedUnits(bonusCropAmount, GameUtil.TimeSlice.None, displaySuffix: false));
				}
				else
				{
					EdiblesManager.FoodInfo foodInfo = EdiblesManager.GetFoodInfo(bonusCropID.Name);
					DebugUtil.Assert(foodInfo != null, "Eeh? Trying to spawn a bonus crop that is caloric but isn't a food??", bonusCropID.ToString());
					newValue = GameUtil.GetFormattedCalories(bonusCropAmount * foodInfo.CaloriesPerUnit);
				}
				stringBuilder.Append(DUPLICANTS.TRAITS.TRAIT_DESCRIPTION_LIST_ENTRY);
				stringBuilder.Append(CREATURES.PLANT_MUTATIONS.BONUS_CROP_FMT.Replace("{Crop}", bonusCropID.ProperName()).Replace("{Amount}", newValue));
			}
			if (droppedDiseaseID != byte.MaxValue)
			{
				if (droppedDiseaseOnGrowAmount > 0)
				{
					stringBuilder.Append(DUPLICANTS.TRAITS.TRAIT_DESCRIPTION_LIST_ENTRY);
					stringBuilder.Append(UI.UISIDESCREENS.PLANTERSIDESCREEN.DISEASE_DROPPER_BURST.Replace("{Disease}", GameUtil.GetFormattedDiseaseName(droppedDiseaseID)).Replace("{DiseaseAmount}", GameUtil.GetFormattedDiseaseAmount(droppedDiseaseOnGrowAmount)));
				}
				if (droppedDiseaseContinuousAmount > 0)
				{
					stringBuilder.Append(DUPLICANTS.TRAITS.TRAIT_DESCRIPTION_LIST_ENTRY);
					stringBuilder.Append(UI.UISIDESCREENS.PLANTERSIDESCREEN.DISEASE_DROPPER_CONSTANT.Replace("{Disease}", GameUtil.GetFormattedDiseaseName(droppedDiseaseID)).Replace("{DiseaseAmount}", GameUtil.GetFormattedDiseaseAmount(droppedDiseaseContinuousAmount, GameUtil.TimeSlice.PerSecond)));
				}
			}
			if (harvestDiseaseID != byte.MaxValue)
			{
				stringBuilder.Append(DUPLICANTS.TRAITS.TRAIT_DESCRIPTION_LIST_ENTRY);
				stringBuilder.Append(UI.UISIDESCREENS.PLANTERSIDESCREEN.DISEASE_ON_HARVEST.Replace("{Disease}", GameUtil.GetFormattedDiseaseName(harvestDiseaseID)).Replace("{DiseaseAmount}", GameUtil.GetFormattedDiseaseAmount(harvestDiseaseAmount)));
			}
			if (forcePrefersDarkness)
			{
				stringBuilder.Append(DUPLICANTS.TRAITS.TRAIT_DESCRIPTION_LIST_ENTRY);
				stringBuilder.Append(UI.GAMEOBJECTEFFECTS.REQUIRES_DARKNESS);
			}
			if (forceSelfHarvestOnGrown)
			{
				stringBuilder.Append(DUPLICANTS.TRAITS.TRAIT_DESCRIPTION_LIST_ENTRY);
				stringBuilder.Append(UI.UISIDESCREENS.PLANTERSIDESCREEN.AUTO_SELF_HARVEST);
			}
			if (ensureIrrigationInfo.tag.IsValid)
			{
				stringBuilder.Append(DUPLICANTS.TRAITS.TRAIT_DESCRIPTION_LIST_ENTRY);
				stringBuilder.Append(string.Format(UI.GAMEOBJECTEFFECTS.IDEAL_FERTILIZER, ensureIrrigationInfo.tag.ProperName(), GameUtil.GetFormattedMass(0f - ensureIrrigationInfo.massConsumptionRate, GameUtil.TimeSlice.PerCycle), true));
			}
			return stringBuilder.ToString();
		}

		public void GetDescriptors(ref List<Descriptor> descriptors, GameObject go)
		{
			if (harvestDiseaseID != byte.MaxValue)
			{
				descriptors.Add(new Descriptor(UI.UISIDESCREENS.PLANTERSIDESCREEN.DISEASE_ON_HARVEST.Replace("{Disease}", GameUtil.GetFormattedDiseaseName(harvestDiseaseID)).Replace("{DiseaseAmount}", GameUtil.GetFormattedDiseaseAmount(harvestDiseaseAmount)), UI.UISIDESCREENS.PLANTERSIDESCREEN.TOOLTIPS.DISEASE_ON_HARVEST.Replace("{Disease}", GameUtil.GetFormattedDiseaseName(harvestDiseaseID)).Replace("{DiseaseAmount}", GameUtil.GetFormattedDiseaseAmount(harvestDiseaseAmount))));
			}
			if (forceSelfHarvestOnGrown)
			{
				descriptors.Add(new Descriptor(UI.UISIDESCREENS.PLANTERSIDESCREEN.AUTO_SELF_HARVEST, UI.UISIDESCREENS.PLANTERSIDESCREEN.TOOLTIPS.AUTO_SELF_HARVEST));
			}
		}

		public PlantMutation Original()
		{
			originalMutation = true;
			return this;
		}

		public PlantMutation RequiredPrefabID(string requiredID)
		{
			requiredPrefabIDs.Add(requiredID);
			return this;
		}

		public PlantMutation RestrictPrefabID(string restrictedID)
		{
			restrictedPrefabIDs.Add(restrictedID);
			return this;
		}

		public PlantMutation AttributeModifier(Attribute attribute, float amount, bool multiplier = false)
		{
			DebugUtil.Assert(!forcePrefersDarkness || attribute != Db.Get().PlantAttributes.MinLightLux, "A plant mutation has both darkness and light defined!", Id);
			Add(new AttributeModifier(attribute.Id, amount, Name, multiplier));
			return this;
		}

		public PlantMutation BonusCrop(Tag cropPrefabID, float bonucCropAmount)
		{
			bonusCropID = cropPrefabID;
			bonusCropAmount = bonucCropAmount;
			return this;
		}

		public PlantMutation DiseaseDropper(byte diseaseID, int onGrowAmount, int continuousAmount)
		{
			droppedDiseaseID = diseaseID;
			droppedDiseaseOnGrowAmount = onGrowAmount;
			droppedDiseaseContinuousAmount = continuousAmount;
			return this;
		}

		public PlantMutation AddDiseaseToHarvest(byte diseaseID, int amount)
		{
			harvestDiseaseID = diseaseID;
			harvestDiseaseAmount = amount;
			return this;
		}

		public PlantMutation ForcePrefersDarkness()
		{
			DebugUtil.Assert(SelfModifiers.Find((AttributeModifier m) => m.AttributeId == Db.Get().PlantAttributes.MinLightLux.Id) == null, "A plant mutation has both darkness and light defined!", Id);
			forcePrefersDarkness = true;
			return this;
		}

		public PlantMutation ForceSelfHarvestOnGrown()
		{
			forceSelfHarvestOnGrown = true;
			AttributeModifier(Db.Get().Amounts.OldAge.maxAttribute, -0.999999f, multiplier: true);
			return this;
		}

		public PlantMutation EnsureIrrigated(PlantElementAbsorber.ConsumeInfo consumeInfo)
		{
			ensureIrrigationInfo = consumeInfo;
			return this;
		}

		public PlantMutation VisualTint(float r, float g, float b)
		{
			return this;
		}

		public PlantMutation VisualSymbolTint(string targetSymbolName, float r, float g, float b)
		{
			return this;
		}

		public PlantMutation VisualSymbolOverride(string targetSymbolName, string sourceAnim, string sourceSymbol)
		{
			return this;
		}

		public PlantMutation VisualSymbolScale(string targetSymbolName, float scale)
		{
			return this;
		}

		public PlantMutation VisualBGFX(string animName)
		{
			return this;
		}

		public PlantMutation VisualFGFX(string animName)
		{
			return this;
		}
	}
}
