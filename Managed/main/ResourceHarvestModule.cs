using System.Collections.Generic;
using UnityEngine;

public class ResourceHarvestModule : GameStateMachine<ResourceHarvestModule, ResourceHarvestModule.StatesInstance, IStateMachineTarget, ResourceHarvestModule.Def>
{
	public class Def : BaseDef
	{
		public float harvestSpeed;
	}

	public class NotGroundedStates : State
	{
		public State not_harvesting;

		public State harvesting;
	}

	public class StatesInstance : GameInstance
	{
		private MeterController meter;

		private Storage storage;

		public StatesInstance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			storage = GetComponent<Storage>();
			GetComponent<RocketModule>().AddModuleCondition(ProcessCondition.ProcessConditionType.RocketStorage, new ConditionHasResource(storage, SimHashes.Diamond, 1000f));
			Subscribe(-1697596308, UpdateMeter);
			meter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, "meter_target", "meter_fill", "meter_frame", "meter_OL");
			meter.gameObject.GetComponent<KBatchedAnimTracker>().matchParentOffset = true;
			UpdateMeter();
		}

		protected override void OnCleanUp()
		{
			base.OnCleanUp();
			Unsubscribe(-1697596308, UpdateMeter);
		}

		public void UpdateMeter(object data = null)
		{
			meter.SetPositionPercent(storage.MassStored() / storage.Capacity());
		}

		public void HarvestFromPOI(float dt)
		{
			Clustercraft component = GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>();
			if (!CheckIfCanHarvest())
			{
				return;
			}
			ClusterGridEntity pOIAtCurrentLocation = component.GetPOIAtCurrentLocation();
			if (pOIAtCurrentLocation == null || pOIAtCurrentLocation.GetComponent<HarvestablePOIClusterGridEntity>() == null)
			{
				return;
			}
			HarvestablePOIStates.Instance sMI = pOIAtCurrentLocation.GetSMI<HarvestablePOIStates.Instance>();
			Dictionary<SimHashes, float> elementsWithWeights = sMI.configuration.GetElementsWithWeights();
			float num = 0f;
			foreach (KeyValuePair<SimHashes, float> item in elementsWithWeights)
			{
				num += item.Value;
			}
			foreach (KeyValuePair<SimHashes, float> item2 in elementsWithWeights)
			{
				Element element = ElementLoader.FindElementByHash(item2.Key);
				if (!DiscoveredResources.Instance.IsDiscovered(element.tag))
				{
					DiscoveredResources.Instance.Discover(element.tag, element.GetMaterialCategoryTag());
				}
			}
			float num2 = Mathf.Min(GetMaxExtractKGFromDiamondAvailable(), base.def.harvestSpeed * dt);
			float num3 = 0f;
			float num4 = 0f;
			float num5 = 0f;
			foreach (KeyValuePair<SimHashes, float> item3 in elementsWithWeights)
			{
				if (num3 >= num2)
				{
					break;
				}
				SimHashes key = item3.Key;
				float num6 = item3.Value / num;
				float num7 = base.def.harvestSpeed * dt * num6;
				num3 += num7;
				Element element2 = ElementLoader.FindElementByHash(key);
				CargoBay.CargoType cargoType = CargoBay.ElementStateToCargoTypes[element2.state & Element.State.Solid];
				List<CargoBayCluster> cargoBaysOfType = component.GetCargoBaysOfType(cargoType);
				float num8 = num7;
				foreach (CargoBayCluster item4 in cargoBaysOfType)
				{
					float num9 = Mathf.Min(item4.RemainingCapacity, num8);
					if (num9 != 0f)
					{
						num4 += num9;
						num8 -= num9;
						switch (element2.state & Element.State.Solid)
						{
						case Element.State.Gas:
							item4.storage.AddGasChunk(key, num9, element2.defaultValues.temperature, byte.MaxValue, 0, keep_zero_mass: false);
							break;
						case Element.State.Liquid:
							item4.storage.AddLiquid(key, num9, element2.defaultValues.temperature, byte.MaxValue, 0);
							break;
						case Element.State.Solid:
							item4.storage.AddOre(key, num9, element2.defaultValues.temperature, byte.MaxValue, 0);
							break;
						}
					}
					if (num8 == 0f)
					{
						break;
					}
				}
				num5 += num8;
			}
			sMI.DeltaPOICapacity(0f - num3);
			ConsumeDiamond(num3 * 0.05f);
			if (num5 > 0f)
			{
				component.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.SpacePOIWasting, num5 / dt);
			}
			else
			{
				component.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.SpacePOIWasting);
			}
		}

		public void ConsumeDiamond(float amount)
		{
			GetComponent<Storage>().ConsumeIgnoringDisease(SimHashes.Diamond.CreateTag(), amount);
		}

		public float GetMaxExtractKGFromDiamondAvailable()
		{
			return GetComponent<Storage>().GetAmountAvailable(SimHashes.Diamond.CreateTag()) / 0.05f;
		}

		public bool CheckIfCanHarvest()
		{
			Clustercraft component = GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>();
			if (component == null)
			{
				base.sm.canHarvest.Set(value: false, this);
				return false;
			}
			if (base.master.GetComponent<Storage>().MassStored() <= 0f)
			{
				base.sm.canHarvest.Set(value: false, this);
				return false;
			}
			ClusterGridEntity pOIAtCurrentLocation = component.GetPOIAtCurrentLocation();
			bool flag = false;
			if (pOIAtCurrentLocation != null && (bool)pOIAtCurrentLocation.GetComponent<HarvestablePOIClusterGridEntity>())
			{
				HarvestablePOIStates.Instance sMI = pOIAtCurrentLocation.GetSMI<HarvestablePOIStates.Instance>();
				if (!sMI.POICanBeHarvested())
				{
					base.sm.canHarvest.Set(value: false, this);
					return false;
				}
				foreach (KeyValuePair<SimHashes, float> elementsWithWeight in sMI.configuration.GetElementsWithWeights())
				{
					Element element = ElementLoader.FindElementByHash(elementsWithWeight.Key);
					CargoBay.CargoType cargoType = CargoBay.ElementStateToCargoTypes[element.state & Element.State.Solid];
					List<CargoBayCluster> cargoBaysOfType = component.GetCargoBaysOfType(cargoType);
					if (cargoBaysOfType == null || cargoBaysOfType.Count <= 0)
					{
						continue;
					}
					foreach (CargoBayCluster item in cargoBaysOfType)
					{
						if (item.storage.RemainingCapacity() > 0f)
						{
							flag = true;
						}
					}
					if (flag)
					{
						break;
					}
				}
			}
			base.sm.canHarvest.Set(flag, this);
			return flag;
		}

		public static void AddHarvestStatusItems(GameObject statusTarget, float harvestRate)
		{
			statusTarget.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.SpacePOIHarvesting, harvestRate);
		}

		public static void RemoveHarvestStatusItems(GameObject statusTarget)
		{
			statusTarget.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.SpacePOIHarvesting);
		}
	}

	public BoolParameter canHarvest;

	public FloatParameter lastHarvestTime;

	public State grounded;

	public NotGroundedStates not_grounded;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = grounded;
		root.Enter(delegate(StatesInstance smi)
		{
			smi.CheckIfCanHarvest();
		});
		grounded.TagTransition(GameTags.RocketNotOnGround, not_grounded).Enter(delegate(StatesInstance smi)
		{
			smi.UpdateMeter();
		});
		not_grounded.DefaultState(not_grounded.not_harvesting).EventHandler(GameHashes.ClusterLocationChanged, (StatesInstance smi) => Game.Instance, delegate(StatesInstance smi)
		{
			smi.CheckIfCanHarvest();
		}).EventHandler(GameHashes.OnStorageChange, delegate(StatesInstance smi)
		{
			smi.CheckIfCanHarvest();
		})
			.TagTransition(GameTags.RocketNotOnGround, grounded, on_remove: true);
		not_grounded.not_harvesting.PlayAnim("loaded").ParamTransition(canHarvest, not_grounded.harvesting, GameStateMachine<ResourceHarvestModule, StatesInstance, IStateMachineTarget, Def>.IsTrue).Enter(delegate(StatesInstance smi)
		{
			StatesInstance.RemoveHarvestStatusItems(smi.master.gameObject.GetComponent<RocketModuleCluster>().CraftInterface.gameObject);
		})
			.Update(delegate(StatesInstance smi, float dt)
			{
				smi.CheckIfCanHarvest();
			}, UpdateRate.SIM_4000ms);
		not_grounded.harvesting.PlayAnim("deploying").Exit(delegate(StatesInstance smi)
		{
			smi.master.gameObject.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>().Trigger(939543986);
			StatesInstance.RemoveHarvestStatusItems(smi.master.gameObject.GetComponent<RocketModuleCluster>().CraftInterface.gameObject);
		}).Enter(delegate(StatesInstance smi)
		{
			Clustercraft component = smi.master.gameObject.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>();
			component.AddTag(GameTags.POIHarvesting);
			component.Trigger(-1762453998);
			StatesInstance.AddHarvestStatusItems(smi.master.gameObject.GetComponent<RocketModuleCluster>().CraftInterface.gameObject, smi.def.harvestSpeed);
		})
			.Exit(delegate(StatesInstance smi)
			{
				smi.master.gameObject.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>().RemoveTag(GameTags.POIHarvesting);
			})
			.Update(delegate(StatesInstance smi, float dt)
			{
				smi.HarvestFromPOI(dt);
				lastHarvestTime.Set(Time.time, smi);
			}, UpdateRate.SIM_4000ms)
			.ParamTransition(canHarvest, not_grounded.not_harvesting, GameStateMachine<ResourceHarvestModule, StatesInstance, IStateMachineTarget, Def>.IsFalse);
	}
}
