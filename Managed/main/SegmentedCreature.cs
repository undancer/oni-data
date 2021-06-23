using System.Collections.Generic;
using UnityEngine;

public class SegmentedCreature : GameStateMachine<SegmentedCreature, SegmentedCreature.Instance, IStateMachineTarget, SegmentedCreature.Def>
{
	public class Def : BaseDef
	{
		public HashedString segmentTrackerSymbol;

		public Vector3 headOffset = Vector3.zero;

		public Vector3 bodyPivot = Vector3.zero;

		public Vector3 tailPivot = Vector3.zero;

		public int numBodySegments;

		public float minSegmentSpacing;

		public float maxSegmentSpacing;

		public int numPathNodes;

		public float pathSpacing;

		public KAnimFile midAnim;

		public KAnimFile tailAnim;

		public string movingAnimName;

		public string idleAnimName;

		public float retractionSegmentSpeed = 1f;

		public float retractionPathSpeed = 1f;

		public float compressedMaxScale = 1.2f;

		public int animFrameOffset = 0;

		public HashSet<HashedString> retractWhenStartingAnimNames = new HashSet<HashedString>
		{
			"trapped",
			"trussed",
			"escape",
			"drown_pre",
			"drown_loop",
			"drown_pst"
		};

		public HashSet<HashedString> retractWhenEndingAnimNames = new HashSet<HashedString>
		{
			"floor_floor_2_0",
			"grooming_pst",
			"fall"
		};
	}

	public class RectractStates : State
	{
		public State pre;

		public State loop;
	}

	public class FreeMovementStates : State
	{
		public State idle;

		public State moving;

		public State layEgg;

		public State poop;

		public State dead;
	}

	public new class Instance : GameInstance
	{
		private const int NUM_CREATURE_SLOTS = 10;

		private static int creatureBatchSlot;

		public float baseAnimScale;

		public Vector3 previousHeadPosition;

		public float previousDist;

		public LinkedList<PathNode> path = new LinkedList<PathNode>();

		public LinkedList<CreatureSegment> segments = new LinkedList<CreatureSegment>();

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			Debug.Assert((float)def.numBodySegments * def.maxSegmentSpacing < (float)def.numPathNodes * def.pathSpacing);
			CreateSegments();
		}

		private void CreateSegments()
		{
			float layerZ = Grid.GetLayerZ(Grid.SceneLayer.Creatures);
			float num = layerZ + (float)creatureBatchSlot * 0.01f;
			creatureBatchSlot = (creatureBatchSlot + 1) % 10;
			CreatureSegment value = segments.AddFirst(new CreatureSegment(base.gameObject, num, base.smi.def.headOffset, Vector3.zero)).Value;
			base.gameObject.SetActive(value: false);
			value.animController = GetComponent<KBatchedAnimController>();
			value.animController.SetSymbolVisiblity(base.smi.def.segmentTrackerSymbol, is_visible: false);
			value.symbol = base.smi.def.segmentTrackerSymbol;
			value.SetPosition(base.transform.position);
			base.gameObject.SetActive(value: true);
			baseAnimScale = value.animController.animScale;
			value.animController.onAnimEnter += AnimEntered;
			value.animController.onAnimComplete += AnimComplete;
			for (int i = 0; i < base.def.numBodySegments; i++)
			{
				GameObject gameObject = new GameObject(base.gameObject.GetProperName() + $" Segment {i}");
				gameObject.SetActive(value: false);
				gameObject.transform.parent = base.transform;
				gameObject.transform.position = value.Position;
				KAnimFile kAnimFile = base.def.midAnim;
				Vector3 pivot = base.def.bodyPivot;
				if (i == base.def.numBodySegments - 1)
				{
					kAnimFile = base.def.tailAnim;
					pivot = base.def.tailPivot;
				}
				KBatchedAnimController kBatchedAnimController = gameObject.AddOrGet<KBatchedAnimController>();
				kBatchedAnimController.AnimFiles = new KAnimFile[1]
				{
					kAnimFile
				};
				kBatchedAnimController.isMovable = true;
				kBatchedAnimController.SetSymbolVisiblity(base.smi.def.segmentTrackerSymbol, is_visible: false);
				kBatchedAnimController.sceneLayer = value.animController.sceneLayer;
				CreatureSegment creatureSegment = new CreatureSegment(gameObject, num + (float)(i + 1) * 0.0001f, Vector3.zero, pivot);
				creatureSegment.animController = kBatchedAnimController;
				creatureSegment.symbol = base.smi.def.segmentTrackerSymbol;
				creatureSegment.distanceToPreviousSegment = base.smi.def.minSegmentSpacing;
				creatureSegment.animLink = new KAnimLink(value.animController, kBatchedAnimController);
				segments.AddLast(creatureSegment);
				gameObject.SetActive(value: true);
			}
			for (int j = 0; j < base.def.numPathNodes; j++)
			{
				path.AddLast(new PathNode(value.Position));
			}
		}

