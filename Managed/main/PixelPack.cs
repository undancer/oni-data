using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/PixelPack")]
public class PixelPack : KMonoBehaviour, ISaveLoadable
{
	public struct ColorPair
	{
		public Color activeColor;

		public Color standbyColor;
	}

	protected KBatchedAnimController animController;

	private static readonly EventSystem.IntraObjectHandler<PixelPack> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<PixelPack>(delegate(PixelPack component, object data)
	{
		component.OnLogicValueChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<PixelPack> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<PixelPack>(delegate(PixelPack component, object data)
	{
		component.OnOperationalChanged(data);
	});

	public static readonly HashedString PORT_ID = new HashedString("PixelPackInput");

	public static readonly HashedString SYMBOL_ONE_NAME = "screen1";

	public static readonly HashedString SYMBOL_TWO_NAME = "screen2";

	public static readonly HashedString SYMBOL_THREE_NAME = "screen3";

	public static readonly HashedString SYMBOL_FOUR_NAME = "screen4";

	[MyCmpGet]
	private Operational operational;

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	private static readonly EventSystem.IntraObjectHandler<PixelPack> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<PixelPack>(delegate(PixelPack component, object data)
	{
		component.OnCopySettings(data);
	});

	public int logicValue;

	[Serialize]
	public List<ColorPair> colorSettings;

	private Color defaultActive = new Color(88f / 255f, 72f / 85f, 28f / 85f);

	private Color defaultStandby = new Color(248f / 255f, 0.47058824f, 88f / 255f);

	protected static readonly HashedString[] ON_ANIMS = new HashedString[2] { "on_pre", "on" };

	protected static readonly HashedString[] OFF_ANIMS = new HashedString[2] { "off_pre", "off" };

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-905833192, OnCopySettingsDelegate);
	}

	private void OnCopySettings(object data)
	{
		PixelPack component = ((GameObject)data).GetComponent<PixelPack>();
		if (component != null)
		{
			for (int i = 0; i < component.colorSettings.Count; i++)
			{
				colorSettings[i] = component.colorSettings[i];
			}
		}
		UpdateColors();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		animController = GetComponent<KBatchedAnimController>();
		Subscribe(-801688580, OnLogicValueChangedDelegate);
		Subscribe(-592767678, OnOperationalChangedDelegate);
		if (colorSettings == null)
		{
			ColorPair colorPair = default(ColorPair);
			colorPair.activeColor = defaultActive;
			colorPair.standbyColor = defaultStandby;
			ColorPair item = colorPair;
			colorPair = default(ColorPair);
			colorPair.activeColor = defaultActive;
			colorPair.standbyColor = defaultStandby;
			ColorPair item2 = colorPair;
			colorPair = default(ColorPair);
			colorPair.activeColor = defaultActive;
			colorPair.standbyColor = defaultStandby;
			ColorPair item3 = colorPair;
			colorPair = default(ColorPair);
			colorPair.activeColor = defaultActive;
			colorPair.standbyColor = defaultStandby;
			ColorPair item4 = colorPair;
			colorSettings = new List<ColorPair>();
			colorSettings.Add(item);
			colorSettings.Add(item2);
			colorSettings.Add(item3);
			colorSettings.Add(item4);
		}
	}

	private void OnLogicValueChanged(object data)
	{
		LogicValueChanged logicValueChanged = (LogicValueChanged)data;
		if (logicValueChanged.portID == PORT_ID)
		{
			logicValue = logicValueChanged.newValue;
			UpdateColors();
		}
	}

	private void OnOperationalChanged(object data)
	{
		if (operational.IsOperational)
		{
			UpdateColors();
			animController.Play(ON_ANIMS);
		}
		else
		{
			animController.Play(OFF_ANIMS);
		}
		operational.SetActive(operational.IsOperational);
	}

	public void UpdateColors()
	{
		if (!operational.IsOperational)
		{
			return;
		}
		LogicPorts component = GetComponent<LogicPorts>();
		if (component != null)
		{
			switch (component.GetConnectedWireBitDepth(PORT_ID))
			{
			case LogicWire.BitDepth.FourBit:
				animController.SetSymbolTint(SYMBOL_ONE_NAME, LogicCircuitNetwork.IsBitActive(0, logicValue) ? colorSettings[0].activeColor : colorSettings[0].standbyColor);
				animController.SetSymbolTint(SYMBOL_TWO_NAME, LogicCircuitNetwork.IsBitActive(1, logicValue) ? colorSettings[1].activeColor : colorSettings[1].standbyColor);
				animController.SetSymbolTint(SYMBOL_THREE_NAME, LogicCircuitNetwork.IsBitActive(2, logicValue) ? colorSettings[2].activeColor : colorSettings[2].standbyColor);
				animController.SetSymbolTint(SYMBOL_FOUR_NAME, LogicCircuitNetwork.IsBitActive(3, logicValue) ? colorSettings[3].activeColor : colorSettings[3].standbyColor);
				break;
			case LogicWire.BitDepth.OneBit:
				animController.SetSymbolTint(SYMBOL_ONE_NAME, LogicCircuitNetwork.IsBitActive(0, logicValue) ? colorSettings[0].activeColor : colorSettings[0].standbyColor);
				animController.SetSymbolTint(SYMBOL_TWO_NAME, LogicCircuitNetwork.IsBitActive(0, logicValue) ? colorSettings[1].activeColor : colorSettings[1].standbyColor);
				animController.SetSymbolTint(SYMBOL_THREE_NAME, LogicCircuitNetwork.IsBitActive(0, logicValue) ? colorSettings[2].activeColor : colorSettings[2].standbyColor);
				animController.SetSymbolTint(SYMBOL_FOUR_NAME, LogicCircuitNetwork.IsBitActive(0, logicValue) ? colorSettings[3].activeColor : colorSettings[3].standbyColor);
				break;
			}
		}
	}
}
