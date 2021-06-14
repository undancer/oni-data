using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Tutorial")]
public class Tutorial : KMonoBehaviour, IRender1000ms
{
	public enum TutorialMessages
	{
		TM_Basics,
		TM_Welcome,
		TM_StressManagement,
		TM_Scheduling,
		TM_Mopping,
		TM_Locomotion,
		TM_Priorities,
		TM_FetchingWater,
		TM_ThermalComfort,
		TM_OverheatingBuildings,
		TM_LotsOfGerms,
		TM_DiseaseCooking,
		TM_Suits,
		TM_Morale,
		TM_Schedule,
		TM_Digging,
		TM_Power,
		TM_Insulation,
		TM_Plumbing,
		TM_Radiation,
		TM_COUNT
	}

	private delegate bool HideConditionDelegate();

	private delegate bool RequirementSatisfiedDelegate();

	private class Item
	{
		public Notification notification;

		public HideConditionDelegate hideCondition;

		public RequirementSatisfiedDelegate requirementSatisfied;

		public float minTimeToNotify;

		public float lastNotifyTime;
	}

	[MyCmpAdd]
	private Notifier notifier;

	[Serialize]
	private SerializedList<TutorialMessages> tutorialMessagesRemaining = new SerializedList<TutorialMessages>();

	private const string HIDDEN_TUTORIAL_PREF_KEY_PREFIX = "HideTutorial_";

	public const string HIDDEN_TUTORIAL_PREF_BUTTON_KEY = "HideTutorial_CheckState";

	private Dictionary<TutorialMessages, bool> hiddenTutorialMessages = new Dictionary<TutorialMessages, bool>();

	private int debugMessageCount = 0;

	private bool queuedPrioritiesMessage = false;

	private const float LOW_RATION_AMOUNT = 1f;

	private List<List<Item>> itemTree = new List<List<Item>>();

	private List<Item> warningItems = new List<Item>();

	private Vector3 notifierPosition;

	public List<GameObject> oxygenGenerators = new List<GameObject>();

	private int focusedOxygenGenerator = 0;

	private int focusedUnrefrigFood = -1;

	public static Tutorial Instance
	{
		get;
		private set;
	}

	public static void ResetHiddenTutorialMessages()
	{
		if (Instance != null)
		{
			Instance.tutorialMessagesRemaining.Clear();
		}
		foreach (TutorialMessages value in Enum.GetValues(typeof(TutorialMessages)))
		{
			string key = "HideTutorial_" + value;
			KPlayerPrefs.SetInt(key, 0);
			if (Instance != null)
			{
				Instance.tutorialMessagesRemaining.Add(value);
				Instance.hiddenTutorialMessages[value] = false;
			}
		}
		KPlayerPrefs.SetInt("HideTutorial_CheckState", 0);
	}

	private void LoadHiddenTutorialMessages()
	{
		foreach (TutorialMessages value2 in Enum.GetValues(typeof(TutorialMessages)))
		{
			string key2 = "HideTutorial_" + value2;
			bool value = KPlayerPrefs.GetInt(key2, 0) != 0;
			hiddenTutorialMessages[value2] = value;
		}
	}

	public void HideTutorialMessage(TutorialMessages message)
	{
		hiddenTutorialMessages[message] = true;
		string key = "HideTutorial_" + message;
		KPlayerPrefs.SetInt(key, 1);
	}

	public static void DestroyInstance()
	{
		Instance = null;
	}

	private void UpdateNotifierPosition()
	{
		if (notifierPosition == Vector3.zero)
		{
			GameObject activeTelepad = GameUtil.GetActiveTelepad();
			if (activeTelepad != null)
			{
				notifierPosition = activeTelepad.transform.GetPosition();
			}
		}
		notifier.transform.SetPosition(notifierPosition);
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
		LoadHiddenTutorialMessages();
	}

