using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/NewGameFlow")]
public class NewGameFlow : KMonoBehaviour
{
	public List<NewGameFlowScreen> newGameFlowScreens;

	private int currentScreenIndex = -1;

	private NewGameFlowScreen currentScreen;

	public void BeginFlow()
	{
		currentScreenIndex = -1;
		Next();
	}

	private void Next()
	{
		ClearCurrentScreen();
		currentScreenIndex++;
		ActivateCurrentScreen();
	}

	private void Previous()
	{
		ClearCurrentScreen();
		currentScreenIndex--;
		ActivateCurrentScreen();
	}

	private void ClearCurrentScreen()
	{
		if (currentScreen != null)
		{
			currentScreen.Deactivate();
			currentScreen = null;
		}
	}

	private void ActivateCurrentScreen()
	{
		if (currentScreenIndex >= 0 && currentScreenIndex < newGameFlowScreens.Count)
		{
			NewGameFlowScreen newGameFlowScreen = Util.KInstantiateUI<NewGameFlowScreen>(newGameFlowScreens[currentScreenIndex].gameObject, base.transform.parent.gameObject, force_active: true);
			newGameFlowScreen.OnNavigateForward += Next;
			newGameFlowScreen.OnNavigateBackward += Previous;
			if (!newGameFlowScreen.IsActive() && !newGameFlowScreen.activateOnSpawn)
			{
				newGameFlowScreen.Activate();
			}
			currentScreen = newGameFlowScreen;
		}
	}
}
