using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using KMod;
using Steamworks;
using STRINGS;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LanguageOptionsScreen : KModalScreen, SteamUGCService.IClient
{
	private static readonly string[] poFile = new string[1]
	{
		"strings.po"
	};

	public const string KPLAYER_PREFS_LANGUAGE_KEY = "InstalledLanguage";

	public const string TAG_LANGUAGE = "language";

	public KButton textButton;

	public KButton dismissButton;

	public KButton closeButton;

	public KButton workshopButton;

	public KButton uninstallButton;

	[Space]
	public GameObject languageButtonPrefab;

	public GameObject preinstalledLanguagesTitle;

	public GameObject preinstalledLanguagesContainer;

	public GameObject ugcLanguagesTitle;

	public GameObject ugcLanguagesContainer;

	private List<GameObject> buttons = new List<GameObject>();

	private string _currentLanguageModId;

	private System.DateTime currentLastModified;

	public string currentLanguageModId
	{
		get
		{
			return _currentLanguageModId;
		}
		private set
		{
			_currentLanguageModId = value;
			SetSavedLanguageMod(_currentLanguageModId);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		dismissButton.onClick += Deactivate;
		dismissButton.GetComponent<HierarchyReferences>().GetReference<LocText>("Title").SetText(UI.FRONTEND.OPTIONS_SCREEN.BACK);
		closeButton.onClick += Deactivate;
		workshopButton.onClick += delegate
		{
			OnClickOpenWorkshop();
		};
		uninstallButton.onClick += delegate
		{
			OnClickUninstall();
		};
		uninstallButton.gameObject.SetActive(value: false);
		RebuildScreen();
	}

	private void RebuildScreen()
	{
		foreach (GameObject button in buttons)
		{
			UnityEngine.Object.Destroy(button);
		}
		buttons.Clear();
		uninstallButton.isInteractable = KPlayerPrefs.GetString(Localization.SELECTED_LANGUAGE_TYPE_KEY, Localization.SelectedLanguageType.None.ToString()) != Localization.SelectedLanguageType.None.ToString();
		RebuildPreinstalledButtons();
		RebuildUGCButtons();
	}

	private void RebuildPreinstalledButtons()
	{
		foreach (string code in Localization.PreinstalledLanguages)
		{
			if (!(code != Localization.DEFAULT_LANGUAGE_CODE) || File.Exists(Localization.GetPreinstalledLocalizationFilePath(code)))
			{
				GameObject gameObject = Util.KInstantiateUI(languageButtonPrefab, preinstalledLanguagesContainer);
				gameObject.name = code + "_button";
				HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
				LocText reference = component.GetReference<LocText>("Title");
				reference.text = Localization.GetPreinstalledLocalizationTitle(code);
				reference.enabled = false;
				reference.enabled = true;
				Texture2D preinstalledLocalizationImage = Localization.GetPreinstalledLocalizationImage(code);
				if (preinstalledLocalizationImage != null)
				{
					component.GetReference<Image>("Image").sprite = Sprite.Create(preinstalledLocalizationImage, new Rect(Vector2.zero, new Vector2(preinstalledLocalizationImage.width, preinstalledLocalizationImage.height)), Vector2.one * 0.5f);
				}
				gameObject.GetComponent<KButton>().onClick += delegate
				{
					ConfirmLanguagePreinstalledOrMod((code != Localization.DEFAULT_LANGUAGE_CODE) ? code : string.Empty, null);
				};
				buttons.Add(gameObject);
			}
		}
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		Global.Instance.modManager.Sanitize(base.gameObject);
		if (SteamUGCService.Instance != null)
		{
			SteamUGCService.Instance.AddClient(this);
		}
	}

	protected override void OnDeactivate()
	{
		base.OnDeactivate();
		if (SteamUGCService.Instance != null)
		{
			SteamUGCService.Instance.RemoveClient(this);
		}
	}

	private void ConfirmLanguageChoiceDialog(string[] lines, bool is_template, System.Action install_language)
	{
		Localization.Locale locale = Localization.GetLocale(lines);
		Dictionary<string, string> translated_strings = Localization.ExtractTranslatedStrings(lines, is_template);
		TMP_FontAsset font = Localization.GetFont(locale.FontName);
		ConfirmDialogScreen screen = GetConfirmDialog();
		HashSet<MemberInfo> excluded_members = new HashSet<MemberInfo>(typeof(ConfirmDialogScreen).GetMember("cancelButton", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy));
		Localization.SetFont(screen, font, locale.IsRightToLeft, excluded_members);
		string value;
		Func<LocString, string> func = (LocString loc_string) => (!translated_strings.TryGetValue(loc_string.key.String, out value)) ? ((string)loc_string) : value;
		screen.PopupConfirmDialog(title_text: func(UI.CONFIRMDIALOG.DIALOG_HEADER), text: func(UI.FRONTEND.TRANSLATIONS_SCREEN.PLEASE_REBOOT), on_confirm: delegate
		{
			CleanUpSavedLanguageMod();
			install_language();
			App.instance.Restart();
		}, on_cancel: delegate
		{
			Localization.SetFont(screen, Localization.FontAsset, Localization.IsRightToLeft, excluded_members);
		}, configurable_text: null, on_configurable_clicked: null, confirm_text: func(UI.FRONTEND.TRANSLATIONS_SCREEN.RESTART), cancel_text: UI.FRONTEND.TRANSLATIONS_SCREEN.CANCEL);
	}

	private void ConfirmPreinstalledLanguage(string selected_preinstalled_translation)
	{
		Localization.GetSelectedLanguageType();
	}

	private void ConfirmLanguagePreinstalledOrMod(string selected_preinstalled_translation, string mod_id)
	{
		Localization.SelectedLanguageType selectedLanguageType = Localization.GetSelectedLanguageType();
		if (mod_id != null)
		{
			if (selectedLanguageType == Localization.SelectedLanguageType.UGC && mod_id == currentLanguageModId)
			{
				Deactivate();
				return;
			}
			string[] languageLinesForMod = GetLanguageLinesForMod(mod_id);
			ConfirmLanguageChoiceDialog(languageLinesForMod, is_template: false, delegate
			{
				SetCurrentLanguage(mod_id);
			});
		}
		else if (!string.IsNullOrEmpty(selected_preinstalled_translation))
		{
			string currentLanguageCode = Localization.GetCurrentLanguageCode();
			if (selectedLanguageType == Localization.SelectedLanguageType.Preinstalled && currentLanguageCode == selected_preinstalled_translation)
			{
				Deactivate();
				return;
			}
			string[] lines = File.ReadAllLines(Localization.GetPreinstalledLocalizationFilePath(selected_preinstalled_translation), Encoding.UTF8);
			ConfirmLanguageChoiceDialog(lines, is_template: false, delegate
			{
				Localization.LoadPreinstalledTranslation(selected_preinstalled_translation);
			});
		}
		else if (selectedLanguageType == Localization.SelectedLanguageType.None)
		{
			Deactivate();
		}
		else
		{
			string[] lines2 = File.ReadAllLines(Localization.GetDefaultLocalizationFilePath(), Encoding.UTF8);
			ConfirmLanguageChoiceDialog(lines2, is_template: true, delegate
			{
				Localization.ClearLanguage();
			});
		}
	}

	private ConfirmDialogScreen GetConfirmDialog()
	{
		KScreen component = KScreenManager.AddChild(base.transform.parent.gameObject, ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject).GetComponent<KScreen>();
		component.Activate();
		return component.GetComponent<ConfirmDialogScreen>();
	}

	private void RebuildUGCButtons()
	{
		foreach (Mod mod in Global.Instance.modManager.mods)
		{
			if ((mod.available_content & Content.Translation) != 0 && mod.status == Mod.Status.Installed)
			{
				GameObject gameObject = Util.KInstantiateUI(languageButtonPrefab, ugcLanguagesContainer);
				gameObject.name = mod.title + "_button";
				HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
				TMP_FontAsset font = Localization.GetFont(Localization.GetFontName(GetLanguageLinesForMod(mod)));
				LocText reference = component.GetReference<LocText>("Title");
				reference.SetText(string.Format(UI.FRONTEND.TRANSLATIONS_SCREEN.UGC_MOD_TITLE_FORMAT, mod.title));
				reference.font = font;
				Texture2D previewImage = mod.GetPreviewImage();
				if (previewImage != null)
				{
					component.GetReference<Image>("Image").sprite = Sprite.Create(previewImage, new Rect(Vector2.zero, new Vector2(previewImage.width, previewImage.height)), Vector2.one * 0.5f);
				}
				string mod_id = mod.label.id;
				gameObject.GetComponent<KButton>().onClick += delegate
				{
					ConfirmLanguagePreinstalledOrMod(string.Empty, mod_id);
				};
				buttons.Add(gameObject);
			}
		}
	}

	private void Uninstall()
	{
		GetConfirmDialog().PopupConfirmDialog(UI.FRONTEND.TRANSLATIONS_SCREEN.ARE_YOU_SURE, delegate
		{
			Localization.ClearLanguage();
			GetConfirmDialog().PopupConfirmDialog(UI.FRONTEND.TRANSLATIONS_SCREEN.PLEASE_REBOOT, App.instance.Restart, Deactivate);
		}, delegate
		{
		});
	}

	private void OnClickUninstall()
	{
		Uninstall();
	}

	private void OnClickOpenWorkshop()
	{
		Application.OpenURL("http://steamcommunity.com/workshop/browse/?appid=457140&requiredtags[]=language");
	}

	public void UpdateMods(IEnumerable<PublishedFileId_t> added, IEnumerable<PublishedFileId_t> updated, IEnumerable<PublishedFileId_t> removed, IEnumerable<SteamUGCService.Mod> loaded_previews)
	{
		string savedLanguageMod = GetSavedLanguageMod();
		if (ulong.TryParse(savedLanguageMod, out var result))
		{
			PublishedFileId_t value = (PublishedFileId_t)result;
			if (removed.Contains(value))
			{
				Debug.Log("Unsubscribe detected for currently installed language mod [" + savedLanguageMod + "]");
				GetConfirmDialog().PopupConfirmDialog(UI.FRONTEND.TRANSLATIONS_SCREEN.PLEASE_REBOOT, delegate
				{
					Localization.ClearLanguage();
					App.instance.Restart();
				}, null, null, null, null, UI.FRONTEND.TRANSLATIONS_SCREEN.RESTART);
			}
			if (updated.Contains(value))
			{
				Debug.Log("Download complete for currently installed language [" + savedLanguageMod + "] updating in background. Changes will happen next restart.");
			}
		}
		RebuildScreen();
	}

	public static string GetSavedLanguageMod()
	{
		return KPlayerPrefs.GetString("InstalledLanguage");
	}

	public static void SetSavedLanguageMod(string mod_id)
	{
		KPlayerPrefs.SetString("InstalledLanguage", mod_id);
	}

	public static void CleanUpSavedLanguageMod()
	{
		KPlayerPrefs.SetString("InstalledLanguage", null);
	}

	public static bool SetCurrentLanguage(string mod_id)
	{
		CleanUpSavedLanguageMod();
		if (LoadTranslation(mod_id))
		{
			SetSavedLanguageMod(mod_id);
			return true;
		}
		return false;
	}

	public static bool HasInstalledLanguage()
	{
		string currentModId = GetSavedLanguageMod();
		if (currentModId == null)
		{
			return false;
		}
		if (Global.Instance.modManager.mods.Find((Mod m) => m.label.id == currentModId) == null)
		{
			CleanUpSavedLanguageMod();
			return false;
		}
		return true;
	}

	public static string GetInstalledLanguageCode()
	{
		string result = "";
		string[] languageLinesForMod = GetLanguageLinesForMod(GetSavedLanguageMod());
		if (languageLinesForMod != null)
		{
			Localization.Locale locale = Localization.GetLocale(languageLinesForMod);
			if (locale != null)
			{
				result = locale.Code;
			}
		}
		return result;
	}

	private static bool LoadTranslation(string mod_id)
	{
		Mod mod = Global.Instance.modManager.mods.Find((Mod m) => m.label.id == mod_id);
		if (mod == null)
		{
			Debug.LogWarning("Tried loading a translation from a non-existent mod id: " + mod_id);
			return false;
		}
		string languageFilename = GetLanguageFilename(mod);
		if (languageFilename == null)
		{
			return false;
		}
		return Localization.LoadLocalTranslationFile(Localization.SelectedLanguageType.UGC, languageFilename);
	}

	private static string GetLanguageFilename(Mod mod)
	{
		Debug.Assert(mod.content_source.GetType() == typeof(KMod.Directory), "Can only load translations from extracted mods.");
		string text = Path.Combine(mod.ContentPath, "strings.po");
		if (!File.Exists(text))
		{
			Debug.LogWarning("GetLanguagFile: " + text + " missing for mod " + mod.label.title);
			return null;
		}
		return text;
	}

	private static string[] GetLanguageLinesForMod(string mod_id)
	{
		return GetLanguageLinesForMod(Global.Instance.modManager.mods.Find((Mod m) => m.label.id == mod_id));
	}

	private static string[] GetLanguageLinesForMod(Mod mod)
	{
		string languageFilename = GetLanguageFilename(mod);
		if (languageFilename == null)
		{
			return null;
		}
		string[] array = File.ReadAllLines(languageFilename, Encoding.UTF8);
		if (array == null || array.Length == 0)
		{
			Debug.LogWarning("Couldn't find any strings in the translation mod " + mod.label.title);
			return null;
		}
		return array;
	}
}