	protected override void OnSpawn()
	{
		if (tutorialMessagesRemaining.Count == 0)
		{
			for (int i = 0; i <= 20; i++)
			{
				tutorialMessagesRemaining.Add((TutorialMessages)i);
			}
		}
		List<Item> list = new List<Item>();
		list.Add(new Item
		{
			notification = new Notification(MISC.NOTIFICATIONS.NEEDTOILET.NAME, NotificationType.Tutorial, (List<Notification> n, object d) => MISC.NOTIFICATIONS.NEEDTOILET.TOOLTIP.text, null, expires: true, 5f, delegate
			{
				PlanScreen.Instance.OpenCategoryByName("Plumbing");
			}),
			requirementSatisfied = ToiletExists
		});
		itemTree.Add(list);
		List<Item> list2 = new List<Item>();
		list2.Add(new Item
		{
			notification = new Notification(MISC.NOTIFICATIONS.NEEDFOOD.NAME, NotificationType.Tutorial, (List<Notification> n, object d) => MISC.NOTIFICATIONS.NEEDFOOD.TOOLTIP.text, null, expires: true, 20f, delegate
			{
				PlanScreen.Instance.OpenCategoryByName("Food");
			}),
			requirementSatisfied = FoodSourceExists
		});
		list2.Add(new Item
		{
			notification = new Notification(MISC.NOTIFICATIONS.THERMALCOMFORT.NAME, NotificationType.Tutorial, (List<Notification> n, object d) => MISC.NOTIFICATIONS.THERMALCOMFORT.TOOLTIP.text)
		});
		itemTree.Add(list2);
		List<Item> list3 = new List<Item>();
		list3.Add(new Item
		{
			notification = new Notification(MISC.NOTIFICATIONS.HYGENE_NEEDED.NAME, NotificationType.Tutorial, (List<Notification> n, object d) => MISC.NOTIFICATIONS.HYGENE_NEEDED.TOOLTIP, null, expires: true, 20f, delegate
			{
				PlanScreen.Instance.OpenCategoryByName("Medicine");
			}),
			requirementSatisfied = HygeneExists
		});
		itemTree.Add(list3);
		warningItems.Add(new Item
		{
			notification = new Notification(MISC.NOTIFICATIONS.NO_OXYGEN_GENERATOR.NAME, NotificationType.Tutorial, (List<Notification> n, object d) => MISC.NOTIFICATIONS.NO_OXYGEN_GENERATOR.TOOLTIP, null, expires: false, 0f, delegate
			{
				PlanScreen.Instance.OpenCategoryByName("Oxygen");
			}),
			requirementSatisfied = OxygenGeneratorBuilt,
			minTimeToNotify = 80f,
			lastNotifyTime = 0f
		});
		warningItems.Add(new Item
		{
			notification = new Notification(MISC.NOTIFICATIONS.INSUFFICIENTOXYGENLASTCYCLE.NAME, NotificationType.Tutorial, OnOxygenTooltip, null, expires: false, 0f, delegate
			{
				ZoomToNextOxygenGenerator();
			}),
			hideCondition = OxygenGeneratorNotBuilt,
			requirementSatisfied = SufficientOxygenLastCycleAndThisCycle,
			minTimeToNotify = 80f,
			lastNotifyTime = 0f
		});
		warningItems.Add(new Item
		{
			notification = new Notification(MISC.NOTIFICATIONS.UNREFRIGERATEDFOOD.NAME, NotificationType.Tutorial, UnrefrigeratedFoodTooltip, null, expires: false, 0f, delegate
			{
				ZoomToNextUnrefrigeratedFood();
			}),
			requirementSatisfied = FoodIsRefrigerated,
			minTimeToNotify = 6f,
			lastNotifyTime = 0f
		});
		warningItems.Add(new Item
		{
			notification = new Notification(MISC.NOTIFICATIONS.NO_MEDICAL_COTS.NAME, NotificationType.Bad, (List<Notification> n, object o) => MISC.NOTIFICATIONS.NO_MEDICAL_COTS.TOOLTIP, null, expires: false, 0f, delegate
			{
				PlanScreen.Instance.OpenCategoryByName("Medicine");
			}),
			requirementSatisfied = CanTreatSickDuplicant,
			minTimeToNotify = 10f,
			lastNotifyTime = 0f
		});
		warningItems.Add(new Item
		{
			notification = new Notification(string.Format(UI.ENDOFDAYREPORT.TRAVELTIMEWARNING.WARNING_TITLE), NotificationType.BadMinor, (List<Notification> n, object d) => string.Format(UI.ENDOFDAYREPORT.TRAVELTIMEWARNING.WARNING_MESSAGE, GameUtil.GetFormattedPercent(40f)), null, expires: true, 0f, delegate
			{
				ManagementMenu.Instance.OpenReports(GameClock.Instance.GetCycle());
			}),
			requirementSatisfied = LongTravelTimes,
			minTimeToNotify = 1f,
			lastNotifyTime = 0f
		});
	}

