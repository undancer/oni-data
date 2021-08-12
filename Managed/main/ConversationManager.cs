using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ConversationManager")]
public class ConversationManager : KMonoBehaviour, ISim200ms
{
	public class Tuning : TuningData<Tuning>
	{
		public float cyclesBeforeFirstConversation;

		public float maxDistance;

		public int maxDupesPerConvo;

		public float minionCooldownTime;

		public float speakTime;

		public float delayBetweenUtterances;

		public float delayBeforeStart;

		public int maxUtterances;
	}

	public class StartedTalkingEvent
	{
		public GameObject talker;

		public string anim;
	}

	private List<Conversation> activeSetups;

	private Dictionary<MinionIdentity, float> lastConvoTimeByMinion;

	private Dictionary<MinionIdentity, Conversation> setupsByMinion = new Dictionary<MinionIdentity, Conversation>();

	private List<Type> convoTypes = new List<Type>
	{
		typeof(RecentThingConversation),
		typeof(AmountStateConversation),
		typeof(CurrentJobConversation)
	};

	private static readonly List<Tag> invalidConvoTags = new List<Tag>
	{
		GameTags.Asleep,
		GameTags.HoldingBreath,
		GameTags.Dead
	};

	protected override void OnPrefabInit()
	{
		activeSetups = new List<Conversation>();
		lastConvoTimeByMinion = new Dictionary<MinionIdentity, float>();
	}

	public void Sim200ms(float dt)
	{
		for (int num = activeSetups.Count - 1; num >= 0; num--)
		{
			Conversation conversation = activeSetups[num];
			for (int num2 = conversation.minions.Count - 1; num2 >= 0; num2--)
			{
				if (!ValidMinionTags(conversation.minions[num2]) || !MinionCloseEnoughToConvo(conversation.minions[num2], conversation))
				{
					conversation.minions.RemoveAt(num2);
				}
				else
				{
					setupsByMinion[conversation.minions[num2]] = conversation;
				}
			}
			if (conversation.minions.Count <= 1)
			{
				activeSetups.RemoveAt(num);
			}
			else
			{
				bool flag = true;
				bool flag2 = conversation.minions.Find((MinionIdentity match) => !match.HasTag(GameTags.Partying)) == null;
				if ((conversation.numUtterances == 0 && flag2 && GameClock.Instance.GetTime() > conversation.lastTalkedTime) || GameClock.Instance.GetTime() > conversation.lastTalkedTime + TuningData<Tuning>.Get().delayBeforeStart)
				{
					MinionIdentity minionIdentity = conversation.minions[UnityEngine.Random.Range(0, conversation.minions.Count)];
					conversation.conversationType.NewTarget(minionIdentity);
					flag = DoTalking(conversation, minionIdentity);
				}
				else if (conversation.numUtterances > 0 && conversation.numUtterances < TuningData<Tuning>.Get().maxUtterances && ((flag2 && GameClock.Instance.GetTime() > conversation.lastTalkedTime + TuningData<Tuning>.Get().speakTime / 4f) || GameClock.Instance.GetTime() > conversation.lastTalkedTime + TuningData<Tuning>.Get().speakTime + TuningData<Tuning>.Get().delayBetweenUtterances))
				{
					int index = (conversation.minions.IndexOf(conversation.lastTalked) + UnityEngine.Random.Range(1, conversation.minions.Count)) % conversation.minions.Count;
					MinionIdentity new_speaker = conversation.minions[index];
					flag = DoTalking(conversation, new_speaker);
				}
				else if (conversation.numUtterances >= TuningData<Tuning>.Get().maxUtterances)
				{
					flag = false;
				}
				if (!flag)
				{
					activeSetups.RemoveAt(num);
				}
			}
		}
		foreach (MinionIdentity item in Components.LiveMinionIdentities.Items)
		{
			if (!ValidMinionTags(item) || setupsByMinion.ContainsKey(item) || MinionOnCooldown(item))
			{
				continue;
			}
			foreach (MinionIdentity item2 in Components.LiveMinionIdentities.Items)
			{
				if (item2 == item || !ValidMinionTags(item2))
				{
					continue;
				}
				if (setupsByMinion.ContainsKey(item2))
				{
					Conversation conversation2 = setupsByMinion[item2];
					if (conversation2.minions.Count < TuningData<Tuning>.Get().maxDupesPerConvo && (GetCentroid(conversation2) - item.transform.GetPosition()).magnitude < TuningData<Tuning>.Get().maxDistance * 0.5f)
					{
						conversation2.minions.Add(item);
						setupsByMinion[item] = conversation2;
						break;
					}
				}
				else if (!MinionOnCooldown(item2) && (item2.transform.GetPosition() - item.transform.GetPosition()).magnitude < TuningData<Tuning>.Get().maxDistance)
				{
					Conversation conversation3 = new Conversation();
					conversation3.minions.Add(item);
					conversation3.minions.Add(item2);
					Type type = convoTypes[UnityEngine.Random.Range(0, convoTypes.Count)];
					conversation3.conversationType = (ConversationType)Activator.CreateInstance(type);
					conversation3.lastTalkedTime = GameClock.Instance.GetTime();
					activeSetups.Add(conversation3);
					setupsByMinion[item] = conversation3;
					setupsByMinion[item2] = conversation3;
					break;
				}
			}
		}
		setupsByMinion.Clear();
	}

