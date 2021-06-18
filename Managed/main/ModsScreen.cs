using System;
using System.Collections.Generic;
using System.Linq;
using KMod;
using STRINGS;
using UnityEngine;

public class ModsScreen : KModalScreen
{
	private struct DisplayedMod
	{
		public RectTransform rect_transform;

		public int mod_index;
	}

	private class ModOrderingDragListener : DragMe.IDragListener
	{
		private List<DisplayedMod> mods;

		private ModsScreen screen;

		private int startDragIdx = -1;

		public ModOrderingDragListener(ModsScreen screen, List<DisplayedMod> mods)
		{
			this.screen = screen;
			this.mods = mods;
		}

		public void OnBeginDrag(Vector2 pos)
		{
			startDragIdx = GetDragIdx(pos, halfPosition: false);
		}

		public void OnEndDrag(Vector2 pos)
		{
			if (startDragIdx >= 0)
			{
				int dragIdx = GetDragIdx(pos, halfPosition: true);
				if (dragIdx != startDragIdx)
				{
					int mod_index = mods[startDragIdx].mod_index;
					int target_index = ((0 <= dragIdx && dragIdx < mods.Count) ? mods[dragIdx].mod_index : (-1));
					Global.Instance.modManager.Reinsert(mod_index, target_index, dragIdx >= mods.Count, this);
					screen.BuildDisplay();
				}
			}
		}

		private int GetDragIdx(Vector2 pos, bool halfPosition)
		{
			int result = -1;
			for (int i = 0; i < mods.Count; i++)
			{
				RectTransformUtility.ScreenPointToLocalPointInRectangle(mods[i].rect_transform, pos, null, out var localPoint);
				if (!halfPosition)
				{
					localPoint += mods[i].rect_transform.rect.min;
				}
				if (localPoint.y < 0f)
				{
					result = i;
					continue;
				}
				break;
			}
			return result;
		}
	}

	[SerializeField]
	private KButton closeButtonTitle;

	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private KButton toggleAllButton;

	[SerializeField]
	private KButton workshopButton;

	[SerializeField]
	private GameObject entryPrefab;

	[SerializeField]
	private Transform entryParent;

	private List<DisplayedMod> displayedMods = new List<DisplayedMod>();

	private List<Label> mod_footprint = new List<Label>();

	protected override void OnActivate()
	{
		base.OnActivate();
		closeButtonTitle.onClick += Exit;
		closeButton.onClick += Exit;
		System.Action value = delegate
		{
			Application.OpenURL("http://steamcommunity.com/workshop/browse/?appid=457140");
		};
		workshopButton.onClick += value;
		UpdateToggleAllButton();
		toggleAllButton.onClick += OnToggleAllClicked;
		Global.Instance.modManager.Sanitize(base.gameObject);
		mod_footprint.Clear();
		foreach (Mod mod in Global.Instance.modManager.mods)
		{
			if (mod.IsEnabledForActiveDlc())
			{
				mod_footprint.Add(mod.label);
				if ((mod.loaded_content & (Content.Strings | Content.DLL | Content.Translation | Content.Animation)) == (mod.available_content & (Content.Strings | Content.DLL | Content.Translation | Content.Animation)))
				{
					mod.Uncrash();
				}
			}
		}
		BuildDisplay();
		Manager modManager = Global.Instance.modManager;
		modManager.on_update = (Manager.OnUpdate)Delegate.Combine(modManager.on_update, new Manager.OnUpdate(RebuildDisplay));
	}

	protected override void OnDeactivate()
	{
		Manager modManager = Global.Instance.modManager;
		modManager.on_update = (Manager.OnUpdate)Delegate.Remove(modManager.on_update, new Manager.OnUpdate(RebuildDisplay));
		base.OnDeactivate();
	}

	private void Exit()
	{
		Global.Instance.modManager.Save();
		if (!Global.Instance.modManager.MatchFootprint(mod_footprint, Content.Strings | Content.DLL | Content.Translation | Content.Animation))
		{
			Global.Instance.modManager.RestartDialog(UI.FRONTEND.MOD_DIALOGS.MODS_SCREEN_CHANGES.TITLE, UI.FRONTEND.MOD_DIALOGS.MODS_SCREEN_CHANGES.MESSAGE, Deactivate, with_details: true, base.gameObject);
		}
		else
		{
			Deactivate();
		}
		Global.Instance.modManager.events.Clear();
	}

	private void RebuildDisplay(object change_source)
	{
		if (change_source != this)
		{
			BuildDisplay();
		}
	}

	private bool ShouldDisplayMod(Mod mod)
	{
		return mod.status != 0 && mod.status != Mod.Status.UninstallPending && !mod.HasOnlyTranslationContent();
	}

