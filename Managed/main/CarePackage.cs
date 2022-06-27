using System.Collections.Generic;
using KSerialization;
using UnityEngine;

public class CarePackage : StateMachineComponent<CarePackage.SMInstance>
{
	public class SMInstance : GameStateMachine<States, SMInstance, CarePackage, object>.GameInstance
	{
		public List<Chore> activeUseChores;

		public SMInstance(CarePackage master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, SMInstance, CarePackage>
	{
		public BoolParameter spawnedContents;

		public State spawn;

		public State open;

		public State pst;

		public State destroy;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = spawn;
			base.serializable = SerializeType.ParamsOnly;
			spawn.PlayAnim("portalbirth").OnAnimQueueComplete(open).ParamTransition(spawnedContents, pst, GameStateMachine<States, SMInstance, CarePackage, object>.IsTrue);
			open.PlayAnim("portalbirth_pst").QueueAnim("object_idle_loop").Exit(delegate(SMInstance smi)
			{
				smi.master.SpawnContents();
				spawnedContents.Set(value: true, smi);
			})
				.ScheduleGoTo(1f, pst);
			pst.PlayAnim("object_idle_pst").ScheduleGoTo(5f, destroy);
			destroy.Enter(delegate(SMInstance smi)
			{
				Util.KDestroyGameObject(smi.master.gameObject);
			});
		}
	}

	[Serialize]
	public CarePackageInfo info;

	private string facadeID;

	private Reactable reactable;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		if (info != null)
		{
			SetAnimToInfo();
		}
		reactable = CreateReactable();
	}

	public Reactable CreateReactable()
	{
		return new EmoteReactable(base.gameObject, "UpgradeFX", Db.Get().ChoreTypes.Emote, "anim_cheer_kanim").AddStep(new EmoteReactable.EmoteStep
		{
			anim = "cheer_pre"
		}).AddStep(new EmoteReactable.EmoteStep
		{
			anim = "cheer_loop"
		}).AddStep(new EmoteReactable.EmoteStep
		{
			anim = "cheer_pst"
		});
	}

	protected override void OnCleanUp()
	{
		reactable.Cleanup();
		base.OnCleanUp();
	}

	public void SetInfo(CarePackageInfo info)
	{
		this.info = info;
		SetAnimToInfo();
	}

	public void SetFacade(string facadeID)
	{
		this.facadeID = facadeID;
		SetAnimToInfo();
	}

	private void SetAnimToInfo()
	{
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab("Meter".ToTag()), base.gameObject);
		GameObject prefab = Assets.GetPrefab(info.id);
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		KBatchedAnimController component2 = prefab.GetComponent<KBatchedAnimController>();
		SymbolOverrideController component3 = prefab.GetComponent<SymbolOverrideController>();
		KBatchedAnimController component4 = gameObject.GetComponent<KBatchedAnimController>();
		component4.transform.SetLocalPosition(Vector3.forward);
		component4.AnimFiles = component2.AnimFiles;
		component4.isMovable = true;
		component4.animWidth = component2.animWidth;
		component4.animHeight = component2.animHeight;
		if (component3 != null)
		{
			SymbolOverrideController symbolOverrideController = SymbolOverrideControllerUtil.AddToPrefab(gameObject);
			SymbolOverrideController.SymbolEntry[] getSymbolOverrides = component3.GetSymbolOverrides;
			for (int i = 0; i < getSymbolOverrides.Length; i++)
			{
				SymbolOverrideController.SymbolEntry symbolEntry = getSymbolOverrides[i];
				symbolOverrideController.AddSymbolOverride(symbolEntry.targetSymbol, symbolEntry.sourceSymbol);
			}
		}
		component4.initialAnim = component2.initialAnim;
		component4.initialMode = KAnim.PlayMode.Loop;
		if (!string.IsNullOrEmpty(facadeID))
		{
			component4.SwapAnims(new KAnimFile[1] { Db.Get().EquippableFacades.Get(facadeID).AnimFile });
			GetComponentsInChildren<KBatchedAnimController>()[1].SetSymbolVisiblity("object", is_visible: false);
		}
		KBatchedAnimTracker component5 = gameObject.GetComponent<KBatchedAnimTracker>();
		component5.controller = component;
		component5.symbol = new HashedString("snapTO_object");
		component5.offset = new Vector3(0f, 0.5f, 0f);
		gameObject.SetActive(value: true);
		component.SetSymbolVisiblity("snapTO_object", is_visible: false);
		new KAnimLink(component, component4);
	}

	private void SpawnContents()
	{
		if (info == null)
		{
			Debug.LogWarning("CarePackage has no data to spawn from. Probably a save from before the CarePackage info data was serialized.");
			return;
		}
		GameObject gameObject = null;
		GameObject prefab = Assets.GetPrefab(info.id);
		Element element = null;
		element = ElementLoader.GetElement(info.id.ToTag());
		Vector3 position = base.transform.position + Vector3.up / 2f;
		if (element == null && prefab != null)
		{
			for (int i = 0; (float)i < info.quantity; i++)
			{
				gameObject = Util.KInstantiate(prefab, position);
				if (gameObject != null)
				{
					if (!facadeID.IsNullOrWhiteSpace())
					{
						EquippableFacade.AddFacadeToEquippable(gameObject.GetComponent<Equippable>(), facadeID);
					}
					gameObject.SetActive(value: true);
				}
			}
		}
		else if (element != null)
		{
			float quantity = info.quantity;
			gameObject = element.substance.SpawnResource(position, quantity, element.defaultValues.temperature, byte.MaxValue, 0, prevent_merge: false, forceTemperature: true);
		}
		else
		{
			Debug.LogWarning("Can't find spawnable thing from tag " + info.id);
		}
		if (gameObject != null)
		{
			gameObject.SetActive(value: true);
		}
	}
}