	private bool DoTalking(Conversation setup, MinionIdentity new_speaker)
	{
		DebugUtil.Assert(setup != null, "setup was null");
		DebugUtil.Assert(new_speaker != null, "new_speaker was null");
		if (setup.lastTalked != null)
		{
			setup.lastTalked.Trigger(25860745, setup.lastTalked.gameObject);
		}
		DebugUtil.Assert(setup.conversationType != null, "setup.conversationType was null");
		Conversation.Topic nextTopic = setup.conversationType.GetNextTopic(new_speaker, setup.lastTopic);
		if (nextTopic == null || nextTopic.mode == Conversation.ModeType.End || nextTopic.mode == Conversation.ModeType.Segue)
		{
			return false;
		}
		Thought thoughtForTopic = GetThoughtForTopic(setup, nextTopic);
		if (thoughtForTopic == null)
		{
			return false;
		}
		ThoughtGraph.Instance sMI = new_speaker.GetSMI<ThoughtGraph.Instance>();
		if (sMI == null)
		{
			return false;
		}
		sMI.AddThought(thoughtForTopic);
		setup.lastTopic = nextTopic;
		setup.lastTalked = new_speaker;
		setup.lastTalkedTime = GameClock.Instance.GetTime();
		DebugUtil.Assert(lastConvoTimeByMinion != null, "lastConvoTimeByMinion was null");
		lastConvoTimeByMinion[setup.lastTalked] = GameClock.Instance.GetTime();
		Effects component = setup.lastTalked.GetComponent<Effects>();
		DebugUtil.Assert(component != null, "effects was null");
		component.Add("GoodConversation", should_save: true);
		Conversation.Mode mode = Conversation.Topic.Modes[(int)nextTopic.mode];
		DebugUtil.Assert(mode != null, "mode was null");
		StartedTalkingEvent data = new StartedTalkingEvent
		{
			talker = new_speaker.gameObject,
			anim = mode.anim
		};
		foreach (MinionIdentity minion in setup.minions)
		{
			if (!minion)
			{
				DebugUtil.DevAssert(test: false, "minion in setup.minions was null");
			}
			else
			{
				minion.Trigger(-594200555, data);
			}
		}
		setup.numUtterances++;
		return true;
	}

	private Vector3 GetCentroid(Conversation setup)
	{
		Vector3 zero = Vector3.zero;
		foreach (MinionIdentity minion in setup.minions)
		{
			if (!(minion == null))
			{
				zero += minion.transform.GetPosition();
			}
		}
		return zero / setup.minions.Count;
	}

	private Thought GetThoughtForTopic(Conversation setup, Conversation.Topic topic)
	{
		if (string.IsNullOrEmpty(topic.topic))
		{
			DebugUtil.DevAssert(test: false, "topic.topic was null");
			return null;
		}
		Sprite sprite = setup.conversationType.GetSprite(topic.topic);
		if (sprite != null)
		{
			Conversation.Mode mode = Conversation.Topic.Modes[(int)topic.mode];
			return new Thought("Topic_" + topic.topic, null, sprite, mode.icon, mode.voice, "bubble_chatter", mode.mouth, DUPLICANTS.THOUGHTS.CONVERSATION.TOOLTIP, show_immediately: true, TuningData<Tuning>.Get().speakTime);
		}
		return null;
	}

	private bool ValidMinionTags(MinionIdentity minion)
	{
		if (minion == null)
		{
			return false;
		}
		return !minion.GetComponent<KPrefabID>().HasAnyTags(invalidConvoTags);
	}

	private bool MinionCloseEnoughToConvo(MinionIdentity minion, Conversation setup)
	{
		return (GetCentroid(setup) - minion.transform.GetPosition()).magnitude < TuningData<Tuning>.Get().maxDistance * 0.5f;
	}

	private bool MinionOnCooldown(MinionIdentity minion)
	{
		if (!minion.GetComponent<KPrefabID>().HasTag(GameTags.AlwaysConverse))
		{
			if (!lastConvoTimeByMinion.ContainsKey(minion) || !(GameClock.Instance.GetTime() < lastConvoTimeByMinion[minion] + TuningData<Tuning>.Get().minionCooldownTime))
			{
				return GameClock.Instance.GetTime() / 600f < TuningData<Tuning>.Get().cyclesBeforeFirstConversation;
			}
			return true;
		}
		return false;
	}
}
