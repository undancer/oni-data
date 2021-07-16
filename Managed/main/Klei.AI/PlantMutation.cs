using System.Collections.Generic;
using System.Text;
using STRINGS;
using UnityEngine;

namespace Klei.AI
{
	public class PlantMutation : Modifier
	{
		private class SymbolOverrideInfo
		{
			public string targetSymbolName;

			public string sourceAnim;

			public string sourceSymbol;
		}

		public string desc;

		public string animationSoundEvent;

		public bool originalMutation;

		public List<string> requiredPrefabIDs = new List<string>();

		public List<string> restrictedPrefabIDs = new List<string>();

		private Tag bonusCropID;

		private float bonusCropAmount;

		private byte droppedDiseaseID = byte.MaxValue;

		private int droppedDiseaseOnGrowAmount;

		private int droppedDiseaseContinuousAmount;

		private byte harvestDiseaseID = byte.MaxValue;

		private int harvestDiseaseAmount;

		private bool forcePrefersDarkness;

		private bool forceSelfHarvestOnGrown;

		private PlantElementAbsorber.ConsumeInfo ensureIrrigationInfo;

		private Color plantTint = Color.white;

		private List<string> symbolTintTargets = new List<string>();

		private List<Color> symbolTints = new List<Color>();

		private List<SymbolOverrideInfo> symbolOverrideInfo;

		private List<string> symbolScaleTargets = new List<string>();

		private List<float> symbolScales = new List<float>();

		private string bGFXAnim;

		private string fGFXAnim;

		private List<string> additionalSoundEvents = new List<string>();

		public List<string> AdditionalSoundEvents => additionalSoundEvents;

		public PlantMutation(string id, string name, string desc)
			: base(id, name, desc)
		{
		}

		public void ApplyTo(MutantPlant target)
		{
			ApplyFunctionalTo(target);
			if (!target.HasTag(GameTags.Seed) && !target.HasTag(GameTags.CropSeed))
			{
				ApplyVisualTo(target);
			}
		}

		private void ApplyFunctionalTo(MutantPlant target)
		{
			SeedProducer component = target.GetComponent<SeedProducer>();
			if (component != null && component.seedInfo.productionType == SeedProducer.ProductionType.Harvest)
			{
				component.Configure(component.seedInfo.seedId, SeedProducer.ProductionType.Sterile, 0);
			}
			if (bonusCropID.IsValid)
			{
				target.Subscribe(-1072826864, OnHarvestBonusCrop);
			}
			if (forcePrefersDarkness || SelfModifiers.Find((AttributeModifier m) => m.AttributeId == Db.Get().PlantAttributes.MinLightLux.Id) != null)
			{
				IlluminationVulnerable illuminationVulnerable = target.GetComponent<IlluminationVulnerable>();
				if (illuminationVulnerable == null)
				{
					illuminationVulnerable = target.gameObject.AddComponent<IlluminationVulnerable>();
				}
				if (forcePrefersDarkness)
				{
					if (illuminationVulnerable != null)
					{
						illuminationVulnerable.SetPrefersDarkness(prefersDarkness: true);
					}
				}
				else
				{
					if (illuminationVulnerable != null)
					{
						illuminationVulnerable.SetPrefersDarkness();
					}
					target.GetComponent<Modifiers>().attributes.Add(Db.Get().PlantAttributes.MinLightLux);
				}
			}
			_ = droppedDiseaseID;
			_ = 255;
			if (harvestDiseaseID != byte.MaxValue)
			{
				target.Subscribe(35625290, OnCropSpawnedAddDisease);
			}
			_ = ensureIrrigationInfo.tag.IsValid;
			Attributes attributes = target.GetAttributes();
			AddTo(attributes);
		}

		private void ApplyVisualTo(MutantPlant target)
		{
			KBatchedAnimController component = target.GetComponent<KBatchedAnimController>();
			if (symbolOverrideInfo != null && symbolOverrideInfo.Count > 0)
			{
				SymbolOverrideController component2 = target.GetComponent<SymbolOverrideController>();
				foreach (SymbolOverrideInfo item in symbolOverrideInfo)
				{
					KAnim.Build.Symbol symbol = Assets.GetAnim(item.sourceAnim).GetData().build.GetSymbol(item.sourceSymbol);
					component2.AddSymbolOverride(item.targetSymbolName, symbol);
				}
			}
			if (bGFXAnim != null)
			{
				CreateFXObject(target, bGFXAnim, "_BGFX", 0.1f);
			}
			if (fGFXAnim != null)
			{
				CreateFXObject(target, fGFXAnim, "_FGFX", -0.1f);
			}
			if (plantTint != Color.white)
			{
				component.TintColour = plantTint;
			}
			if (symbolTints.Count > 0)
			{
				for (int i = 0; i < symbolTints.Count; i++)
				{
					component.SetSymbolTint(symbolTintTargets[i], symbolTints[i]);
				}
			}
			if (symbolScales.Count > 0)
			{
				for (int j = 0; j < symbolScales.Count; j++)
				{
					component.SetSymbolScale(symbolScaleTargets[j], symbolScales[j]);
				}
			}
			if (additionalSoundEvents.Count > 0)
			{
				for (int k = 0; k < additionalSoundEvents.Count; k++)
				{
				}
			}
		}

