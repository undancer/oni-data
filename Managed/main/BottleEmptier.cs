using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class BottleEmptier : StateMachineComponent<BottleEmptier.StatesInstance>, IGameObjectEffectDescriptor
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, BottleEmptier, object>.GameInstance
	{
		private FetchChore chore;

		public MeterController meter { get; private set; }

		public StatesInstance(BottleEmptier smi)
			: base(smi)
		{
			TreeFilterable component = base.master.GetComponent<TreeFilterable>();
			component.OnFilterChanged = (Action<Tag[]>)Delegate.Combine(component.OnFilterChanged, new Action<Tag[]>(OnFilterChanged));
			meter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, "meter_target", "meter_arrow", "meter_scale");
			Subscribe(-1697596308, OnStorageChange);
			Subscribe(644822890, OnOnlyFetchMarkedItemsSettingChanged);
		}

		public void CreateChore()
		{
			GetComponent<KBatchedAnimController>();
			Tag[] tags = GetComponent<TreeFilterable>().GetTags();
			Tag[] forbidden_tags = (base.master.allowManualPumpingStationFetching ? new Tag[0] : new Tag[2]
			{
				GameTags.LiquidSource,
				GameTags.GasSource
			});
			Storage component = GetComponent<Storage>();
			chore = new FetchChore(Db.Get().ChoreTypes.StorageFetch, component, component.Capacity(), tags, null, forbidden_tags);
		}

		public void CancelChore()
		{
			if (chore != null)
			{
				chore.Cancel("Storage Changed");
				chore = null;
			}
		}

		public void RefreshChore()
		{
			GoTo(base.sm.unoperational);
		}

		private void OnFilterChanged(Tag[] tags)
		{
			RefreshChore();
		}

		private void OnStorageChange(object data)
		{
			Storage component = GetComponent<Storage>();
			meter.SetPositionPercent(Mathf.Clamp01(component.RemainingCapacity() / component.capacityKg));
		}

		private void OnOnlyFetchMarkedItemsSettingChanged(object data)
		{
			RefreshChore();
		}

		public void StartMeter()
		{
			PrimaryElement firstPrimaryElement = GetFirstPrimaryElement();
			if (!(firstPrimaryElement == null))
			{
				meter.SetSymbolTint(new KAnimHashedString("meter_fill"), firstPrimaryElement.Element.substance.colour);
				meter.SetSymbolTint(new KAnimHashedString("water1"), firstPrimaryElement.Element.substance.colour);
				GetComponent<KBatchedAnimController>().SetSymbolTint(new KAnimHashedString("leak_ceiling"), firstPrimaryElement.Element.substance.colour);
			}
		}

		private PrimaryElement GetFirstPrimaryElement()
		{
			Storage component = GetComponent<Storage>();
			for (int i = 0; i < component.Count; i++)
			{
				GameObject gameObject = component[i];
				if (!(gameObject == null))
				{
					PrimaryElement component2 = gameObject.GetComponent<PrimaryElement>();
					if (!(component2 == null))
					{
						return component2;
					}
				}
			}
			return null;
		}

		public void Emit(float dt)
		{
			PrimaryElement firstPrimaryElement = GetFirstPrimaryElement();
			if (firstPrimaryElement == null)
			{
				return;
			}
			Storage component = GetComponent<Storage>();
			float num = Mathf.Min(firstPrimaryElement.Mass, base.master.emptyRate * dt);
			if (!(num <= 0f))
			{
				Tag prefabTag = firstPrimaryElement.GetComponent<KPrefabID>().PrefabTag;
				component.ConsumeAndGetDisease(prefabTag, num, out var amount_consumed, out var disease_info, out var aggregate_temperature);
				Vector3 position = base.transform.GetPosition();
				position.y += 1.8f;
				bool flag = GetComponent<Rotatable>().GetOrientation() == Orientation.FlipH;
				position.x += (flag ? (-0.2f) : 0.2f);
				int num2 = Grid.PosToCell(position) + ((!flag) ? 1 : (-1));
				if (Grid.Solid[num2])
				{
					num2 += (flag ? 1 : (-1));
				}
				Element element = firstPrimaryElement.Element;
				byte idx = element.idx;
				if (element.IsLiquid)
				{
					FallingWater.instance.AddParticle(num2, idx, amount_consumed, aggregate_temperature, disease_info.idx, disease_info.count, skip_sound: true);
				}
				else
				{
					SimMessages.ModifyCell(num2, idx, aggregate_temperature, amount_consumed, disease_info.idx, disease_info.count);
				}
			}
		}
	}

	public class States : GameStateMachine<States, StatesInstance, BottleEmptier>
	{
		private StatusItem statusItem;

		public State unoperational;

		public State waitingfordelivery;

		public State emptying;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = waitingfordelivery;
			statusItem = new StatusItem("BottleEmptier", "", "", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			statusItem.resolveStringCallback = delegate(string str, object data)
			{
				BottleEmptier bottleEmptier2 = (BottleEmptier)data;
				if (bottleEmptier2 == null)
				{
					return str;
				}
				return bottleEmptier2.allowManualPumpingStationFetching ? ((string)(bottleEmptier2.isGasEmptier ? BUILDING.STATUSITEMS.CANISTER_EMPTIER.ALLOWED.NAME : BUILDING.STATUSITEMS.BOTTLE_EMPTIER.ALLOWED.NAME)) : ((string)(bottleEmptier2.isGasEmptier ? BUILDING.STATUSITEMS.CANISTER_EMPTIER.DENIED.NAME : BUILDING.STATUSITEMS.BOTTLE_EMPTIER.DENIED.NAME));
			};
			statusItem.resolveTooltipCallback = delegate(string str, object data)
			{
				BottleEmptier bottleEmptier = (BottleEmptier)data;
				if (bottleEmptier == null)
				{
					return str;
				}
				if (bottleEmptier.allowManualPumpingStationFetching)
				{
					if (bottleEmptier.isGasEmptier)
					{
						return BUILDING.STATUSITEMS.CANISTER_EMPTIER.ALLOWED.TOOLTIP;
					}
					return BUILDING.STATUSITEMS.BOTTLE_EMPTIER.ALLOWED.TOOLTIP;
				}
				return bottleEmptier.isGasEmptier ? ((string)BUILDING.STATUSITEMS.CANISTER_EMPTIER.DENIED.TOOLTIP) : ((string)BUILDING.STATUSITEMS.BOTTLE_EMPTIER.DENIED.TOOLTIP);
			};
			root.ToggleStatusItem(statusItem, (StatesInstance smi) => smi.master);
			unoperational.TagTransition(GameTags.Operational, waitingfordelivery).PlayAnim("off");
			waitingfordelivery.TagTransition(GameTags.Operational, unoperational, on_remove: true).EventTransition(GameHashes.OnStorageChange, emptying, (StatesInstance smi) => !smi.GetComponent<Storage>().IsEmpty()).Enter("CreateChore", delegate(StatesInstance smi)
			{
				smi.CreateChore();
			})
				.Exit("CancelChore", delegate(StatesInstance smi)
				{
					smi.CancelChore();
				})
				.PlayAnim("on");
			emptying.TagTransition(GameTags.Operational, unoperational, on_remove: true).EventTransition(GameHashes.OnStorageChange, waitingfordelivery, (StatesInstance smi) => smi.GetComponent<Storage>().IsEmpty()).Enter("StartMeter", delegate(StatesInstance smi)
			{
				smi.StartMeter();
			})
				.Update("Emit", delegate(StatesInstance smi, float dt)
				{
					smi.Emit(dt);
				})
				.PlayAnim("working_loop", KAnim.PlayMode.Loop);
		}
	}

	public float emptyRate = 10f;

	[Serialize]
	public bool allowManualPumpingStationFetching;

	public bool isGasEmptier;

	private static readonly EventSystem.IntraObjectHandler<BottleEmptier> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<BottleEmptier>(delegate(BottleEmptier component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	private static readonly EventSystem.IntraObjectHandler<BottleEmptier> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<BottleEmptier>(delegate(BottleEmptier component, object data)
	{
		component.OnCopySettings(data);
	});

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		Subscribe(493375141, OnRefreshUserMenuDelegate);
		Subscribe(-905833192, OnCopySettingsDelegate);
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		return null;
	}

	private void OnChangeAllowManualPumpingStationFetching()
	{
		allowManualPumpingStationFetching = !allowManualPumpingStationFetching;
		base.smi.RefreshChore();
	}

	private void OnRefreshUserMenu(object data)
	{
		if (isGasEmptier)
		{
			KIconButtonMenu.ButtonInfo button = (allowManualPumpingStationFetching ? new KIconButtonMenu.ButtonInfo("action_bottler_delivery", UI.USERMENUACTIONS.MANUAL_PUMP_DELIVERY.DENIED_GAS.NAME, OnChangeAllowManualPumpingStationFetching, Action.NumActions, null, null, null, UI.USERMENUACTIONS.MANUAL_PUMP_DELIVERY.DENIED_GAS.TOOLTIP) : new KIconButtonMenu.ButtonInfo("action_bottler_delivery", UI.USERMENUACTIONS.MANUAL_PUMP_DELIVERY.ALLOWED_GAS.NAME, OnChangeAllowManualPumpingStationFetching, Action.NumActions, null, null, null, UI.USERMENUACTIONS.MANUAL_PUMP_DELIVERY.ALLOWED_GAS.TOOLTIP));
			Game.Instance.userMenu.AddButton(base.gameObject, button, 0.4f);
		}
		else
		{
			KIconButtonMenu.ButtonInfo button2 = (allowManualPumpingStationFetching ? new KIconButtonMenu.ButtonInfo("action_bottler_delivery", UI.USERMENUACTIONS.MANUAL_PUMP_DELIVERY.DENIED.NAME, OnChangeAllowManualPumpingStationFetching, Action.NumActions, null, null, null, UI.USERMENUACTIONS.MANUAL_PUMP_DELIVERY.DENIED.TOOLTIP) : new KIconButtonMenu.ButtonInfo("action_bottler_delivery", UI.USERMENUACTIONS.MANUAL_PUMP_DELIVERY.ALLOWED.NAME, OnChangeAllowManualPumpingStationFetching, Action.NumActions, null, null, null, UI.USERMENUACTIONS.MANUAL_PUMP_DELIVERY.ALLOWED.TOOLTIP));
			Game.Instance.userMenu.AddButton(base.gameObject, button2, 0.4f);
		}
	}

	private void OnCopySettings(object data)
	{
		BottleEmptier component = ((GameObject)data).GetComponent<BottleEmptier>();
		allowManualPumpingStationFetching = component.allowManualPumpingStationFetching;
		base.smi.RefreshChore();
	}
}
