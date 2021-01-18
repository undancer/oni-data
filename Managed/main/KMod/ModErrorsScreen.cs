using System.Collections.Generic;
using UnityEngine;

namespace KMod
{
	public class ModErrorsScreen : KScreen
	{
		[SerializeField]
		private KButton closeButtonTitle;

		[SerializeField]
		private KButton closeButton;

		[SerializeField]
		private GameObject entryPrefab;

		[SerializeField]
		private Transform entryParent;

		public static bool ShowErrors(List<Event> events)
		{
			if (Global.Instance.modManager.events.Count == 0)
			{
				return false;
			}
			GameObject parent = GameObject.Find("Canvas");
			ModErrorsScreen modErrorsScreen = Util.KInstantiateUI<ModErrorsScreen>(Global.Instance.modErrorsPrefab, parent);
			modErrorsScreen.Initialize(events);
			modErrorsScreen.gameObject.SetActive(value: true);
			return true;
		}

		private void Initialize(List<Event> events)
		{
			foreach (Event @event in events)
			{
				HierarchyReferences hierarchyReferences = Util.KInstantiateUI<HierarchyReferences>(entryPrefab, entryParent.gameObject, force_active: true);
				LocText reference = hierarchyReferences.GetReference<LocText>("Title");
				LocText reference2 = hierarchyReferences.GetReference<LocText>("Description");
				KButton reference3 = hierarchyReferences.GetReference<KButton>("Details");
				Event.GetUIStrings(@event.event_type, out var title, out var title_tooltip);
				reference.text = title;
				reference.GetComponent<ToolTip>().toolTip = title_tooltip;
				reference2.text = @event.mod.title;
				ToolTip component = reference2.GetComponent<ToolTip>();
				if (component != null)
				{
					Label mod = @event.mod;
					component.toolTip = mod.ToString();
				}
				reference3.isInteractable = false;
				Mod mod2 = Global.Instance.modManager.FindMod(@event.mod);
				if (mod2 != null)
				{
					if (component != null && !string.IsNullOrEmpty(mod2.description))
					{
						component.toolTip = mod2.description;
					}
					if (mod2.on_managed != null)
					{
						reference3.onClick += mod2.on_managed;
						reference3.isInteractable = true;
					}
				}
			}
		}

		protected override void OnActivate()
		{
			base.OnActivate();
			closeButtonTitle.onClick += Deactivate;
			closeButton.onClick += Deactivate;
		}
	}
}