	private void BuildDisplay()
	{
		foreach (DisplayedMod displayedMod in displayedMods)
		{
			if (displayedMod.rect_transform != null)
			{
				UnityEngine.Object.Destroy(displayedMod.rect_transform.gameObject);
			}
		}
		displayedMods.Clear();
		ModOrderingDragListener listener = new ModOrderingDragListener(this, displayedMods);
		for (int i = 0; i != Global.Instance.modManager.mods.Count; i++)
		{
			Mod mod = Global.Instance.modManager.mods[i];
			if (!ShouldDisplayMod(mod))
			{
				continue;
			}
			HierarchyReferences hierarchyReferences = Util.KInstantiateUI<HierarchyReferences>(entryPrefab, entryParent.gameObject);
			displayedMods.Add(new DisplayedMod
			{
				rect_transform = hierarchyReferences.gameObject.GetComponent<RectTransform>(),
				mod_index = i
			});
			DragMe component = hierarchyReferences.GetComponent<DragMe>();
			component.listener = listener;
			LocText reference = hierarchyReferences.GetReference<LocText>("Title");
			string text = mod.title;
			hierarchyReferences.name = mod.title;
			if (mod.available_content == (Content)0)
			{
				text = mod.contentCompatability switch
				{
					ModContentCompatability.OldAPI => text + UI.FRONTEND.MODS.CONTENT_FAILURE.OLD_API, 
					ModContentCompatability.NoContent => text + UI.FRONTEND.MODS.CONTENT_FAILURE.NO_CONTENT, 
					_ => text + UI.FRONTEND.MODS.CONTENT_FAILURE.DISABLED_CONTENT.Replace("{Content}", GetDlcName(DlcManager.GetHighestActiveDlcId())), 
				};
			}
			reference.text = text;
			LocText reference2 = hierarchyReferences.GetReference<LocText>("Version");
			if (mod.packagedModInfo != null && mod.packagedModInfo.version != null && mod.packagedModInfo.version.Length > 0)
			{
				string text2 = mod.packagedModInfo.version;
				if (text2.StartsWith("V"))
				{
					text2 = "v" + text2.Substring(1, text2.Length - 1);
				}
				else if (!text2.StartsWith("v"))
				{
					text2 = "v" + text2;
				}
				reference2.text = text2;
				reference2.gameObject.SetActive(value: true);
			}
			else
			{
				reference2.gameObject.SetActive(value: false);
			}
			ToolTip reference3 = hierarchyReferences.GetReference<ToolTip>("Description");
			reference3.toolTip = mod.description;
			if (mod.crash_count != 0)
			{
				reference.color = Color.Lerp(Color.white, Color.red, (float)mod.crash_count / 3f);
			}
			KButton reference4 = hierarchyReferences.GetReference<KButton>("ManageButton");
			reference4.GetComponentInChildren<LocText>().text = (mod.IsLocal ? UI.FRONTEND.MODS.MANAGE_LOCAL : UI.FRONTEND.MODS.MANAGE);
			reference4.isInteractable = mod.is_managed;
			if (reference4.isInteractable)
			{
				reference4.GetComponent<ToolTip>().toolTip = mod.manage_tooltip;
				reference4.onClick += mod.on_managed;
			}
			KImage reference5 = hierarchyReferences.GetReference<KImage>("BG");
			MultiToggle toggle = hierarchyReferences.GetReference<MultiToggle>("EnabledToggle");
			toggle.ChangeState(mod.IsEnabledForActiveDlc() ? 1 : 0);
			if (mod.available_content != 0)
			{
				reference5.defaultState = KImage.ColorSelector.Inactive;
				reference5.ColorState = KImage.ColorSelector.Inactive;
				MultiToggle multiToggle = toggle;
				multiToggle.onClick = (System.Action)Delegate.Combine(multiToggle.onClick, (System.Action)delegate
				{
					OnToggleClicked(toggle, mod.label);
				});
				toggle.GetComponent<ToolTip>().OnToolTip = () => mod.IsEnabledForActiveDlc() ? UI.FRONTEND.MODS.TOOLTIPS.ENABLED : UI.FRONTEND.MODS.TOOLTIPS.DISABLED;
			}
			else
			{
				reference5.defaultState = KImage.ColorSelector.Disabled;
				reference5.ColorState = KImage.ColorSelector.Disabled;
			}
			hierarchyReferences.gameObject.SetActive(value: true);
		}
		foreach (DisplayedMod displayedMod2 in displayedMods)
		{
			displayedMod2.rect_transform.gameObject.SetActive(value: true);
		}
		if (displayedMods.Count != 0)
		{
		}
	}

	private static string GetDlcName(string dlcId)
	{
		if (dlcId != null)
		{
			if (dlcId == "EXPANSION1_ID")
			{
				return UI.DLC1.NAME_ITAL;
			}
			if (dlcId != null && dlcId.Length == 0)
			{
			}
		}
		return UI.VANILLA.NAME_ITAL;
	}

	private void OnToggleClicked(MultiToggle toggle, Label mod)
	{
		Manager modManager = Global.Instance.modManager;
		bool flag = modManager.IsModEnabled(mod);
		flag = !flag;
		toggle.ChangeState(flag ? 1 : 0);
		modManager.EnableMod(mod, flag, this);
		UpdateToggleAllButton();
	}

	private bool AreAnyModsDisabled()
	{
		return Global.Instance.modManager.mods.Any((Mod mod) => !mod.IsEmpty() && !mod.IsEnabledForActiveDlc() && ShouldDisplayMod(mod));
	}

	private void UpdateToggleAllButton()
	{
		toggleAllButton.GetComponentInChildren<LocText>().text = (AreAnyModsDisabled() ? UI.FRONTEND.MODS.ENABLE_ALL : UI.FRONTEND.MODS.DISABLE_ALL);
	}

	private void OnToggleAllClicked()
	{
		bool enabled = AreAnyModsDisabled();
		Manager modManager = Global.Instance.modManager;
		foreach (Mod mod in modManager.mods)
		{
			if (ShouldDisplayMod(mod))
			{
				modManager.EnableMod(mod.label, enabled, this);
			}
		}
		BuildDisplay();
		UpdateToggleAllButton();
	}
}