		private static void CreateFXObject(MutantPlant target, string anim, string nameSuffix, float offset)
		{
			GameObject gameObject = Object.Instantiate(Assets.GetPrefab(SimpleFXConfig.ID));
			gameObject.name = target.name + nameSuffix;
			gameObject.transform.parent = target.transform;
			gameObject.AddComponent<LoopingSounds>();
			gameObject.GetComponent<KPrefabID>().PrefabTag = new Tag(gameObject.name);
			Extents extents = target.GetComponent<OccupyArea>().GetExtents();
			Vector3 position = target.transform.GetPosition();
			position.x = (float)extents.x + (float)extents.width / 2f;
			position.y = (float)extents.y + (float)extents.height / 2f;
			position.z += offset;
			gameObject.transform.SetPosition(position);
			KBatchedAnimController component = gameObject.GetComponent<KBatchedAnimController>();
			component.AnimFiles = new KAnimFile[1]
			{
				Assets.GetAnim(anim)
			};
			component.initialAnim = "idle";
			component.initialMode = KAnim.PlayMode.Loop;
			component.randomiseLoopedOffset = true;
			component.fgLayer = Grid.SceneLayer.NoLayer;
			if (target.HasTag(GameTags.Hanging))
			{
				component.Rotation = 180f;
			}
			gameObject.SetActive(value: true);
		}

		private void OnHarvestBonusCrop(object data)
		{
			((Crop)data).SpawnSomeFruit(bonusCropID, bonusCropAmount);
		}

		private void OnCropSpawnedAddDisease(object data)
		{
			((GameObject)data).GetComponent<PrimaryElement>().AddDisease(harvestDiseaseID, harvestDiseaseAmount, Name);
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
			if (!originalMutation)
			{
				stringBuilder.Append(DUPLICANTS.TRAITS.TRAIT_DESCRIPTION_LIST_ENTRY);
				stringBuilder.Append(UI.GAMEOBJECTEFFECTS.MUTANT_STERILE);
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
			Debug.Assert(Mathf.Sign(r) == Mathf.Sign(g) && Mathf.Sign(r) == Mathf.Sign(b), "Vales for tints must be all positive or all negative for the shader to work correctly!");
			if (r < 0f)
			{
				plantTint = Color.white + new Color(r, g, b, 0f);
			}
			else
			{
				plantTint = new Color(r, g, b, 0f);
			}
			return this;
		}

		public PlantMutation VisualSymbolTint(string targetSymbolName, float r, float g, float b)
		{
			Debug.Assert(Mathf.Sign(r) == Mathf.Sign(g) && Mathf.Sign(r) == Mathf.Sign(b), "Vales for tints must be all positive or all negative for the shader to work correctly!");
			symbolTintTargets.Add(targetSymbolName);
			symbolTints.Add(Color.white + new Color(r, g, b, 0f));
			return this;
		}

		public PlantMutation VisualSymbolOverride(string targetSymbolName, string sourceAnim, string sourceSymbol)
		{
			if (symbolOverrideInfo == null)
			{
				symbolOverrideInfo = new List<SymbolOverrideInfo>();
			}
			symbolOverrideInfo.Add(new SymbolOverrideInfo
			{
				targetSymbolName = targetSymbolName,
				sourceAnim = sourceAnim,
				sourceSymbol = sourceSymbol
			});
			return this;
		}

		public PlantMutation VisualSymbolScale(string targetSymbolName, float scale)
		{
			symbolScaleTargets.Add(targetSymbolName);
			symbolScales.Add(scale);
			return this;
		}

		public PlantMutation VisualBGFX(string animName)
		{
			bGFXAnim = animName;
			return this;
		}

		public PlantMutation VisualFGFX(string animName)
		{
			fGFXAnim = animName;
			return this;
		}

		public PlantMutation AddSoundEvent(string soundEventName)
		{
			additionalSoundEvents.Add(soundEventName);
			return this;
		}
	}
}
