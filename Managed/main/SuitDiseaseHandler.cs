using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SuitDiseaseHandler")]
public class SuitDiseaseHandler : KMonoBehaviour
{
	private static readonly EventSystem.IntraObjectHandler<SuitDiseaseHandler> OnEquippedDelegate = new EventSystem.IntraObjectHandler<SuitDiseaseHandler>(delegate(SuitDiseaseHandler component, object data)
	{
		component.OnEquipped(data);
	});

	private static readonly EventSystem.IntraObjectHandler<SuitDiseaseHandler> OnUnequippedDelegate = new EventSystem.IntraObjectHandler<SuitDiseaseHandler>(delegate(SuitDiseaseHandler component, object data)
	{
		component.OnUnequipped(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-1617557748, OnEquippedDelegate);
		Subscribe(-170173755, OnUnequippedDelegate);
	}

	private PrimaryElement GetPrimaryElement(object data)
	{
		Equipment equipment = (Equipment)data;
		MinionAssignablesProxy component = equipment.GetComponent<MinionAssignablesProxy>();
		GameObject targetGameObject = component.GetTargetGameObject();
		if ((bool)targetGameObject)
		{
			return targetGameObject.GetComponent<PrimaryElement>();
		}
		return null;
	}

	private void OnEquipped(object data)
	{
		PrimaryElement primaryElement = GetPrimaryElement(data);
		if (primaryElement != null)
		{
			primaryElement.ForcePermanentDiseaseContainer(force_on: true);
			primaryElement.RedirectDisease(base.gameObject);
		}
	}

	private void OnUnequipped(object data)
	{
		PrimaryElement primaryElement = GetPrimaryElement(data);
		if (primaryElement != null)
		{
			primaryElement.ForcePermanentDiseaseContainer(force_on: false);
			primaryElement.RedirectDisease(null);
		}
	}

	private void OnModifyDiseaseCount(int delta, string reason)
	{
		GetComponent<PrimaryElement>().ModifyDiseaseCount(delta, reason);
	}

	private void OnAddDisease(byte disease_idx, int delta, string reason)
	{
		GetComponent<PrimaryElement>().AddDisease(disease_idx, delta, reason);
	}
}