	public Message TutorialMessage(TutorialMessages tm, bool queueMessage = true)
	{
		bool flag = false;
		Message message = null;
		switch (tm)
		{
		case TutorialMessages.TM_Basics:
			message = new TutorialMessage(TutorialMessages.TM_Basics, MISC.NOTIFICATIONS.BASICCONTROLS.NAME, MISC.NOTIFICATIONS.BASICCONTROLS.MESSAGEBODY, MISC.NOTIFICATIONS.BASICCONTROLS.TOOLTIP);
			break;
		case TutorialMessages.TM_Welcome:
			message = new TutorialMessage(TutorialMessages.TM_Welcome, MISC.NOTIFICATIONS.WELCOMEMESSAGE.NAME, MISC.NOTIFICATIONS.WELCOMEMESSAGE.MESSAGEBODY, MISC.NOTIFICATIONS.WELCOMEMESSAGE.TOOLTIP);
			break;
		case TutorialMessages.TM_StressManagement:
			message = new TutorialMessage(TutorialMessages.TM_StressManagement, MISC.NOTIFICATIONS.STRESSMANAGEMENTMESSAGE.NAME, MISC.NOTIFICATIONS.STRESSMANAGEMENTMESSAGE.MESSAGEBODY, MISC.NOTIFICATIONS.STRESSMANAGEMENTMESSAGE.TOOLTIP, null, null, null, "hud_stress");
			break;
		case TutorialMessages.TM_Scheduling:
			flag = true;
			break;
		case TutorialMessages.TM_Mopping:
			message = new TutorialMessage(TutorialMessages.TM_Mopping, MISC.NOTIFICATIONS.MOPPINGMESSAGE.NAME, MISC.NOTIFICATIONS.MOPPINGMESSAGE.MESSAGEBODY, MISC.NOTIFICATIONS.MOPPINGMESSAGE.TOOLTIP, null, null, null, "icon_action_mop");
			break;
		case TutorialMessages.TM_Locomotion:
			message = new TutorialMessage(TutorialMessages.TM_Locomotion, MISC.NOTIFICATIONS.LOCOMOTIONMESSAGE.NAME, MISC.NOTIFICATIONS.LOCOMOTIONMESSAGE.MESSAGEBODY, MISC.NOTIFICATIONS.LOCOMOTIONMESSAGE.TOOLTIP, "tutorials\\Locomotion", "Tute_Locomotion", VIDEOS.LOCOMOTION, "action_navigable_regions");
			break;
		case TutorialMessages.TM_Priorities:
			message = new TutorialMessage(TutorialMessages.TM_Priorities, MISC.NOTIFICATIONS.PRIORITIESMESSAGE.NAME, MISC.NOTIFICATIONS.PRIORITIESMESSAGE.MESSAGEBODY, MISC.NOTIFICATIONS.PRIORITIESMESSAGE.TOOLTIP, null, null, null, "icon_action_prioritize");
			break;
		case TutorialMessages.TM_FetchingWater:
			message = new TutorialMessage(TutorialMessages.TM_FetchingWater, MISC.NOTIFICATIONS.FETCHINGWATERMESSAGE.NAME, MISC.NOTIFICATIONS.FETCHINGWATERMESSAGE.MESSAGEBODY, MISC.NOTIFICATIONS.FETCHINGWATERMESSAGE.TOOLTIP, null, null, null, "element_liquid");
			break;
		case TutorialMessages.TM_ThermalComfort:
			message = new TutorialMessage(TutorialMessages.TM_ThermalComfort, MISC.NOTIFICATIONS.THERMALCOMFORT.NAME, MISC.NOTIFICATIONS.THERMALCOMFORT.MESSAGEBODY, MISC.NOTIFICATIONS.THERMALCOMFORT.TOOLTIP, null, null, null, "temperature");
			break;
		case TutorialMessages.TM_OverheatingBuildings:
			message = new TutorialMessage(TutorialMessages.TM_OverheatingBuildings, MISC.NOTIFICATIONS.TUTORIAL_OVERHEATING.NAME, MISC.NOTIFICATIONS.TUTORIAL_OVERHEATING.MESSAGEBODY, MISC.NOTIFICATIONS.TUTORIAL_OVERHEATING.TOOLTIP, null, null, null, "temperature");
			break;
		case TutorialMessages.TM_LotsOfGerms:
			message = new TutorialMessage(TutorialMessages.TM_LotsOfGerms, MISC.NOTIFICATIONS.LOTS_OF_GERMS.NAME, MISC.NOTIFICATIONS.LOTS_OF_GERMS.MESSAGEBODY, MISC.NOTIFICATIONS.LOTS_OF_GERMS.TOOLTIP, null, null, null, "overlay_disease");
			break;
		case TutorialMessages.TM_DiseaseCooking:
			message = new TutorialMessage(TutorialMessages.TM_DiseaseCooking, MISC.NOTIFICATIONS.DISEASE_COOKING.NAME, MISC.NOTIFICATIONS.DISEASE_COOKING.MESSAGEBODY, MISC.NOTIFICATIONS.DISEASE_COOKING.TOOLTIP, null, null, null, "icon_category_food");
			break;
		case TutorialMessages.TM_Suits:
			message = new TutorialMessage(TutorialMessages.TM_Suits, MISC.NOTIFICATIONS.SUITS.NAME, MISC.NOTIFICATIONS.SUITS.MESSAGEBODY, MISC.NOTIFICATIONS.SUITS.TOOLTIP, null, null, null, "overlay_suit");
			break;
		case TutorialMessages.TM_Morale:
			message = new TutorialMessage(TutorialMessages.TM_Morale, MISC.NOTIFICATIONS.MORALE.NAME, MISC.NOTIFICATIONS.MORALE.MESSAGEBODY, MISC.NOTIFICATIONS.MORALE.TOOLTIP, "tutorials\\Morale", "Tute_Morale", VIDEOS.MORALE, "icon_category_morale");
			break;
		case TutorialMessages.TM_Schedule:
			message = new TutorialMessage(TutorialMessages.TM_Schedule, MISC.NOTIFICATIONS.SCHEDULEMESSAGE.NAME, MISC.NOTIFICATIONS.SCHEDULEMESSAGE.MESSAGEBODY, MISC.NOTIFICATIONS.SCHEDULEMESSAGE.TOOLTIP, null, null, null, "OverviewUI_schedule2_icon");
			break;
		case TutorialMessages.TM_Power:
			message = new TutorialMessage(TutorialMessages.TM_Power, MISC.NOTIFICATIONS.POWER.NAME, MISC.NOTIFICATIONS.POWER.MESSAGEBODY, MISC.NOTIFICATIONS.POWER.TOOLTIP, "tutorials\\Power", "Tute_Power", VIDEOS.POWER, "overlay_power");
			break;
		case TutorialMessages.TM_Digging:
			message = new TutorialMessage(TutorialMessages.TM_Digging, MISC.NOTIFICATIONS.DIGGING.NAME, MISC.NOTIFICATIONS.DIGGING.MESSAGEBODY, MISC.NOTIFICATIONS.DIGGING.TOOLTIP, "tutorials\\Digging", "Tute_Digging", VIDEOS.DIGGING, "icon_action_dig");
			break;
		case TutorialMessages.TM_Insulation:
			message = new TutorialMessage(TutorialMessages.TM_Insulation, MISC.NOTIFICATIONS.INSULATION.NAME, MISC.NOTIFICATIONS.INSULATION.MESSAGEBODY, MISC.NOTIFICATIONS.INSULATION.TOOLTIP, "tutorials\\Insulation", "Tute_Insulation", VIDEOS.INSULATION, "icon_thermal_conductivity");
			break;
		case TutorialMessages.TM_Plumbing:
			message = new TutorialMessage(TutorialMessages.TM_Plumbing, MISC.NOTIFICATIONS.PLUMBING.NAME, MISC.NOTIFICATIONS.PLUMBING.MESSAGEBODY, MISC.NOTIFICATIONS.PLUMBING.TOOLTIP, "tutorials\\Piping", "Tute_Plumbing", VIDEOS.PLUMBING, "icon_category_plumbing");
			break;
		case TutorialMessages.TM_Radiation:
			message = new TutorialMessage(TutorialMessages.TM_Radiation, MISC.NOTIFICATIONS.RADIATION.NAME, MISC.NOTIFICATIONS.RADIATION.MESSAGEBODY, MISC.NOTIFICATIONS.RADIATION.TOOLTIP, null, null, null, "icon_category_radiation");
			break;
		}
		Debug.Assert(message != null || flag, $"No Tutorial message: {tm.ToString()}");
		if (queueMessage)
		{
			Debug.Assert(!flag, "Attempted to queue deprecated Tutorial Message " + tm);
			if (!tutorialMessagesRemaining.Contains(tm))
			{
				return null;
			}
			if (hiddenTutorialMessages.ContainsKey(tm) && hiddenTutorialMessages[tm])
			{
				return null;
			}
			tutorialMessagesRemaining.Remove(tm);
			Messenger.Instance.QueueMessage(message);
		}
		return message;
	}

