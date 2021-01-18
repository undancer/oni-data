using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[SkipSaveFileSerialization]
public class MoveableLogicGateVisualizer : LogicGateBase
{
	private int cell;

	protected List<GameObject> visChildren = new List<GameObject>();

	private static readonly EventSystem.IntraObjectHandler<MoveableLogicGateVisualizer> OnRotatedDelegate = new EventSystem.IntraObjectHandler<MoveableLogicGateVisualizer>(delegate(MoveableLogicGateVisualizer component, object data)
	{
		component.OnRotated(data);
	});

	protected override void OnSpawn()
	{
		base.OnSpawn();
		cell = -1;
		OverlayScreen instance = OverlayScreen.Instance;
		instance.OnOverlayChanged = (Action<HashedString>)Delegate.Combine(instance.OnOverlayChanged, new Action<HashedString>(OnOverlayChanged));
		OnOverlayChanged(OverlayScreen.Instance.mode);
		Subscribe(-1643076535, OnRotatedDelegate);
	}

	protected override void OnCleanUp()
	{
		OverlayScreen instance = OverlayScreen.Instance;
		instance.OnOverlayChanged = (Action<HashedString>)Delegate.Remove(instance.OnOverlayChanged, new Action<HashedString>(OnOverlayChanged));
		Unregister();
		base.OnCleanUp();
	}

	private void OnOverlayChanged(HashedString mode)
	{
		if (mode == OverlayModes.Logic.ID)
		{
			Register();
		}
		else
		{
			Unregister();
		}
	}

	private void OnRotated(object data)
	{
		Unregister();
		OnOverlayChanged(OverlayScreen.Instance.mode);
	}

	private void Update()
	{
		if (visChildren.Count > 0)
		{
			int num = Grid.PosToCell(base.transform.GetPosition());
			if (num != cell)
			{
				cell = num;
				Unregister();
				Register();
			}
		}
	}

	private GameObject CreateUIElem(int cell, bool is_input)
	{
		GameObject gameObject = Util.KInstantiate(LogicGateBase.uiSrcData.prefab, Grid.CellToPosCCC(cell, Grid.SceneLayer.Front), Quaternion.identity, GameScreenManager.Instance.worldSpaceCanvas);
		Image component = gameObject.GetComponent<Image>();
		component.sprite = (is_input ? LogicGateBase.uiSrcData.inputSprite : LogicGateBase.uiSrcData.outputSprite);
		component.raycastTarget = false;
		return gameObject;
	}

	private void Register()
	{
		if (visChildren.Count <= 0)
		{
			base.enabled = true;
			visChildren.Add(CreateUIElem(base.OutputCellOne, is_input: false));
			if (base.RequiresFourOutputs)
			{
				visChildren.Add(CreateUIElem(base.OutputCellTwo, is_input: false));
				visChildren.Add(CreateUIElem(base.OutputCellThree, is_input: false));
				visChildren.Add(CreateUIElem(base.OutputCellFour, is_input: false));
			}
			visChildren.Add(CreateUIElem(base.InputCellOne, is_input: true));
			if (base.RequiresTwoInputs)
			{
				visChildren.Add(CreateUIElem(base.InputCellTwo, is_input: true));
			}
			else if (base.RequiresFourInputs)
			{
				visChildren.Add(CreateUIElem(base.InputCellTwo, is_input: true));
				visChildren.Add(CreateUIElem(base.InputCellThree, is_input: true));
				visChildren.Add(CreateUIElem(base.InputCellFour, is_input: true));
			}
			if (base.RequiresControlInputs)
			{
				visChildren.Add(CreateUIElem(base.ControlCellOne, is_input: true));
				visChildren.Add(CreateUIElem(base.ControlCellTwo, is_input: true));
			}
		}
	}

	private void Unregister()
	{
		if (visChildren.Count <= 0)
		{
			return;
		}
		base.enabled = false;
		cell = -1;
		foreach (GameObject visChild in visChildren)
		{
			Util.KDestroyGameObject(visChild);
		}
		visChildren.Clear();
	}
}
