using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SkillPerkMissingComplainer")]
public class SkillPerkMissingComplainer : KMonoBehaviour
{
	public string requiredSkillPerk;

	private int skillUpdateHandle = -1;

	private Guid workStatusItemHandle;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (!string.IsNullOrEmpty(requiredSkillPerk))
		{
			skillUpdateHandle = Game.Instance.Subscribe(-1523247426, UpdateStatusItem);
		}
		UpdateStatusItem();
	}

	protected override void OnCleanUp()
	{
		if (skillUpdateHandle != -1)
		{
			Game.Instance.Unsubscribe(skillUpdateHandle);
		}
		base.OnCleanUp();
	}

	protected virtual void UpdateStatusItem(object data = null)
	{
		KSelectable component = GetComponent<KSelectable>();
		if (!(component == null) && !string.IsNullOrEmpty(requiredSkillPerk))
		{
			bool flag = MinionResume.AnyMinionHasPerk(requiredSkillPerk, this.GetMyWorldId());
			if (!flag && workStatusItemHandle == Guid.Empty)
			{
				workStatusItemHandle = component.AddStatusItem(Db.Get().BuildingStatusItems.ColonyLacksRequiredSkillPerk, requiredSkillPerk);
			}
			else if (flag && workStatusItemHandle != Guid.Empty)
			{
				component.RemoveStatusItem(workStatusItemHandle);
				workStatusItemHandle = Guid.Empty;
			}
		}
	}
}
