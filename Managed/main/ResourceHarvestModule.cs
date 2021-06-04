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
		public StatesInstance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			Storage component = GetComponent<Storage>();
			RocketModule component2 = GetComponent<RocketModule>();
			component2.AddModuleCondition(ProcessCondition.ProcessConditionType.RocketStorage, new ConditionHasResource(component, SimHashes.Diamond, 1000f));
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
			float num2 = Mathf.Min(GetMaxExtractKGFromDiamondAvailable(), base.def.harvestSpeed * dt);
			float num3 = 0f;
			float num4 = 0f;
			float num5 = 0f;
			foreach (KeyValuePair<SimHashes, float> item2 in elementsWithWeights)
			{
				if (num3 >= num2)
				{
					break;
				}
				SimHashes key = item2.Key;
				float value = item2.Value;
				float num6 = value / num;
				float num7 = base.def.harvestSpeed * dt * num6;
				num3 += num7;
				Element element = ElementLoader.FindElementByHash(key);
				CargoBay.CargoType cargoType = CargoBay.ElementStateToCargoTypes[element.state & Element.State.Solid];
				List<CargoBayCluster> cargoBaysOfType = component.GetCargoBaysOfType(cargoType);
				float num8 = num7;
				foreach (CargoBayCluster item3 in cargoBaysOfType)
				{
					float num9 = Mathf.Min(item3.RemainingCapacity, num8);
					if (num9 != 0f)
					{
						num4 += num9;
						num8 -= num9;
						switch (element.state & Element.State.Solid)
						{
						case Element.State.Gas:
							item3.storage.AddGasChunk(key, num9, element.defaultValues.temperature, byte.MaxValue, 0, keep_zero_mass: false);
							break;
						case Element.State.Liquid:
							item3.storage.AddLiquid(key, num9, element.defaultValues.temperature, byte.MaxValue, 0);
							break;
						case Element.State.Solid:
							item3.storage.AddOre(key, num9, element.defaultValues.temperature, byte.MaxValue, 0);
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
			Storage component = GetComponent<Storage>();
			return component.GetAmountAvailable(SimHashes.Diamond.CreateTag()) / 0.05f;
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
			if (pOIAtCurrentLocation != null && (bool)pOIAtCurrentLocation.GetComponent<HarvestablePOIClusterGridEntity>())
			{
				HarvestablePOIStates.Instance sMI = pOIAtCurrentLocation.GetSMI<HarvestablePOIStates.Instance>();
				if (!sMI.POICanBeHarvested())
				{
					base.sm.canHarvest.Set(value: false, this);
					return false;
				}
				Dictionary<SimHashes, float> elementsWithWeights = sMI.configuration.GetElementsWithWeights();
				foreach (KeyValuePair<SimHashes, float> item in elementsWithWeights)
				{
					SimHashes key = item.Key;
					Element element = ElementLoader.FindElementByHash(key);
					CargoBay.CargoType cargoType = CargoBay.ElementStateToCargoTypes[element.state & Element.State.Solid];
					List<CargoBayCluster> cargoBaysOfType = component.GetCargoBaysOfType(cargoType);
					if (cargoBaysOfType == null || cargoBaysOfType.Count <= 0)
					{
						base.sm.canHarvest.Set(value: false, this);
						return false;
					}
					foreach (CargoBayCluster item2 in cargoBaysOfType)
					{
						if (item2.storage.RemainingCapacity() > 0f)
						{
							base.sm.canHarvest.Set(value: true, this);
							return true;
						}
					}
				}
			}
			base.sm.canHarvest.Set(value: false, this);
			return false;
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
		grounded.TagTransition(GameTags.RocketNotOnGround, not_grounded);
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
			StatesInstance.RemoveHarvestStatusItems(smi.master.gameObject.GetComponent<RocketModuleCluster>().CraftInterface.gameObject);
		}).Enter(delegate(StatesInstance smi)
		{
			StatesInstance.AddHarvestStatusItems(smi.master.gameObject.GetComponent<RocketModuleCluster>().CraftInterface.gameObject, smi.def.harvestSpeed);
		})
			.Update(delegate(StatesInstance smi, float dt)
			{
				smi.HarvestFromPOI(dt);
				lastHarvestTime.Set(Time.time, smi);
			}, UpdateRate.SIM_4000ms)
			.ParamTransition(canHarvest, not_grounded.not_harvesting, GameStateMachine<ResourceHarvestModule, StatesInstance, IStateMachineTarget, Def>.IsFalse);
	}
}
