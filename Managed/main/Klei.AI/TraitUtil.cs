using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

namespace Klei.AI
{
	public class TraitUtil
	{
		public static System.Action CreateDisabledTaskTrait(string id, string name, string desc, string disabled_chore_group, bool is_valid_starter_trait)
		{
			return delegate
			{
				ChoreGroup[] disabled_chore_groups = new ChoreGroup[1]
				{
					Db.Get().ChoreGroups.Get(disabled_chore_group)
				};
				Db.Get().CreateTrait(id, name, desc, null, should_save: true, disabled_chore_groups, positive_trait: false, is_valid_starter_trait);
			};
		}

		public static System.Action CreateTrait(string id, string name, string desc, string attributeId, float delta, string[] chore_groups, bool positiveTrait = false)
		{
			return delegate
			{
				List<ChoreGroup> list = new List<ChoreGroup>();
				string[] array = chore_groups;
				foreach (string id2 in array)
				{
					list.Add(Db.Get().ChoreGroups.Get(id2));
				}
				Trait trait = Db.Get().CreateTrait(id, name, desc, null, should_save: true, list.ToArray(), positiveTrait, is_valid_starter_trait: true);
				trait.Add(new AttributeModifier(attributeId, delta, name));
			};
		}

		public static System.Action CreateAttributeEffectTrait(string id, string name, string desc, string attributeId, float delta, string attributeId2, float delta2, bool positiveTrait = false)
		{
			return delegate
			{
				Trait trait = Db.Get().CreateTrait(id, name, desc, null, should_save: true, null, positiveTrait, is_valid_starter_trait: true);
				trait.Add(new AttributeModifier(attributeId, delta, name));
				trait.Add(new AttributeModifier(attributeId2, delta2, name));
			};
		}

		public static System.Action CreateAttributeEffectTrait(string id, string name, string desc, string attributeId, float delta, bool positiveTrait = false, Action<GameObject> on_add = null, bool is_valid_starter_trait = true)
		{
			return delegate
			{
				Trait trait = Db.Get().CreateTrait(id, name, desc, null, should_save: true, null, positiveTrait, is_valid_starter_trait);
				trait.Add(new AttributeModifier(attributeId, delta, name));
				trait.OnAddTrait = on_add;
			};
		}

		public static System.Action CreateEffectModifierTrait(string id, string name, string desc, string[] ignoredEffects, bool positiveTrait = false)
		{
			return delegate
			{
				Trait trait = Db.Get().CreateTrait(id, name, desc, null, should_save: true, null, positiveTrait, is_valid_starter_trait: true);
				trait.AddIgnoredEffects(ignoredEffects);
			};
		}

		public static System.Action CreateNamedTrait(string id, string name, string desc, bool positiveTrait = false)
		{
			return delegate
			{
				Db.Get().CreateTrait(id, name, desc, null, should_save: true, null, positiveTrait, is_valid_starter_trait: true);
			};
		}

		public static System.Action CreateTrait(string id, string name, string desc, Action<GameObject> on_add, ChoreGroup[] disabled_chore_groups = null, bool positiveTrait = false, Func<string> extendedDescFn = null)
		{
			return delegate
			{
				Trait trait = Db.Get().CreateTrait(id, name, desc, null, should_save: true, disabled_chore_groups, positiveTrait, is_valid_starter_trait: true);
				trait.OnAddTrait = on_add;
				if (extendedDescFn != null)
				{
					trait.ExtendedTooltip = (Func<string>)Delegate.Combine(trait.ExtendedTooltip, extendedDescFn);
				}
			};
		}

		public static System.Action CreateComponentTrait<T>(string id, string name, string desc, bool positiveTrait = false, Func<string> extendedDescFn = null) where T : KMonoBehaviour
		{
			return delegate
			{
				Trait trait = Db.Get().CreateTrait(id, name, desc, null, should_save: true, null, positiveTrait, is_valid_starter_trait: true);
				trait.OnAddTrait = delegate(GameObject go)
				{
					go.FindOrAddUnityComponent<T>();
				};
				if (extendedDescFn != null)
				{
					trait.ExtendedTooltip = (Func<string>)Delegate.Combine(trait.ExtendedTooltip, extendedDescFn);
				}
			};
		}

		public static System.Action CreateSkillGrantingTrait(string id, string name, string desc, string skillId)
		{
			return delegate
			{
				Trait trait = Db.Get().CreateTrait(id, name, desc, null, should_save: true, null, positive_trait: true, is_valid_starter_trait: true);
				trait.TooltipCB = () => string.Format(DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_DESC, desc, SkillWidget.SkillPerksString(Db.Get().Skills.Get(skillId)));
				trait.OnAddTrait = delegate(GameObject go)
				{
					MinionResume component = go.GetComponent<MinionResume>();
					if (component != null)
					{
						component.GrantSkill(skillId);
					}
				};
			};
		}

		public static string GetSkillGrantingTraitNameById(string id)
		{
			string result = "";
			if (Strings.TryGet("STRINGS.DUPLICANTS.TRAITS.GRANTSKILL_" + id.ToUpper() + ".NAME", out var result2))
			{
				result = result2.String;
			}
			return result;
		}
	}
}
