using Klei.AI;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/AttackableBase")]
public class AttackableBase : Workable, IApproachable
{
	private HandleVector<int>.Handle scenePartitionerEntry;

	private static readonly EventSystem.IntraObjectHandler<AttackableBase> OnDeadTagAddedDelegate = GameUtil.CreateHasTagHandler(GameTags.Dead, delegate(AttackableBase component, object data)
	{
		component.OnDefeated(data);
	});

	private static readonly EventSystem.IntraObjectHandler<AttackableBase> OnDefeatedDelegate = new EventSystem.IntraObjectHandler<AttackableBase>(delegate(AttackableBase component, object data)
	{
		component.OnDefeated(data);
	});

	private static readonly EventSystem.IntraObjectHandler<AttackableBase> SetupScenePartitionerDelegate = new EventSystem.IntraObjectHandler<AttackableBase>(delegate(AttackableBase component, object data)
	{
		component.SetupScenePartitioner(data);
	});

	private static readonly EventSystem.IntraObjectHandler<AttackableBase> OnCellChangedDelegate = new EventSystem.IntraObjectHandler<AttackableBase>(delegate(AttackableBase component, object data)
	{
		GameScenePartitioner.Instance.UpdatePosition(component.scenePartitionerEntry, Grid.PosToCell(component.gameObject));
	});

	protected override void OnSpawn()
	{
		base.OnSpawn();
		attributeConverter = Db.Get().AttributeConverters.AttackDamage;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.BARELY_EVER_EXPERIENCE;
		skillExperienceSkillGroup = Db.Get().SkillGroups.Mining.Id;
		skillExperienceMultiplier = SKILLS.BARELY_EVER_EXPERIENCE;
		SetupScenePartitioner();
		Subscribe(1088554450, OnCellChangedDelegate);
		GameUtil.SubscribeToTags(this, OnDeadTagAddedDelegate, triggerImmediately: true);
		Subscribe(-1506500077, OnDefeatedDelegate);
		Subscribe(-1256572400, SetupScenePartitionerDelegate);
	}

	public float GetDamageMultiplier()
	{
		if (attributeConverter != null && base.worker != null)
		{
			AttributeConverterInstance converter = base.worker.GetComponent<AttributeConverters>().GetConverter(attributeConverter.Id);
			return Mathf.Max(1f + converter.Evaluate(), 0.1f);
		}
		return 1f;
	}

	private void SetupScenePartitioner(object data = null)
	{
		Extents extents = new Extents(Grid.PosToXY(base.transform.GetPosition()).x, Grid.PosToXY(base.transform.GetPosition()).y, 1, 1);
		scenePartitionerEntry = GameScenePartitioner.Instance.Add(base.gameObject.name, GetComponent<FactionAlignment>(), extents, GameScenePartitioner.Instance.attackableEntitiesLayer, null);
	}

	private void OnDefeated(object data = null)
	{
		GameScenePartitioner.Instance.Free(ref scenePartitionerEntry);
	}

	public override float GetEfficiencyMultiplier(Worker worker)
	{
		return 1f;
	}

	protected override void OnCleanUp()
	{
		Unsubscribe(1088554450, OnCellChangedDelegate);
		GameUtil.UnsubscribeToTags(this, OnDeadTagAddedDelegate);
		Unsubscribe(-1506500077, OnDefeatedDelegate);
		Unsubscribe(-1256572400, SetupScenePartitionerDelegate);
		GameScenePartitioner.Instance.Free(ref scenePartitionerEntry);
		base.OnCleanUp();
	}
}
