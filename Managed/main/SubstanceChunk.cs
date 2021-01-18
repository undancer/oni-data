using KSerialization;
using STRINGS;
using UnityEngine;

[SkipSaveFileSerialization]
[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/SubstanceChunk")]
public class SubstanceChunk : KMonoBehaviour, ISaveLoadable
{
	private static readonly KAnimHashedString symbolToTint = new KAnimHashedString("substance_tinter");

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Color color = GetComponent<PrimaryElement>().Element.substance.colour;
		color.a = 1f;
		GetComponent<KBatchedAnimController>().SetSymbolTint(symbolToTint, color);
	}

	private void OnRefreshUserMenu(object data)
	{
		Game.Instance.userMenu.AddButton(base.gameObject, new KIconButtonMenu.ButtonInfo("action_deconstruct", UI.USERMENUACTIONS.RELEASEELEMENT.NAME, OnRelease, Action.NumActions, null, null, null, UI.USERMENUACTIONS.RELEASEELEMENT.TOOLTIP));
	}

	private void OnRelease()
	{
		int gameCell = Grid.PosToCell(base.transform.GetPosition());
		PrimaryElement component = GetComponent<PrimaryElement>();
		if (component.Mass > 0f)
		{
			SimMessages.AddRemoveSubstance(gameCell, component.ElementID, CellEventLogger.Instance.ExhaustSimUpdate, component.Mass, component.Temperature, component.DiseaseIdx, component.DiseaseCount);
		}
		base.gameObject.DeleteObject();
	}
}