		public void AnimEntered(HashedString name)
		{
			if (base.smi.def.retractWhenStartingAnimNames.Contains(name))
			{
				base.smi.sm.isRetracted.Set(value: true, base.smi);
			}
			else
			{
				base.smi.sm.isRetracted.Set(value: false, base.smi);
			}
		}

		public void AnimComplete(HashedString name)
		{
			if (base.smi.def.retractWhenEndingAnimNames.Contains(name))
			{
				base.smi.sm.isRetracted.Set(value: true, base.smi);
			}
		}

		public LinkedListNode<CreatureSegment> GetHeadSegmentNode()
		{
			return base.smi.segments.First;
		}

		public LinkedListNode<CreatureSegment> GetFirstBodySegmentNode()
		{
			return base.smi.segments.First.Next;
		}

		public float LengthPercentage()
		{
			float num = 0f;
			for (LinkedListNode<CreatureSegment> linkedListNode = GetFirstBodySegmentNode(); linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				num += linkedListNode.Value.distanceToPreviousSegment;
			}
			float num2 = MinLength();
			float num3 = MaxLength();
			return Mathf.Clamp(num - num2, 0f, num3) / (num3 - num2);
		}

		public float MinLength()
		{
			return base.smi.def.minSegmentSpacing * (float)base.smi.def.numBodySegments;
		}

		public float MaxLength()
		{
			return base.smi.def.maxSegmentSpacing * (float)base.smi.def.numBodySegments;
		}

