using System.Collections.Generic;
using System.Collections.ObjectModel;
using STRINGS;
using TMPro;
using UnityEngine;

public class EnergyInfoScreen : TargetScreen
{
	public GameObject labelTemplate;

	private GameObject overviewPanel;

	private GameObject generatorsPanel;

	private GameObject consumersPanel;

	private GameObject batteriesPanel;

	private Dictionary<string, GameObject> overviewLabels = new Dictionary<string, GameObject>();

	private Dictionary<string, GameObject> generatorsLabels = new Dictionary<string, GameObject>();

	private Dictionary<string, GameObject> consumersLabels = new Dictionary<string, GameObject>();

	private Dictionary<string, GameObject> batteriesLabels = new Dictionary<string, GameObject>();

	public override bool IsValidForTarget(GameObject target)
	{
		if (!(target.GetComponent<Generator>() != null) && !(target.GetComponent<Wire>() != null) && !(target.GetComponent<Battery>() != null))
		{
			return target.GetComponent<EnergyConsumer>() != null;
		}
		return true;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		overviewPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject);
		overviewPanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = UI.DETAILTABS.ENERGYGENERATOR.CIRCUITOVERVIEW;
		generatorsPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject);
		generatorsPanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = UI.DETAILTABS.ENERGYGENERATOR.GENERATORS;
		consumersPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject);
		consumersPanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = UI.DETAILTABS.ENERGYGENERATOR.CONSUMERS;
		batteriesPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject);
		batteriesPanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = UI.DETAILTABS.ENERGYGENERATOR.BATTERIES;
	}

	private GameObject AddOrGetLabel(Dictionary<string, GameObject> labels, GameObject panel, string id)
	{
		GameObject gameObject = null;
		if (labels.ContainsKey(id))
		{
			gameObject = labels[id];
		}
		else
		{
			gameObject = Util.KInstantiate(labelTemplate, panel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject);
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			labels[id] = gameObject;
			gameObject.SetActive(value: true);
		}
		return gameObject;
	}

	private void LateUpdate()
	{
		Refresh();
	}

	private void Refresh()
	{
		if (selectedTarget == null)
		{
			return;
		}
		foreach (KeyValuePair<string, GameObject> overviewLabel in overviewLabels)
		{
			overviewLabel.Value.SetActive(value: false);
		}
		foreach (KeyValuePair<string, GameObject> generatorsLabel in generatorsLabels)
		{
			generatorsLabel.Value.SetActive(value: false);
		}
		foreach (KeyValuePair<string, GameObject> consumersLabel in consumersLabels)
		{
			consumersLabel.Value.SetActive(value: false);
		}
		foreach (KeyValuePair<string, GameObject> batteriesLabel in batteriesLabels)
		{
			batteriesLabel.Value.SetActive(value: false);
		}
		CircuitManager circuitManager = Game.Instance.circuitManager;
		ushort num = ushort.MaxValue;
		EnergyConsumer component = selectedTarget.GetComponent<EnergyConsumer>();
		if (component != null)
		{
			num = component.CircuitID;
		}
		else
		{
			Generator component2 = selectedTarget.GetComponent<Generator>();
			if (component2 != null)
			{
				num = component2.CircuitID;
			}
		}
		if (num == ushort.MaxValue)
		{
			int cell = Grid.PosToCell(selectedTarget.transform.GetPosition());
			num = circuitManager.GetCircuitID(cell);
		}
		if (num != ushort.MaxValue)
		{
			overviewPanel.SetActive(value: true);
			generatorsPanel.SetActive(value: true);
			consumersPanel.SetActive(value: true);
			batteriesPanel.SetActive(value: true);
			float joulesAvailableOnCircuit = circuitManager.GetJoulesAvailableOnCircuit(num);
			GameObject gameObject = AddOrGetLabel(overviewLabels, overviewPanel, "joulesAvailable");
			gameObject.GetComponent<LocText>().text = string.Format(UI.DETAILTABS.ENERGYGENERATOR.AVAILABLE_JOULES, GameUtil.GetFormattedJoules(joulesAvailableOnCircuit));
			gameObject.GetComponent<ToolTip>().toolTip = UI.DETAILTABS.ENERGYGENERATOR.AVAILABLE_JOULES_TOOLTIP;
			gameObject.SetActive(value: true);
			float wattsGeneratedByCircuit = circuitManager.GetWattsGeneratedByCircuit(num);
			float potentialWattsGeneratedByCircuit = circuitManager.GetPotentialWattsGeneratedByCircuit(num);
			gameObject = AddOrGetLabel(overviewLabels, overviewPanel, "wattageGenerated");
			string text = null;
			text = ((wattsGeneratedByCircuit != potentialWattsGeneratedByCircuit) ? $"{GameUtil.GetFormattedWattage(wattsGeneratedByCircuit)} / {GameUtil.GetFormattedWattage(potentialWattsGeneratedByCircuit)}" : GameUtil.GetFormattedWattage(wattsGeneratedByCircuit));
			gameObject.GetComponent<LocText>().text = string.Format(UI.DETAILTABS.ENERGYGENERATOR.WATTAGE_GENERATED, text);
			gameObject.GetComponent<ToolTip>().toolTip = UI.DETAILTABS.ENERGYGENERATOR.WATTAGE_GENERATED_TOOLTIP;
			gameObject.SetActive(value: true);
			gameObject = AddOrGetLabel(overviewLabels, overviewPanel, "wattageConsumed");
			gameObject.GetComponent<LocText>().text = string.Format(UI.DETAILTABS.ENERGYGENERATOR.WATTAGE_CONSUMED, GameUtil.GetFormattedWattage(circuitManager.GetWattsUsedByCircuit(num)));
			gameObject.GetComponent<ToolTip>().toolTip = UI.DETAILTABS.ENERGYGENERATOR.WATTAGE_CONSUMED_TOOLTIP;
			gameObject.SetActive(value: true);
			gameObject = AddOrGetLabel(overviewLabels, overviewPanel, "potentialWattageConsumed");
			gameObject.GetComponent<LocText>().text = string.Format(UI.DETAILTABS.ENERGYGENERATOR.POTENTIAL_WATTAGE_CONSUMED, GameUtil.GetFormattedWattage(circuitManager.GetWattsNeededWhenActive(num)));
			gameObject.GetComponent<ToolTip>().toolTip = UI.DETAILTABS.ENERGYGENERATOR.POTENTIAL_WATTAGE_CONSUMED_TOOLTIP;
			gameObject.SetActive(value: true);
			gameObject = AddOrGetLabel(overviewLabels, overviewPanel, "maxSafeWattage");
			gameObject.GetComponent<LocText>().text = string.Format(UI.DETAILTABS.ENERGYGENERATOR.MAX_SAFE_WATTAGE, GameUtil.GetFormattedWattage(circuitManager.GetMaxSafeWattageForCircuit(num)));
			gameObject.GetComponent<ToolTip>().toolTip = UI.DETAILTABS.ENERGYGENERATOR.MAX_SAFE_WATTAGE_TOOLTIP;
			gameObject.SetActive(value: true);
			ReadOnlyCollection<Generator> generatorsOnCircuit = circuitManager.GetGeneratorsOnCircuit(num);
			ReadOnlyCollection<IEnergyConsumer> consumersOnCircuit = circuitManager.GetConsumersOnCircuit(num);
			List<Battery> batteriesOnCircuit = circuitManager.GetBatteriesOnCircuit(num);
			ReadOnlyCollection<Battery> transformersOnCircuit = circuitManager.GetTransformersOnCircuit(num);
			if (generatorsOnCircuit.Count > 0)
			{
				foreach (Generator item in generatorsOnCircuit)
				{
					if (item != null && item.GetComponent<Battery>() == null)
					{
						gameObject = AddOrGetLabel(generatorsLabels, generatorsPanel, item.gameObject.GetInstanceID().ToString());
						if (item.GetComponent<Operational>().IsActive)
						{
							gameObject.GetComponent<LocText>().text = $"{item.GetComponent<KSelectable>().entityName}: {GameUtil.GetFormattedWattage(item.WattageRating)}";
						}
						else
						{
							gameObject.GetComponent<LocText>().text = $"{item.GetComponent<KSelectable>().entityName}: {GameUtil.GetFormattedWattage(0f)} / {GameUtil.GetFormattedWattage(item.WattageRating)}";
						}
						gameObject.SetActive(value: true);
						gameObject.GetComponent<LocText>().fontStyle = ((item.gameObject == selectedTarget) ? FontStyles.Bold : FontStyles.Normal);
					}
				}
			}
			else
			{
				gameObject = AddOrGetLabel(generatorsLabels, generatorsPanel, "nogenerators");
				gameObject.GetComponent<LocText>().text = UI.DETAILTABS.ENERGYGENERATOR.NOGENERATORS;
				gameObject.SetActive(value: true);
			}
			if (consumersOnCircuit.Count > 0 || transformersOnCircuit.Count > 0)
			{
				foreach (IEnergyConsumer item2 in consumersOnCircuit)
				{
					AddConsumerInfo(item2, gameObject);
				}
				foreach (Battery item3 in transformersOnCircuit)
				{
					AddConsumerInfo(item3, gameObject);
				}
			}
			else
			{
				gameObject = AddOrGetLabel(consumersLabels, consumersPanel, "noconsumers");
				gameObject.GetComponent<LocText>().text = UI.DETAILTABS.ENERGYGENERATOR.NOCONSUMERS;
				gameObject.SetActive(value: true);
			}
			if (batteriesOnCircuit.Count > 0)
			{
				foreach (Battery item4 in batteriesOnCircuit)
				{
					if (item4 != null)
					{
						gameObject = AddOrGetLabel(batteriesLabels, batteriesPanel, item4.gameObject.GetInstanceID().ToString());
						gameObject.GetComponent<LocText>().text = $"{item4.GetComponent<KSelectable>().entityName}: {GameUtil.GetFormattedJoules(item4.JoulesAvailable)}";
						gameObject.SetActive(value: true);
						gameObject.GetComponent<LocText>().fontStyle = ((item4.gameObject == selectedTarget) ? FontStyles.Bold : FontStyles.Normal);
					}
				}
			}
			else
			{
				gameObject = AddOrGetLabel(batteriesLabels, batteriesPanel, "nobatteries");
				gameObject.GetComponent<LocText>().text = UI.DETAILTABS.ENERGYGENERATOR.NOBATTERIES;
				gameObject.SetActive(value: true);
			}
		}
		else
		{
			overviewPanel.SetActive(value: true);
			generatorsPanel.SetActive(value: false);
			consumersPanel.SetActive(value: false);
			batteriesPanel.SetActive(value: false);
			GameObject gameObject2 = AddOrGetLabel(overviewLabels, overviewPanel, "nocircuit");
			gameObject2.GetComponent<LocText>().text = UI.DETAILTABS.ENERGYGENERATOR.DISCONNECTED;
			gameObject2.SetActive(value: true);
		}
	}

	private void AddConsumerInfo(IEnergyConsumer consumer, GameObject label)
	{
		KMonoBehaviour kMonoBehaviour = consumer as KMonoBehaviour;
		if (kMonoBehaviour != null)
		{
			label = AddOrGetLabel(consumersLabels, consumersPanel, kMonoBehaviour.gameObject.GetInstanceID().ToString());
			float wattsUsed = consumer.WattsUsed;
			float wattsNeededWhenActive = consumer.WattsNeededWhenActive;
			string text = null;
			text = ((wattsUsed != wattsNeededWhenActive) ? $"{GameUtil.GetFormattedWattage(wattsUsed)} / {GameUtil.GetFormattedWattage(wattsNeededWhenActive)}" : GameUtil.GetFormattedWattage(wattsUsed));
			label.GetComponent<LocText>().text = $"{consumer.Name}: {text}";
			label.SetActive(value: true);
			label.GetComponent<LocText>().fontStyle = ((kMonoBehaviour.gameObject == selectedTarget) ? FontStyles.Bold : FontStyles.Normal);
		}
	}
}