	private string OnOxygenTooltip(List<Notification> notifications, object data)
	{
		ReportManager.ReportEntry entry = ReportManager.Instance.YesterdaysReport.GetEntry(ReportManager.ReportType.OxygenCreated);
		string text = MISC.NOTIFICATIONS.INSUFFICIENTOXYGENLASTCYCLE.TOOLTIP;
		text = text.Replace("{EmittingRate}", GameUtil.GetFormattedMass(entry.Positive));
		return text.Replace("{ConsumptionRate}", GameUtil.GetFormattedMass(Mathf.Abs(entry.Negative)));
	}

	private string UnrefrigeratedFoodTooltip(List<Notification> notifications, object data)
	{
		string text = MISC.NOTIFICATIONS.UNREFRIGERATEDFOOD.TOOLTIP;
		ListPool<Pickupable, Tutorial>.PooledList pooledList = ListPool<Pickupable, Tutorial>.Allocate();
		GetUnrefrigeratedFood(pooledList);
		for (int i = 0; i < pooledList.Count; i++)
		{
			text = text + "\n" + pooledList[i].GetProperName();
		}
		pooledList.Recycle();
		return text;
	}

	private string OnLowFoodTooltip(List<Notification> notifications, object data)
	{
		Debug.Assert(((WorldContainer)data).id == ClusterManager.Instance.activeWorldId);
		float calories = RationTracker.Get().CountRations(null, ((WorldContainer)data).worldInventory);
		float f = (float)Components.LiveMinionIdentities.GetWorldItems(((WorldContainer)data).id).Count * -1000000f;
		return string.Format(MISC.NOTIFICATIONS.FOODLOW.TOOLTIP, GameUtil.GetFormattedCalories(calories), GameUtil.GetFormattedCalories(Mathf.Abs(f)));
	}