		protected override void OnCleanUp()
		{
			GetHeadSegmentNode().Value.animController.onAnimEnter -= AnimEntered;
			GetHeadSegmentNode().Value.animController.onAnimComplete -= AnimComplete;
			for (LinkedListNode<CreatureSegment> linkedListNode = GetFirstBodySegmentNode(); linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				linkedListNode.Value.CleanUp();
			}
		}
	}

	public class PathNode
	{
		public Vector3 position;

		public Quaternion rotation;

		public PathNode(Vector3 position)
		{
			this.position = position;
			rotation = Quaternion.identity;
		}
	}

	public class CreatureSegment
	{
		public KBatchedAnimController animController;

		public KAnimLink animLink;

		public float distanceToPreviousSegment;

		public HashedString symbol;

		public Vector3 offset;

		public Vector3 pivot;

		public float zRelativeOffset;

		private Transform m_transform;

		public Vector3 Position
		{
			get
			{
				Vector3 vector = offset;
				vector.x *= ((!animController.FlipX) ? 1 : (-1));
				if (vector != Vector3.zero)
				{
					vector = Rotation * vector;
				}
				if (symbol.IsValid)
				{
					bool symbolVisible;
					Vector4 column = animController.GetSymbolTransform(symbol, out symbolVisible).GetColumn(3);
					Vector3 a = column;
					a.z = zRelativeOffset;
					return a + vector;
				}
				return m_transform.position + vector;
			}
		}

		public Quaternion Rotation
		{
			get
			{
				if (symbol.IsValid)
				{
					bool symbolVisible;
					Vector3 toDirection = animController.GetSymbolLocalTransform(symbol, out symbolVisible).MultiplyVector(Vector3.right);
					if (!animController.FlipX)
					{
						toDirection.y *= -1f;
					}
					return Quaternion.FromToRotation(Vector3.right, toDirection);
				}
				return m_transform.rotation;
			}
		}

		public Vector3 Forward => Rotation * (animController.FlipX ? Vector3.left : Vector3.right);

		public Vector3 Up => Rotation * Vector3.up;

		public CreatureSegment(GameObject go, float zRelativeOffset, Vector3 offset, Vector3 pivot)
		{
			m_transform = go.transform;
			this.zRelativeOffset = zRelativeOffset;
			this.offset = offset;
			this.pivot = pivot;
			SetPosition(go.transform.position);
		}

		public void SetPosition(Vector3 value)
		{
			value.z = zRelativeOffset;
			m_transform.position = value;
		}

		public void SetRotation(Quaternion rotation)
		{
			m_transform.rotation = rotation;
		}

		public void CleanUp()
		{
			Object.Destroy(m_transform.gameObject);
		}
	}

	public RectractStates retracted;

	public FreeMovementStates freeMovement;

	private BoolParameter isRetracted;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = freeMovement.idle;
		root.Enter(SetRetractedPath);
		retracted.DefaultState(retracted.pre).Enter(delegate(Instance smi)
		{
			PlayBodySegmentsAnim(smi, "idle_loop", KAnim.PlayMode.Loop);
		}).Exit(SetRetractedPath);
		retracted.pre.Update(UpdateRetractedPre, UpdateRate.SIM_EVERY_TICK);
		retracted.loop.ParamTransition(isRetracted, freeMovement, (Instance smi, bool p) => !isRetracted.Get(smi)).Update(UpdateRetractedLoop, UpdateRate.SIM_EVERY_TICK);
		freeMovement.DefaultState(freeMovement.idle).ParamTransition(isRetracted, retracted, (Instance smi, bool p) => isRetracted.Get(smi)).Update(UpdateFreeMovement, UpdateRate.SIM_EVERY_TICK);
		freeMovement.idle.Transition(freeMovement.moving, (Instance smi) => smi.GetComponent<Navigator>().IsMoving()).Enter(delegate(Instance smi)
		{
			PlayBodySegmentsAnim(smi, "idle_loop", KAnim.PlayMode.Loop, queue: true);
		});
		freeMovement.moving.Transition(freeMovement.idle, (Instance smi) => !smi.GetComponent<Navigator>().IsMoving()).Enter(delegate(Instance smi)
		{
			PlayBodySegmentsAnim(smi, "walking_pre", KAnim.PlayMode.Once);
			PlayBodySegmentsAnim(smi, "walking_loop", KAnim.PlayMode.Loop, queue: false, smi.def.animFrameOffset);
		}).Exit(delegate(Instance smi)
		{
			PlayBodySegmentsAnim(smi, "walking_pst", KAnim.PlayMode.Once, queue: true);
		});
	}

	private void PlayBodySegmentsAnim(Instance smi, string animName, KAnim.PlayMode playMode, bool queue = false, int frameOffset = 0)
	{
		LinkedListNode<CreatureSegment> linkedListNode = smi.GetFirstBodySegmentNode();
		int num = 0;
		while (linkedListNode != null)
		{
			if (queue)
			{
				linkedListNode.Value.animController.Queue(animName, playMode);
			}
			else
			{
				linkedListNode.Value.animController.Play(animName, playMode);
			}
			if (frameOffset > 0)
			{
				float num2 = linkedListNode.Value.animController.GetCurrentNumFrames();
				float elapsedTime = (float)num * ((float)frameOffset / num2);
				linkedListNode.Value.animController.SetElapsedTime(elapsedTime);
			}
			num++;
			linkedListNode = linkedListNode.Next;
		}
	}

	private void UpdateRetractedPre(Instance smi, float dt)
	{
		UpdateHeadPosition(smi);
		bool flag = true;
		for (LinkedListNode<CreatureSegment> linkedListNode = smi.GetFirstBodySegmentNode(); linkedListNode != null; linkedListNode = linkedListNode.Next)
		{
			linkedListNode.Value.distanceToPreviousSegment = Mathf.Max(smi.def.minSegmentSpacing, linkedListNode.Value.distanceToPreviousSegment - dt * smi.def.retractionSegmentSpeed);
			if (linkedListNode.Value.distanceToPreviousSegment > smi.def.minSegmentSpacing)
			{
				flag = false;
			}
		}
		CreatureSegment value = smi.GetHeadSegmentNode().Value;
		LinkedListNode<PathNode> linkedListNode2 = smi.path.First;
		Vector3 forward = value.Forward;
		Quaternion rotation = value.Rotation;
		int num = 0;
		while (linkedListNode2 != null)
		{
			Vector3 b = value.Position - smi.def.pathSpacing * (float)num * forward;
			linkedListNode2.Value.position = Vector3.Lerp(linkedListNode2.Value.position, b, dt * smi.def.retractionPathSpeed);
			linkedListNode2.Value.rotation = Quaternion.Slerp(linkedListNode2.Value.rotation, rotation, dt * smi.def.retractionPathSpeed);
			num++;
			linkedListNode2 = linkedListNode2.Next;
		}
		UpdateBodyPosition(smi);
		if (flag)
		{
			smi.GoTo(retracted.loop);
		}
	}

	private void UpdateRetractedLoop(Instance smi, float dt)
	{
		UpdateHeadPosition(smi);
		SetRetractedPath(smi);
		UpdateBodyPosition(smi);
	}

	private void SetRetractedPath(Instance smi)
	{
		CreatureSegment value = smi.GetHeadSegmentNode().Value;
		LinkedListNode<PathNode> linkedListNode = smi.path.First;
		Vector3 position = value.Position;
		Quaternion rotation = value.Rotation;
		Vector3 forward = value.Forward;
		int num = 0;
		while (linkedListNode != null)
		{
			linkedListNode.Value.position = position - smi.def.pathSpacing * (float)num * forward;
			linkedListNode.Value.rotation = rotation;
			num++;
			linkedListNode = linkedListNode.Next;
		}
	}

	private void UpdateFreeMovement(Instance smi, float dt)
	{
		float spacing = UpdateHeadPosition(smi);
		AdjustBodySegmentsSpacing(smi, spacing);
		UpdateBodyPosition(smi);
	}

	private float UpdateHeadPosition(Instance smi)
	{
		CreatureSegment value = smi.GetHeadSegmentNode().Value;
		if (value.Position == smi.previousHeadPosition)
		{
			return 0f;
		}
		PathNode value2 = smi.path.First.Value;
		PathNode value3 = smi.path.First.Next.Value;
		float magnitude = (value2.position - value3.position).magnitude;
		float magnitude2 = (value.Position - value3.position).magnitude;
		float result = magnitude2 - magnitude;
		value2.position = value.Position;
		value2.rotation = value.Rotation;
		smi.previousHeadPosition = value2.position;
		Vector3 normalized = (value2.position - value3.position).normalized;
		int num = Mathf.FloorToInt(magnitude2 / smi.def.pathSpacing);
		for (int i = 0; i < num; i++)
		{
			Vector3 position = value3.position + normalized * smi.def.pathSpacing;
			LinkedListNode<PathNode> last = smi.path.Last;
			last.Value.position = position;
			last.Value.rotation = value2.rotation;
			float num2 = magnitude2 - (float)i * smi.def.pathSpacing;
			float t = num2 - smi.def.pathSpacing / num2;
			last.Value.rotation = Quaternion.Lerp(value2.rotation, value3.rotation, t);
			smi.path.RemoveLast();
			smi.path.AddAfter(smi.path.First, last);
			value3 = last.Value;
		}
		return result;
	}

	private void AdjustBodySegmentsSpacing(Instance smi, float spacing)
	{
		if (spacing == 0f)
		{
			return;
		}
		for (LinkedListNode<CreatureSegment> linkedListNode = smi.GetFirstBodySegmentNode(); linkedListNode != null; linkedListNode = linkedListNode.Next)
		{
			linkedListNode.Value.distanceToPreviousSegment += spacing;
			if (linkedListNode.Value.distanceToPreviousSegment < smi.def.minSegmentSpacing)
			{
				spacing = linkedListNode.Value.distanceToPreviousSegment - smi.def.minSegmentSpacing;
				linkedListNode.Value.distanceToPreviousSegment = smi.def.minSegmentSpacing;
			}
			else
			{
				if (!(linkedListNode.Value.distanceToPreviousSegment > smi.def.maxSegmentSpacing))
				{
					break;
				}
				spacing = linkedListNode.Value.distanceToPreviousSegment - smi.def.maxSegmentSpacing;
				linkedListNode.Value.distanceToPreviousSegment = smi.def.maxSegmentSpacing;
			}
		}
	}

	private void UpdateBodyPosition(Instance smi)
	{
		LinkedListNode<CreatureSegment> linkedListNode = smi.GetFirstBodySegmentNode();
		LinkedListNode<PathNode> linkedListNode2 = smi.path.First;
		float num = 0f;
		float num2 = smi.LengthPercentage();
		int num3 = 0;
		while (linkedListNode != null)
		{
			float num4 = linkedListNode.Value.distanceToPreviousSegment;
			float num5 = 0f;
			while (linkedListNode2.Next != null)
			{
				num5 = (linkedListNode2.Value.position - linkedListNode2.Next.Value.position).magnitude - num;
				if (num4 < num5)
				{
					break;
				}
				num4 -= num5;
				num = 0f;
				linkedListNode2 = linkedListNode2.Next;
			}
			if (linkedListNode2.Next == null)
			{
				linkedListNode.Value.SetPosition(linkedListNode2.Value.position);
				linkedListNode.Value.SetRotation(smi.path.Last.Value.rotation);
			}
			else
			{
				PathNode value = linkedListNode2.Value;
				PathNode value2 = linkedListNode2.Next.Value;
				linkedListNode.Value.SetPosition(linkedListNode2.Value.position + (linkedListNode2.Next.Value.position - linkedListNode2.Value.position).normalized * num4);
				linkedListNode.Value.SetRotation(Quaternion.Slerp(value.rotation, value2.rotation, num4 / num5));
				num = num4;
			}
			linkedListNode.Value.animController.FlipX = linkedListNode.Previous.Value.Position.x < linkedListNode.Value.Position.x;
			linkedListNode.Value.animController.animScale = smi.baseAnimScale + smi.baseAnimScale * smi.def.compressedMaxScale * ((float)(smi.def.numBodySegments - num3) / (float)smi.def.numBodySegments) * (1f - num2);
			linkedListNode = linkedListNode.Next;
			num3++;
		}
	}

	private void DrawDebug(Instance smi, float dt)
	{
		CreatureSegment value = smi.GetHeadSegmentNode().Value;
		DrawUtil.Arrow(value.Position, value.Position + value.Up, 0.05f, Color.red);
		DrawUtil.Arrow(value.Position, value.Position + value.Forward * 0.06f, 0.05f, Color.cyan);
		int num = 0;
		foreach (PathNode item in smi.path)
		{
			Color color = Color.HSVToRGB((float)num / (float)smi.def.numPathNodes, 1f, 1f);
			DrawUtil.Gnomon(item.position, 0.05f, Color.cyan);
			DrawUtil.Arrow(item.position, item.position + item.rotation * Vector3.up * 0.5f, 0.025f, color);
			num++;
		}
		for (LinkedListNode<CreatureSegment> linkedListNode = smi.segments.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
		{
			DrawUtil.Circle(linkedListNode.Value.Position, 0.05f, Color.white, Vector3.forward);
			DrawUtil.Gnomon(linkedListNode.Value.Position, 0.05f, Color.white);
		}
	}
}
