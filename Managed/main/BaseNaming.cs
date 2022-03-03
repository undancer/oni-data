using System;
using System.IO;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/BaseNaming")]
public class BaseNaming : KMonoBehaviour
{
	[SerializeField]
	private KInputTextField inputField;

	[SerializeField]
	private KButton shuffleBaseNameButton;

	private MinionSelectScreen minionSelectScreen;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		GenerateBaseName();
		shuffleBaseNameButton.onClick += GenerateBaseName;
		inputField.onEndEdit.AddListener(OnEndEdit);
		inputField.onValueChanged.AddListener(OnEditing);
		minionSelectScreen = GetComponent<MinionSelectScreen>();
	}

	private bool CheckBaseName(string newName)
	{
		if (string.IsNullOrEmpty(newName))
		{
			return true;
		}
		string savePrefixAndCreateFolder = SaveLoader.GetSavePrefixAndCreateFolder();
		string cloudSavePrefix = SaveLoader.GetCloudSavePrefix();
		if (minionSelectScreen != null)
		{
			bool flag = false;
			try
			{
				bool num = Directory.Exists(Path.Combine(savePrefixAndCreateFolder, newName));
				bool flag2 = cloudSavePrefix != null && Directory.Exists(Path.Combine(cloudSavePrefix, newName));
				flag = num || flag2;
			}
			catch (Exception arg)
			{
				flag = true;
				Debug.Log($"Base Naming / Warning / {arg}");
			}
			if (flag)
			{
				minionSelectScreen.SetProceedButtonActive(state: false, string.Format(UI.IMMIGRANTSCREEN.DUPLICATE_COLONY_NAME, newName));
				return false;
			}
			minionSelectScreen.SetProceedButtonActive(state: true);
		}
		return true;
	}

	private void OnEditing(string newName)
	{
		Util.ScrubInputField(inputField);
		CheckBaseName(inputField.text);
	}

	private void OnEndEdit(string newName)
	{
		if (Localization.HasDirtyWords(newName))
		{
			inputField.text = GenerateBaseNameString();
			newName = inputField.text;
		}
		if (!string.IsNullOrEmpty(newName) && CheckBaseName(newName))
		{
			inputField.text = newName;
			SaveGame.Instance.SetBaseName(newName);
			string path = Path.ChangeExtension(newName, ".sav");
			string savePrefixAndCreateFolder = SaveLoader.GetSavePrefixAndCreateFolder();
			string cloudSavePrefix = SaveLoader.GetCloudSavePrefix();
			string path2 = savePrefixAndCreateFolder;
			if (SaveLoader.GetCloudSavesAvailable() && Game.Instance.SaveToCloudActive && cloudSavePrefix != null)
			{
				path2 = cloudSavePrefix;
			}
			SaveLoader.SetActiveSaveFilePath(Path.Combine(path2, newName, path));
		}
	}

	private void GenerateBaseName()
	{
		string text = GenerateBaseNameString();
		((LocText)inputField.placeholder).text = text;
		inputField.text = text;
		OnEndEdit(text);
	}

	private string GenerateBaseNameString()
	{
		string random = LocString.GetStrings(typeof(NAMEGEN.COLONY.FORMATS)).GetRandom();
		random = ReplaceStringWithRandom(random, "{noun}", LocString.GetStrings(typeof(NAMEGEN.COLONY.NOUN)));
		string[] strings = LocString.GetStrings(typeof(NAMEGEN.COLONY.ADJECTIVE));
		random = ReplaceStringWithRandom(random, "{adjective}", strings);
		random = ReplaceStringWithRandom(random, "{adjective2}", strings);
		random = ReplaceStringWithRandom(random, "{adjective3}", strings);
		return ReplaceStringWithRandom(random, "{adjective4}", strings);
	}

	private string ReplaceStringWithRandom(string fullString, string replacementKey, string[] replacementValues)
	{
		if (!fullString.Contains(replacementKey))
		{
			return fullString;
		}
		return fullString.Replace(replacementKey, replacementValues.GetRandom());
	}
}