	public void DebugNotification()
	{
		string text = "";
		NotificationType type;
		if (debugMessageCount % 3 == 0)
		{
			type = NotificationType.Tutorial;
			text = "Warning message e.g. \"not enough oxygen\" uses Warning Color";
		}
		else if (debugMessageCount % 3 == 1)
		{
			type = NotificationType.BadMinor;
			text = "Normal message e.g. Idle. Uses Normal Color BG";
		}
		else
		{
			type = NotificationType.Bad;
			text = "Urgent important message. Uses Bad Color BG";
		}
		Notification notification = new Notification($"{text} ({debugMessageCount++.ToString()})", type, (List<Notification> n, object d) => MISC.NOTIFICATIONS.NEEDTOILET.TOOLTIP.text);
		notifier.Add(notification);
	}

	public void DebugNotificationMessage()
	{
		Message message = new GenericMessage("This is a message notification. " + debugMessageCount++, MISC.NOTIFICATIONS.LOCOMOTIONMESSAGE.MESSAGEBODY, MISC.NOTIFICATIONS.LOCOMOTIONMESSAGE.TOOLTIP);
		Messenger.Instance.QueueMessage(message);
	}

	public void Render1000ms(float dt)
	{
		if (App.isLoading || Components.LiveMinionIdentities.Count == 0)
		{
			return;
		}
		if (itemTree.Count > 0)
		{
			List<Item> list = itemTree[0];
			for (int num = list.Count - 1; num >= 0; num--)
			{
				Item item = list[num];
				if (item != null)
				{
					if (item.requirementSatisfied == null || item.requirementSatisfied())
					{
						item.notification.Clear();
						list.RemoveAt(num);
					}
					else if (item.hideCondition != null && item.hideCondition())
					{
						item.notification.Clear();
						list.RemoveAt(num);
					}
					else
					{
						UpdateNotifierPosition();
						notifier.Add(item.notification);
					}
				}
			}
			if (list.Count == 0)
			{
				itemTree.RemoveAt(0);
			}
		}
		foreach (Item warningItem in warningItems)
		{
			if (warningItem.requirementSatisfied())
			{
				warningItem.notification.Clear();
				warningItem.lastNotifyTime = Time.time;
			}
			else if (warningItem.hideCondition != null && warningItem.hideCondition())
			{
				warningItem.notification.Clear();
				warningItem.lastNotifyTime = Time.time;
			}
			else if (warningItem.lastNotifyTime == 0f || Time.time - warningItem.lastNotifyTime > warningItem.minTimeToNotify)
			{
				notifier.Add(warningItem.notification);
				warningItem.lastNotifyTime = Time.time;
			}
		}
		if (GameClock.Instance.GetCycle() > 0 && !tutorialMessagesRemaining.Contains(TutorialMessages.TM_Priorities) && !queuedPrioritiesMessage)
		{
			queuedPrioritiesMessage = true;
			GameScheduler.Instance.Schedule("PrioritiesTutorial", 2f, delegate
			{
				Instance.TutorialMessage(TutorialMessages.TM_Priorities);
			});
		}
	}

