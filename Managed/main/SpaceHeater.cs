using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class SpaceHeater : StateMachineComponent<SpaceHeater.StatesInstance>, IGameObjectEffectDescriptor
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, SpaceHeater, object>.GameInstance
	{
		public StatesInstance(SpaceHeater master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, SpaceHeater>
	{
		public class OnlineStates : State
		{
			public State heating;

			public State overtemp;

			public State undermassliquid;

			public State undermassgas;
		}

		public State offline;

		public OnlineStates online;

		private StatusItem statusItemUnderMassLiquid;

		private StatusItem statusItemUnderMassGas;

		private StatusItem statusItemOverTemp;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = offline;
			base.serializable = false;
			statusItemUnderMassLiquid = new StatusItem("statusItemUnderMassLiquid", BUILDING.STATUSITEMS.HEATINGSTALLEDLOWMASS_LIQUID.NAME, BUILDING.STATUSITEMS.HEATINGSTALLEDLOWMASS_LIQUID.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			statusItemUnderMassGas = new StatusItem("statusItemUnderMassGas", BUILDING.STATUSITEMS.HEATINGSTALLEDLOWMASS_GAS.NAME, BUILDING.STATUSITEMS.HEATINGSTALLEDLOWMASS_GAS.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			statusItemOverTemp = new StatusItem("statusItemOverTemp", BUILDING.STATUSITEMS.HEATINGSTALLEDHOTENV.NAME, BUILDING.STATUSITEMS.HEATINGSTALLEDHOTENV.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			statusItemOverTemp.resolveStringCallback = delegate(string str, object obj)
			{
				StatesInstance statesInstance = (StatesInstance)obj;
				return string.Format(str, GameUtil.GetFormattedTemperature(statesInstance.master.TargetTemperature));
			};
			offline.EventTransition(GameHashes.OperationalChanged, online, (StatesInstance smi) => smi.master.operational.IsOperational);
			online.EventTransition(GameHashes.OperationalChanged, offline, (StatesInstance smi) => !smi.master.operational.IsOperational).DefaultState(online.heating).Update("spaceheater_online", delegate(StatesInstance smi, float dt)
			{
				switch (smi.master.MonitorHeating(dt))
				{
				case MonitorState.NotEnoughLiquid:
					smi.GoTo(online.undermassliquid);
					break;
				case MonitorState.NotEnoughGas:
					smi.GoTo(online.undermassgas);
					break;
				case MonitorState.TooHot:
					smi.GoTo(online.overtemp);
					break;
				case MonitorState.ReadyToHeat:
					smi.GoTo(online.heating);
					break;
				}
			}, UpdateRate.SIM_4000ms);
			online.heating.Enter(delegate(StatesInstance smi)
			{
				smi.master.operational.SetActive(value: true);
			}).Exit(delegate(StatesInstance smi)
			{
				smi.master.operational.SetActive(value: false);
			});
			online.undermassliquid.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Heat, statusItemUnderMassLiquid);
			online.undermassgas.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Heat, statusItemUnderMassGas);
			online.overtemp.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Heat, statusItemOverTemp);
		}
	}

	private enum MonitorState
	{
		ReadyToHeat,
		TooHot,
		NotEnoughLiquid,
		NotEnoughGas
	}

	public float targetTemperature = 308.15f;

	public float minimumCellMass;

	public int radius = 2;

	[SerializeField]
	private bool heatLiquid;

	[MyCmpReq]
	private Operational operational;

	private List<int> monitorCells = new List<int>();

	public float TargetTemperature => targetTemperature;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		GameScheduler.Instance.Schedule("InsulationTutorial", 2f, delegate
		{
			Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Insulation);
		});
		base.smi.StartSM();
	}

	public void SetLiquidHeater()
	{
		heatLiquid = true;
	}

	private MonitorState MonitorHeating(float dt)
	{
		monitorCells.Clear();
		GameUtil.GetNonSolidCells(Grid.PosToCell(base.transform.GetPosition()), radius, monitorCells);
		int num = 0;
		float num2 = 0f;
		for (int i = 0; i < monitorCells.Count; i++)
		{
			if (Grid.Mass[monitorCells[i]] > minimumCellMass && ((Grid.Element[monitorCells[i]].IsGas && !heatLiquid) || (Grid.Element[monitorCells[i]].IsLiquid && heatLiquid)))
			{
				num++;
				num2 += Grid.Temperature[monitorCells[i]];
			}
		}
		if (num == 0)
		{
			if (!heatLiquid)
			{
				return MonitorState.NotEnoughGas;
			}
			return MonitorState.NotEnoughLiquid;
		}
		if (num2 / (float)num >= targetTemperature)
		{
			return MonitorState.TooHot;
		}
		return MonitorState.ReadyToHeat;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		Descriptor item = default(Descriptor);
		item.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.HEATER_TARGETTEMPERATURE, GameUtil.GetFormattedTemperature(targetTemperature)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.HEATER_TARGETTEMPERATURE, GameUtil.GetFormattedTemperature(targetTemperature)));
		list.Add(item);
		return list;
	}
}
