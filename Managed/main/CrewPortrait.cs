using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
[AddComponentMenu("KMonoBehaviour/scripts/CrewPortrait")]
public class CrewPortrait : KMonoBehaviour
{
	public Image targetImage;

	public bool startTransparent;

	public bool useLabels = true;

	[SerializeField]
	public KBatchedAnimController controller;

	public float animScaleBase = 0.2f;

	public LocText duplicantName;

	public LocText duplicantJob;

	public LocText subTitle;

	public bool useDefaultExpression = true;

	private bool requiresRefresh;

	private bool areEventsRegistered;

	public IAssignableIdentity identityObject
	{
		get;
		private set;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (startTransparent)
		{
			StartCoroutine(AlphaIn());
		}
		requiresRefresh = true;
	}

	private IEnumerator AlphaIn()
	{
		SetAlpha(0f);
		for (float i = 0f; i < 1f; i += Time.unscaledDeltaTime * 4f)
		{
			SetAlpha(i);
			yield return 0;
		}
		SetAlpha(1f);
	}

	private void OnRoleChanged(object data)
	{
		if (!(controller == null))
		{
			RefreshHat(identityObject, controller);
		}
	}

	private void RegisterEvents()
	{
		if (!areEventsRegistered)
		{
			KMonoBehaviour kMonoBehaviour = identityObject as KMonoBehaviour;
			if (!(kMonoBehaviour == null))
			{
				kMonoBehaviour.Subscribe(540773776, OnRoleChanged);
				areEventsRegistered = true;
			}
		}
	}

	private void UnregisterEvents()
	{
		if (areEventsRegistered)
		{
			areEventsRegistered = false;
			KMonoBehaviour kMonoBehaviour = identityObject as KMonoBehaviour;
			if (!(kMonoBehaviour == null))
			{
				kMonoBehaviour.Unsubscribe(540773776, OnRoleChanged);
			}
		}
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		RegisterEvents();
		ForceRefresh();
	}

	protected override void OnCmpDisable()
	{
		base.OnCmpDisable();
		UnregisterEvents();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		UnregisterEvents();
	}

	public void SetIdentityObject(IAssignableIdentity identity, bool jobEnabled = true)
	{
		UnregisterEvents();
		identityObject = identity;
		RegisterEvents();
		targetImage.enabled = true;
		if (identityObject != null)
		{
			targetImage.enabled = false;
		}
		if (useLabels && (identity is MinionIdentity || identity is MinionAssignablesProxy))
		{
			SetDuplicantJobTitleActive(jobEnabled);
		}
		requiresRefresh = true;
	}

	public void SetSubTitle(string newTitle)
	{
		if (subTitle != null)
		{
			if (string.IsNullOrEmpty(newTitle))
			{
				subTitle.gameObject.SetActive(value: false);
				return;
			}
			subTitle.gameObject.SetActive(value: true);
			subTitle.SetText(newTitle);
		}
	}

	public void SetDuplicantJobTitleActive(bool state)
	{
		if (duplicantJob != null && duplicantJob.gameObject.activeInHierarchy != state)
		{
			duplicantJob.gameObject.SetActive(state);
		}
	}

	public void ForceRefresh()
	{
		requiresRefresh = true;
	}

	public void Update()
	{
		if (requiresRefresh && (controller == null || controller.enabled))
		{
			requiresRefresh = false;
			Rebuild();
		}
	}

	private void Rebuild()
	{
		if (controller == null)
		{
			controller = GetComponentInChildren<KBatchedAnimController>();
			if (controller == null)
			{
				if (targetImage != null)
				{
					targetImage.enabled = true;
				}
				Debug.LogWarning("Controller for [" + base.name + "] null");
				return;
			}
		}
		SetPortraitData(identityObject, controller, useDefaultExpression);
		if (useLabels && duplicantName != null)
		{
			duplicantName.SetText((identityObject != null) ? identityObject.GetProperName() : "");
			if (identityObject is MinionIdentity && duplicantJob != null)
			{
				duplicantJob.SetText((identityObject != null) ? (identityObject as MinionIdentity).GetComponent<MinionResume>().GetSkillsSubtitle() : "");
				duplicantJob.GetComponent<ToolTip>().toolTip = (identityObject as MinionIdentity).GetComponent<MinionResume>().GetSkillsSubtitle();
			}
		}
	}