	private bool OxygenGeneratorBuilt()
	{
		return oxygenGenerators.Count > 0;
	}

	private bool OxygenGeneratorNotBuilt()
	{
		return oxygenGenerators.Count == 0;
	}

	private bool SufficientOxygenLastCycleAndThisCycle()
	{
		if (ReportManager.Instance.YesterdaysReport == null)
		{
			return true;
		}
		ReportManager.ReportEntry entry = ReportManager.Instance.YesterdaysReport.GetEntry(ReportManager.ReportType.OxygenCreated);
		ReportManager.ReportEntry entry2 = ReportManager.Instance.TodaysReport.GetEntry(ReportManager.ReportType.OxygenCreated);
		return entry2.Net > 0.0001f || entry.Net > 0.0001f || (GameClock.Instance.GetCycle() < 1 && !GameClock.Instance.IsNighttime());
	}

	private bool FoodIsRefrigerated()
	{
		if (GetUnrefrigeratedFood(null) > 0)
		{
			return false;
		}
		return true;
	}

	private int GetUnrefrigeratedFood(List<Pickupable> foods)
	{
		int num = 0;
		if (ClusterManager.Instance.activeWorld.worldInventory != null)
		{
			ICollection<Pickupable> pickupables = ClusterManager.Instance.activeWorld.worldInventory.GetPickupables(GameTags.Edible);
			if (pickupables == null)
			{
				return 0;
			}
			foreach (Pickupable item in pickupables)
			{
				if (item.storage != null && (item.storage.GetComponent<RationBox>() != null || item.storage.GetComponent<Refrigerator>() != null))
				{
					Rottable.Instance sMI = item.GetSMI<Rottable.Instance>();
					if (sMI != null && Rottable.RefrigerationLevel(sMI) == Rottable.RotRefrigerationLevel.Normal && Rottable.AtmosphereQuality(sMI) != Rottable.RotAtmosphereQuality.Sterilizing && sMI != null && sMI.RotConstitutionPercentage < 0.8f)
					{
						num++;
						foods?.Add(item);
					}
				}
			}
		}
		return num;
	}

