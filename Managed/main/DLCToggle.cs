using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class DLCToggle : KMonoBehaviour
{
	[SerializeField]
	private KButton button;

	[SerializeField]
	private LocText label;

	[SerializeField]
	private Image logo;

	private bool expansion1Active;

	protected override void OnPrefabInit()
	{
		expansion1Active = DistributionPlatform.Inst.IsExpansion1Active;
		button.onClick += ToggleExpansion1Cicked;
		label.text = (expansion1Active ? UI.FRONTEND.MAINMENU.DLC.DEACTIVATE_EXPANSION1 : UI.FRONTEND.MAINMENU.DLC.ACTIVATE_EXPANSION1);
		logo.sprite = (expansion1Active ? GlobalResources.Instance().baseGameLogoSmall : GlobalResources.Instance().expansion1LogoSmall);
	}

	private void ToggleExpansion1Cicked()
	{
		Util.KInstantiateUI<InfoDialogScreen>(ScreenPrefabs.Instance.InfoDialogScreen.gameObject, GetComponentInParent<Canvas>().gameObject, force_active: true).AddDefaultCancel().SetHeader(expansion1Active ? UI.FRONTEND.MAINMENU.DLC.DEACTIVATE_EXPANSION1 : UI.FRONTEND.MAINMENU.DLC.ACTIVATE_EXPANSION1)
			.AddSprite(expansion1Active ? GlobalResources.Instance().baseGameLogoSmall : GlobalResources.Instance().expansion1LogoSmall)
			.AddPlainText(expansion1Active ? UI.FRONTEND.MAINMENU.DLC.DEACTIVATE_EXPANSION1_DESC : UI.FRONTEND.MAINMENU.DLC.ACTIVATE_EXPANSION1_DESC)
			.AddOption(UI.CONFIRMDIALOG.OK, delegate
			{
				KPlayerPrefs.SetInt("EXPANSION1_ID.ENABLED", 1);
				DistributionPlatform.Inst.ToggleDLC();
			}, rightSide: true);
	}
}