	private static void RefreshHat(IAssignableIdentity identityObject, KBatchedAnimController controller)
	{
		string hat_id = "";
		MinionIdentity minionIdentity = identityObject as MinionIdentity;
		if (minionIdentity != null)
		{
			hat_id = minionIdentity.GetComponent<MinionResume>().CurrentHat;
		}
		else if (identityObject as StoredMinionIdentity != null)
		{
			hat_id = (identityObject as StoredMinionIdentity).currentHat;
		}
		MinionResume.ApplyHat(hat_id, controller);
	}

	public static void SetPortraitData(IAssignableIdentity identityObject, KBatchedAnimController controller, bool useDefaultExpression = true)
	{
		if (identityObject == null)
		{
			controller.gameObject.SetActive(value: false);
			return;
		}
		MinionIdentity minionIdentity = identityObject as MinionIdentity;
		if (minionIdentity == null)
		{
			MinionAssignablesProxy minionAssignablesProxy = identityObject as MinionAssignablesProxy;
			if (minionAssignablesProxy != null && minionAssignablesProxy.target != null)
			{
				minionIdentity = minionAssignablesProxy.target as MinionIdentity;
			}
		}
		controller.gameObject.SetActive(value: true);
		controller.Play("ui_idle");
		SymbolOverrideController component = controller.GetComponent<SymbolOverrideController>();
		component.RemoveAllSymbolOverrides();
		if (minionIdentity != null)
		{
			Accessorizer component2 = minionIdentity.GetComponent<Accessorizer>();
			foreach (AccessorySlot resource in Db.Get().AccessorySlots.resources)
			{
				Accessory accessory = component2.GetAccessory(resource);
				if (accessory != null)
				{
					component.AddSymbolOverride(resource.targetSymbolId, accessory.symbol);
					controller.SetSymbolVisiblity(resource.targetSymbolId, is_visible: true);
				}
				else
				{
					controller.SetSymbolVisiblity(resource.targetSymbolId, is_visible: false);
				}
			}
			component.AddSymbolOverride(Db.Get().AccessorySlots.HatHair.targetSymbolId, Db.Get().AccessorySlots.HatHair.Lookup("hat_" + HashCache.Get().Get(component2.GetAccessory(Db.Get().AccessorySlots.Hair).symbol.hash)).symbol, 1);
			RefreshHat(minionIdentity, controller);
		}
		else
		{
			StoredMinionIdentity storedMinionIdentity = identityObject as StoredMinionIdentity;
			if (storedMinionIdentity == null)
			{
				MinionAssignablesProxy minionAssignablesProxy2 = identityObject as MinionAssignablesProxy;
				if (minionAssignablesProxy2 != null && minionAssignablesProxy2.target != null)
				{
					storedMinionIdentity = minionAssignablesProxy2.target as StoredMinionIdentity;
				}
			}
			if (!(storedMinionIdentity != null))
			{
				controller.gameObject.SetActive(value: false);
				return;
			}
			foreach (AccessorySlot resource2 in Db.Get().AccessorySlots.resources)
			{
				Accessory accessory2 = storedMinionIdentity.GetAccessory(resource2);
				if (accessory2 != null)
				{
					component.AddSymbolOverride(resource2.targetSymbolId, accessory2.symbol);
					controller.SetSymbolVisiblity(resource2.targetSymbolId, is_visible: true);
				}
				else
				{
					controller.SetSymbolVisiblity(resource2.targetSymbolId, is_visible: false);
				}
			}
			component.AddSymbolOverride(Db.Get().AccessorySlots.HatHair.targetSymbolId, Db.Get().AccessorySlots.HatHair.Lookup("hat_" + HashCache.Get().Get(storedMinionIdentity.GetAccessory(Db.Get().AccessorySlots.Hair).symbol.hash)).symbol, 1);
			RefreshHat(storedMinionIdentity, controller);
		}
		float num = (controller.animScale = 0.25f);
		string s = "ui_idle";
		controller.Play(s, KAnim.PlayMode.Loop);
		controller.SetSymbolVisiblity("snapTo_neck", is_visible: false);
		controller.SetSymbolVisiblity("snapTo_goggles", is_visible: false);
	}

	public void SetAlpha(float value)
	{
		if (!(controller == null) && (float)(int)controller.TintColour.a != value)
		{
			controller.TintColour = new Color(1f, 1f, 1f, value);
		}
	}
}
