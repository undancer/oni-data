using System.Collections.Generic;
using System.IO;
using Klei;
using ProcGen;
using STRINGS;
using UnityEngine;

public class PasteBaseTemplateScreen : KScreen
{
	public static PasteBaseTemplateScreen Instance;

	public GameObject button_list_container;

	public GameObject prefab_paste_button;

	public GameObject prefab_directory_button;

	public KButton button_directory_up;

	public LocText directory_path_text;

	private List<GameObject> m_template_buttons = new List<GameObject>();

	private static readonly string NO_DIRECTORY = "NONE";

	private string m_CurrentDirectory = NO_DIRECTORY;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
		TemplateCache.Init();
		button_directory_up.onClick += UpDirectory;
		base.ConsumeMouseScroll = true;
		RefreshStampButtons();
	}

	protected override void OnForcedCleanUp()
	{
		Instance = null;
		base.OnForcedCleanUp();
	}

	[ContextMenu("Refresh")]
	public void RefreshStampButtons()
	{
		directory_path_text.text = m_CurrentDirectory;
		button_directory_up.isInteractable = m_CurrentDirectory != NO_DIRECTORY;
		foreach (GameObject template_button in m_template_buttons)
		{
			Object.Destroy(template_button);
		}
		m_template_buttons.Clear();
		Debug.Log("Changing directory to " + m_CurrentDirectory);
		if (m_CurrentDirectory == NO_DIRECTORY)
		{
			directory_path_text.text = "";
			{
				foreach (string dlcId in DlcManager.RELEASE_ORDER)
				{
					if (DlcManager.IsContentActive(dlcId))
					{
						GameObject gameObject = Util.KInstantiateUI(prefab_directory_button, button_list_container, force_active: true);
						gameObject.GetComponent<KButton>().onClick += delegate
						{
							UpdateDirectory(SettingsCache.GetScope(dlcId));
						};
						gameObject.GetComponentInChildren<LocText>().text = ((dlcId == "") ? UI.DEBUG_TOOLS.SAVE_BASE_TEMPLATE.BASE_GAME_FOLDER_NAME.text : SettingsCache.GetScope(dlcId));
						m_template_buttons.Add(gameObject);
					}
				}
				return;
			}
		}
		string[] directories = Directory.GetDirectories(TemplateCache.RewriteTemplatePath(m_CurrentDirectory));
		foreach (string path in directories)
		{
			string directory_name = System.IO.Path.GetFileNameWithoutExtension(path);
			GameObject gameObject2 = Util.KInstantiateUI(prefab_directory_button, button_list_container, force_active: true);
			gameObject2.GetComponent<KButton>().onClick += delegate
			{
				UpdateDirectory(directory_name);
			};
			gameObject2.GetComponentInChildren<LocText>().text = directory_name;
			m_template_buttons.Add(gameObject2);
		}
		ListPool<FileHandle, PasteBaseTemplateScreen>.PooledList pooledList = ListPool<FileHandle, PasteBaseTemplateScreen>.Allocate();
		FileSystem.GetFiles(TemplateCache.RewriteTemplatePath(m_CurrentDirectory), "*.yaml", pooledList);
		foreach (FileHandle item in pooledList)
		{
			string file_path_no_extension = System.IO.Path.GetFileNameWithoutExtension(item.full_path);
			GameObject gameObject3 = Util.KInstantiateUI(prefab_paste_button, button_list_container, force_active: true);
			gameObject3.GetComponent<KButton>().onClick += delegate
			{
				OnClickPasteButton(file_path_no_extension);
			};
			gameObject3.GetComponentInChildren<LocText>().text = file_path_no_extension;
			m_template_buttons.Add(gameObject3);
		}
	}

	private void UpdateDirectory(string relativePath)
	{
		if (m_CurrentDirectory == NO_DIRECTORY)
		{
			m_CurrentDirectory = "";
		}
		m_CurrentDirectory = FileSystem.CombineAndNormalize(m_CurrentDirectory, relativePath);
		RefreshStampButtons();
	}

	private void UpDirectory()
	{
		int num = m_CurrentDirectory.LastIndexOf("/");
		if (num > 0)
		{
			m_CurrentDirectory = m_CurrentDirectory.Substring(0, num);
		}
		else
		{
			SettingsCache.GetDlcIdAndPath(m_CurrentDirectory, out var dlcId, out var path);
			if (path.IsNullOrWhiteSpace())
			{
				m_CurrentDirectory = NO_DIRECTORY;
			}
			else
			{
				m_CurrentDirectory = SettingsCache.GetScope(dlcId);
			}
		}
		RefreshStampButtons();
	}

	private void OnClickPasteButton(string template_name)
	{
		if (template_name != null)
		{
			string text = FileSystem.CombineAndNormalize(m_CurrentDirectory, template_name);
			DebugTool.Instance.DeactivateTool();
			DebugBaseTemplateButton.Instance.ClearSelection();
			DebugBaseTemplateButton.Instance.nameField.text = text;
			TemplateContainer template = TemplateCache.GetTemplate(text);
			StampTool.Instance.Activate(template, SelectAffected: true);
		}
	}
}
