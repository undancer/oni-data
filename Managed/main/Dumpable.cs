using KSerialization;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/Dumpable")]
public class Dumpable : Workable
{
	private Chore chore;

	[Serialize]
	private bool isMarkedForDumping;

	private static readonly EventSystem.IntraObjectHandler<Dumpable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Dumpable>(delegate(Dumpable component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(493375141, OnRefreshUserMenuDelegate);
		workerStatusItem = Db.Get().DuplicantStatusItems.Emptying;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (isMarkedForDumping)
		{
			CreateChore();
		}
		SetWorkTime(0.1f);
	}

	public void ToggleDumping()
	{
		if (DebugHandler.InstantBuildMode)
		{
			OnCompleteWork(null);
		}
		else if (isMarkedForDumping)
		{
			isMarkedForDumping = false;
			chore.Cancel("Cancel Dumping!");
			Prioritizable.RemoveRef(base.gameObject);
			chore = null;
			ShowProgressBar(show: false);
		}
		else
		{
			isMarkedForDumping = true;
			CreateChore();
		}
	}

	private void CreateChore()
	{
		if (chore == null)
		{
			Prioritizable.AddRef(base.gameObject);
			chore = new WorkChore<Dumpable>(Db.Get().ChoreTypes.EmptyStorage, this);
		}
	}

	protected override void OnCompleteWork(Worker worker)
	{
		isMarkedForDumping = false;
		chore = null;
		Dump();
		Prioritizable.RemoveRef(base.gameObject);
	}

	public void Dump()
	{
		Dump(base.transform.GetPosition());
	}

	public void Dump(Vector3 pos)
	{
		PrimaryElement component = GetComponent<PrimaryElement>();
		if (component.Mass > 0f)
		{
			if (component.Element.IsLiquid)
			{
				FallingWater.instance.AddParticle(Grid.PosToCell(pos), component.Element.idx, component.Mass, component.Temperature, component.DiseaseIdx, component.DiseaseCount, skip_sound: true);
			}
			else
			{
				SimMessages.AddRemoveSubstance(Grid.PosToCell(pos), component.ElementID, CellEventLogger.Instance.Dumpable, component.Mass, component.Temperature, component.DiseaseIdx, component.DiseaseCount);
			}
		}
		Util.KDestroyGameObject(base.gameObject);
	}

	private void OnRefreshUserMenu(object data)
	{
		if (!this.HasTag(GameTags.Stored))
		{
			KIconButtonMenu.ButtonInfo button = (isMarkedForDumping ? new KIconButtonMenu.ButtonInfo("action_empty_contents", UI.USERMENUACTIONS.DUMP.NAME_OFF, ToggleDumping, Action.NumActions, null, null, null, UI.USERMENUACTIONS.DUMP.TOOLTIP_OFF) : new KIconButtonMenu.ButtonInfo("action_empty_contents", UI.USERMENUACTIONS.DUMP.NAME, ToggleDumping, Action.NumActions, null, null, null, UI.USERMENUACTIONS.DUMP.TOOLTIP));
			Game.Instance.userMenu.AddButton(base.gameObject, button);
		}
	}
}
