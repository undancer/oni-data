using KSerialization;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SweepBotStation")]
public class SweepBotStation : KMonoBehaviour
{
	[Serialize]
	public Ref<KSelectable> sweepBot;

	[Serialize]
	public string storedName;

	private Operational.Flag dockedRobot = new Operational.Flag("dockedRobot", Operational.Flag.Type.Functional);

	private MeterController meter;

	private Storage sweepStorage;

	private Storage botMaterialStorage;

	private SchedulerHandle newSweepyHandle;

	private static readonly EventSystem.IntraObjectHandler<SweepBotStation> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<SweepBotStation>(delegate(SweepBotStation component, object data)
	{
		component.OnOperationalChanged(data);
	});

	private int refreshSweepbotHandle = -1;

	private int sweepBotNameChangeHandle = -1;

	protected override void OnPrefabInit()
	{
		Initialize(use_logic_meter: false);
		Subscribe(-592767678, OnOperationalChangedDelegate);
	}

	protected void Initialize(bool use_logic_meter)
	{
		base.OnPrefabInit();
		GetComponent<Operational>().SetFlag(dockedRobot, value: false);
	}

	protected override void OnSpawn()
	{
		Subscribe(-1697596308, OnStorageChanged);
		meter = new MeterController(base.gameObject.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, "meter_frame", "meter_level");
		botMaterialStorage = GetComponents<Storage>()[0];
		sweepStorage = GetComponents<Storage>()[1];
		if (sweepBot == null || sweepBot.Get() == null)
		{
			RequestNewSweepBot();
		}
		else
		{
			StorageUnloadMonitor.Instance sMI = sweepBot.Get().GetSMI<StorageUnloadMonitor.Instance>();
			sMI.sm.sweepLocker.Set(sweepStorage, sMI);
			RefreshSweepBotSubscription();
		}
		UpdateMeter();
		UpdateNameDisplay();
	}

	private void RequestNewSweepBot(object data = null)
	{
		if (botMaterialStorage.FindFirstWithMass(GameTags.RefinedMetal, SweepBotConfig.MASS) == null)
		{
			FetchList2 fetchList = new FetchList2(botMaterialStorage, Db.Get().ChoreTypes.Fetch);
			fetchList.Add(GameTags.RefinedMetal, null, null, SweepBotConfig.MASS);
			fetchList.Submit(null, check_storage_contents: true);
		}
		else
		{
			MakeNewSweepBot();
		}
	}

	private void MakeNewSweepBot(object data = null)
	{
		if (newSweepyHandle.IsValid || botMaterialStorage.GetAmountAvailable(GameTags.RefinedMetal) < SweepBotConfig.MASS)
		{
			return;
		}
		PrimaryElement primaryElement = botMaterialStorage.FindFirstWithMass(GameTags.RefinedMetal, SweepBotConfig.MASS);
		if (primaryElement == null)
		{
			return;
		}
		SimHashes sweepBotMaterial = primaryElement.ElementID;
		primaryElement.Mass -= SweepBotConfig.MASS;
		UpdateMeter();
		newSweepyHandle = GameScheduler.Instance.Schedule("MakeSweepy", 2f, delegate
		{
			GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab("SweepBot"), Grid.CellToPos(Grid.CellRight(Grid.PosToCell(base.gameObject))), Grid.SceneLayer.Creatures);
			gameObject.SetActive(value: true);
			sweepBot = new Ref<KSelectable>(gameObject.GetComponent<KSelectable>());
			if (!string.IsNullOrEmpty(storedName))
			{
				sweepBot.Get().GetComponent<UserNameable>().SetName(storedName);
			}
			UpdateNameDisplay();
			StorageUnloadMonitor.Instance sMI = gameObject.GetSMI<StorageUnloadMonitor.Instance>();
			sMI.sm.sweepLocker.Set(sweepStorage, sMI);
			sweepBot.Get().GetComponent<PrimaryElement>().ElementID = sweepBotMaterial;
			RefreshSweepBotSubscription();
			newSweepyHandle.ClearScheduler();
		});
		GetComponent<KBatchedAnimController>().Play("newsweepy");
	}

	private void RefreshSweepBotSubscription()
	{
		if (refreshSweepbotHandle != -1)
		{
			sweepBot.Get().Unsubscribe(refreshSweepbotHandle);
			sweepBot.Get().Unsubscribe(sweepBotNameChangeHandle);
		}
		refreshSweepbotHandle = sweepBot.Get().Subscribe(1969584890, RequestNewSweepBot);
		sweepBotNameChangeHandle = sweepBot.Get().Subscribe(1102426921, UpdateStoredName);
	}

	private void UpdateStoredName(object data)
	{
		storedName = (string)data;
		UpdateNameDisplay();
	}

	private void UpdateNameDisplay()
	{
		if (string.IsNullOrEmpty(storedName))
		{
			GetComponent<KSelectable>().SetName(string.Format(BUILDINGS.PREFABS.SWEEPBOTSTATION.NAMEDSTATION, ROBOTS.MODELS.SWEEPBOT.NAME));
		}
		else
		{
			GetComponent<KSelectable>().SetName(string.Format(BUILDINGS.PREFABS.SWEEPBOTSTATION.NAMEDSTATION, storedName));
		}
		NameDisplayScreen.Instance.UpdateName(base.gameObject);
	}

	public void DockRobot(bool docked)
	{
		GetComponent<Operational>().SetFlag(dockedRobot, docked);
	}

	public void StartCharging()
	{
		GetComponent<KBatchedAnimController>().Queue("sleep_pre");
		GetComponent<KBatchedAnimController>().Queue("sleep_idle", KAnim.PlayMode.Loop);
	}

	public void StopCharging()
	{
		GetComponent<KBatchedAnimController>().Play("sleep_pst");
		UpdateNameDisplay();
	}

	protected override void OnCleanUp()
	{
		if (newSweepyHandle.IsValid)
		{
			newSweepyHandle.ClearScheduler();
		}
		if (refreshSweepbotHandle != -1 && sweepBot.Get() != null)
		{
			sweepBot.Get().Unsubscribe(refreshSweepbotHandle);
		}
	}

	private void UpdateMeter()
	{
		float maxCapacityMinusStorageMargin = GetMaxCapacityMinusStorageMargin();
		float positionPercent = Mathf.Clamp01(GetAmountStored() / maxCapacityMinusStorageMargin);
		if (meter != null)
		{
			meter.SetPositionPercent(positionPercent);
		}
	}

	private void OnStorageChanged(object data)
	{
		UpdateMeter();
		if (sweepBot == null || sweepBot.Get() == null)
		{
			RequestNewSweepBot();
		}
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		if (component.currentFrame >= component.GetCurrentNumFrames())
		{
			GetComponent<KBatchedAnimController>().Play("remove");
		}
		for (int i = 0; i < sweepStorage.Count; i++)
		{
			sweepStorage[i].GetComponent<Clearable>().MarkForClear(restoringFromSave: false, allowWhenStored: true);
		}
	}

	private void OnOperationalChanged(object data)
	{
		Operational component = GetComponent<Operational>();
		if (component.Flags.ContainsValue(value: false))
		{
			component.SetActive(value: false);
		}
		else
		{
			component.SetActive(value: true);
		}
		if (sweepBot == null || sweepBot.Get() == null)
		{
			RequestNewSweepBot();
		}
	}

	private float GetMaxCapacityMinusStorageMargin()
	{
		return sweepStorage.Capacity() - sweepStorage.storageFullMargin;
	}

	private float GetAmountStored()
	{
		return sweepStorage.MassStored();
	}
}