	private bool EnergySourceExists()
	{
		return Game.Instance.circuitManager.HasGenerators();
	}

	private bool BedExists()
	{
		return Components.Sleepables.Count > 0;
	}

	private bool EnoughFood()
	{
		int count = Components.LiveMinionIdentities.GetWorldItems(ClusterManager.Instance.activeWorldId).Count;
		float num = RationTracker.Get().CountRations(null, ClusterManager.Instance.activeWorld.worldInventory);
		float num2 = (float)count * 1000000f;
		return num / num2 >= 1f;
	}

	private bool CanTreatSickDuplicant()
	{
		bool result = Components.Clinics.Count >= 1;
		bool flag = false;
		for (int i = 0; i < Components.LiveMinionIdentities.Count; i++)
		{
			Sicknesses sicknesses = Components.LiveMinionIdentities[i].GetSicknesses();
			foreach (SicknessInstance item in sicknesses)
			{
				if (item.Sickness.severity >= Sickness.Severity.Major)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				break;
			}
		}
		if (!flag)
		{
			return true;
		}
		return result;
	}

	private bool LongTravelTimes()
	{
		if (ReportManager.Instance.reports.Count < 3)
		{
			return true;
		}
		float num = 0f;
		float num2 = 0f;
		for (int num3 = ReportManager.Instance.reports.Count - 1; num3 >= ReportManager.Instance.reports.Count - 3; num3--)
		{
			ReportManager.ReportEntry entry = ReportManager.Instance.reports[num3].GetEntry(ReportManager.ReportType.TravelTime);
			num += entry.Net;
			num2 += 600f * (float)entry.contextEntries.Count;
		}
		float num4 = num / num2;
		return num4 <= 0.4f;
	}

	private bool FoodSourceExists()
	{
		foreach (ComplexFabricator item in Components.ComplexFabricators.Items)
		{
			if (item.GetType() == typeof(MicrobeMusher))
			{
				return true;
			}
		}
		return Components.PlantablePlots.Count > 0;
	}

	private bool HygeneExists()
	{
		return Components.HandSanitizers.Count > 0;
	}

	private bool ToiletExists()
	{
		return Components.Toilets.Count > 0;
	}

	private void ZoomToNextOxygenGenerator()
	{
		if (oxygenGenerators.Count != 0)
		{
			focusedOxygenGenerator %= oxygenGenerators.Count;
			GameObject gameObject = oxygenGenerators[focusedOxygenGenerator];
			if (gameObject != null)
			{
				Vector3 position = gameObject.transform.GetPosition();
				CameraController.Instance.SetTargetPos(position, 8f, playSound: true);
			}
			else
			{
				DebugUtil.DevLogErrorFormat("ZoomToNextOxygenGenerator generator was null: {0}", gameObject);
			}
			focusedOxygenGenerator++;
		}
	}

	private void ZoomToNextUnrefrigeratedFood()
	{
		ListPool<Pickupable, Tutorial>.PooledList pooledList = ListPool<Pickupable, Tutorial>.Allocate();
		int unrefrigeratedFood = GetUnrefrigeratedFood(pooledList);
		if (pooledList.Count != 0)
		{
			focusedUnrefrigFood++;
			if (focusedUnrefrigFood >= unrefrigeratedFood)
			{
				focusedUnrefrigFood = 0;
			}
			Pickupable pickupable = pooledList[focusedUnrefrigFood];
			if (pickupable != null)
			{
				CameraController.Instance.SetTargetPos(pickupable.transform.GetPosition(), 8f, playSound: true);
			}
			pooledList.Recycle();
		}
	}
}
