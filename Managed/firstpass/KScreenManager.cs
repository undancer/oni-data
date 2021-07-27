using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

[AddComponentMenu("KMonoBehaviour/Plugins/KScreenManager")]
public class KScreenManager : KMonoBehaviour, IInputHandler
{
	private static bool quitting;

	private static bool inputDisabled;

	private List<KScreen> screenStack = new List<KScreen>();

	private UnityEngine.EventSystems.EventSystem evSys;

	private KButtonEvent lastConsumedEvent;

	private KScreen lastConsumedEventScreen;

	public static KScreenManager Instance { get; private set; }

	public string handlerName => base.gameObject.name;

	public KInputHandler inputHandler { get; set; }

	private void OnApplicationQuit()
	{
		quitting = true;
	}

	public void DisableInput(bool disable)
	{
		inputDisabled = disable;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
	}

	protected override void OnCleanUp()
	{
		Instance = null;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		evSys = UnityEngine.EventSystems.EventSystem.current;
	}

	protected override void OnCmpDisable()
	{
		if (quitting)
		{
			for (int num = screenStack.Count - 1; num >= 0; num--)
			{
				screenStack[num].Deactivate();
			}
		}
	}

	public GameObject ActivateScreen(GameObject screen, GameObject parent)
	{
		AddExistingChild(parent, screen);
		screen.GetComponent<KScreen>().Activate();
		return screen;
	}

	public KScreen InstantiateScreen(GameObject screenPrefab, GameObject parent)
	{
		return AddChild(parent, screenPrefab).GetComponent<KScreen>();
	}

	public KScreen StartScreen(GameObject screenPrefab, GameObject parent)
	{
		KScreen component = AddChild(parent, screenPrefab).GetComponent<KScreen>();
		component.Activate();
		return component;
	}

	public void PushScreen(KScreen screen)
	{
		screenStack.Add(screen);
		RefreshStack();
	}

	public void RefreshStack()
	{
		screenStack = (from x in screenStack
			where x != null
			orderby x.GetSortKey()
			select x).ToList();
	}

	public KScreen PopScreen(KScreen screen)
	{
		KScreen result = null;
		int num = screenStack.IndexOf(screen);
		if (num >= 0)
		{
			result = screenStack[num];
			screenStack.RemoveAt(num);
		}
		screenStack = (from x in screenStack
			where x != null
			orderby x.GetSortKey()
			select x).ToList();
		return result;
	}

	public KScreen PopScreen()
	{
		KScreen result = screenStack[screenStack.Count - 1];
		screenStack.RemoveAt(screenStack.Count - 1);
		return result;
	}

	public string DebugScreenStack()
	{
		string text = "";
		foreach (KScreen item in screenStack)
		{
			if (item != null)
			{
				if (!item.isActiveAndEnabled)
				{
					text += "Not isActiveAndEnabled: ";
				}
				text = text + item.name + "\n";
			}
			else
			{
				text += "Null screen in screenStack\n";
			}
		}
		return text;
	}

	private void Update()
	{
		bool flag = true;
		for (int num = screenStack.Count - 1; num >= 0; num--)
		{
			KScreen kScreen = screenStack[num];
			if (kScreen != null && kScreen.isActiveAndEnabled)
			{
				kScreen.ScreenUpdate(flag);
			}
			if (flag && kScreen.IsModal())
			{
				flag = false;
			}
		}
	}

	public void OnKeyDown(KButtonEvent e)
	{
		if (inputDisabled)
		{
			return;
		}
		for (int num = screenStack.Count - 1; num >= 0; num--)
		{
			KScreen kScreen = screenStack[num];
			if (kScreen != null && kScreen.isActiveAndEnabled)
			{
				kScreen.OnKeyDown(e);
				if (e.Consumed)
				{
					lastConsumedEvent = e;
					lastConsumedEventScreen = kScreen;
					break;
				}
			}
		}
	}

	public void OnKeyUp(KButtonEvent e)
	{
		if (inputDisabled)
		{
			return;
		}
		for (int num = screenStack.Count - 1; num >= 0; num--)
		{
			KScreen kScreen = screenStack[num];
			if (kScreen != null && kScreen.isActiveAndEnabled)
			{
				kScreen.OnKeyUp(e);
				if (e.Consumed)
				{
					lastConsumedEvent = e;
					lastConsumedEventScreen = kScreen;
					break;
				}
			}
		}
	}

	public void SetEventSystemEnabled(bool state)
	{
		if (evSys == null)
		{
			evSys = UnityEngine.EventSystems.EventSystem.current;
			if (evSys == null)
			{
				Debug.LogWarning("Cannot enable/disable null UI event system");
				return;
			}
		}
		if (evSys.enabled != state)
		{
			evSys.enabled = state;
		}
	}

	public void SetNavigationEventsEnabled(bool state)
	{
		if (!(evSys == null))
		{
			evSys.sendNavigationEvents = state;
		}
	}

	public static GameObject AddExistingChild(GameObject parent, GameObject go)
	{
		if (go != null && parent != null)
		{
			go.transform.SetParent(parent.transform, worldPositionStays: false);
			go.layer = parent.layer;
		}
		return go;
	}

	public static GameObject AddChild(GameObject parent, GameObject prefab)
	{
		return Util.KInstantiateUI(prefab, parent);
	}
}
