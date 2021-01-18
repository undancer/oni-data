using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ScreenPrefabs")]
public class ScreenPrefabs : KMonoBehaviour
{
	public ControlsScreen ControlsScreen;

	public Hud HudScreen;

	public HoverTextScreen HoverTextScreen;

	public OverlayScreen OverlayScreen;

	public TileScreen TileScreen;

	public SpeedControlScreen SpeedControlScreen;

	public ManagementMenu ManagementMenu;

	public ToolTipScreen ToolTipScreen;

	public DebugPaintElementScreen DebugPaintElementScreen;

	public UserMenuScreen UserMenuScreen;

	public KButtonMenu OwnerScreen;

	public EnergyInfoScreen EnergyInfoScreen;

	public KButtonMenu ButtonGrid;

	public NameDisplayScreen NameDisplayScreen;

	public ConfirmDialogScreen ConfirmDialogScreen;

	public CustomizableDialogScreen CustomizableDialogScreen;

	public SpriteListDialogScreen SpriteListDialogScreen;

	public InfoDialogScreen InfoDialogScreen;

	public StoryMessageScreen StoryMessageScreen;

	public FileNameDialog FileNameDialog;

	public TagFilterScreen TagFilterScreen;

	public ResearchScreen ResearchScreen;

	public MessageDialogFrame MessageDialogFrame;

	public ResourceCategoryScreen ResourceCategoryScreen;

	public LanguageOptionsScreen languageOptionsScreen;

	public ModsScreen modsMenu;

	public GameObject GameOverScreen;

	public GameObject VictoryScreen;

	public GameObject StatusItemIndicatorScreen;

	public GameObject CollapsableContentPanel;

	public GameObject DescriptionLabel;

	public LoadingOverlay loadingOverlay;

	public LoadScreen LoadScreen;

	public InspectSaveScreen InspectSaveScreen;

	public OptionsMenuScreen OptionsScreen;

	public WorldGenScreen WorldGenScreen;

	public ModeSelectScreen ModeSelectScreen;

	public NewGameSettingsScreen NewGameSettingsScreen;

	public ColonyDestinationSelectScreen ColonyDestinationSelectScreen;

	public RetiredColonyInfoScreen RetiredColonyInfoScreen;

	public VideoScreen VideoScreen;

	public ComicViewer ComicViewer;

	public static ScreenPrefabs Instance
	{
		get;
		private set;
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
	}

	public void ConfirmDoAction(string message, System.Action action, Transform parent)
	{
		((ConfirmDialogScreen)KScreenManager.Instance.StartScreen(Instance.ConfirmDialogScreen.gameObject, parent.gameObject)).PopupConfirmDialog(message, action, delegate
		{
		});
	}
}
