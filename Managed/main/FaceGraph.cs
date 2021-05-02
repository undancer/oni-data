using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/FaceGraph")]
public class FaceGraph : KMonoBehaviour
{
	private List<Expression> expressions = new List<Expression>();

	[MyCmpGet]
	private KBatchedAnimController m_controller;

	[MyCmpGet]
	private Accessorizer m_accessorizer;

	[MyCmpGet]
	private SymbolOverrideController m_symbolOverrideController;

	private BlinkMonitor.Instance m_blinkMonitor;

	private SpeechMonitor.Instance m_speechMonitor;

	private static HashedString HASH_HEAD_MASTER_SWAP_KANIM = "head_master_swap_kanim";

	private static KAnimHashedString ANIM_HASH_SNAPTO_EYES = "snapto_eyes";

	private static KAnimHashedString ANIM_HASH_SNAPTO_MOUTH = "snapto_mouth";

	private static KAnimHashedString ANIM_HASH_NEUTRAL = "neutral";

	private static int FIRST_SIDEWAYS_FRAME = 29;

	public Expression overrideExpression
	{
		get;
		private set;
	}

	public Expression currentExpression
	{
		get;
		private set;
	}

	public IEnumerator<Expression> GetEnumerator()
	{
		return expressions.GetEnumerator();
	}

	public void AddExpression(Expression expression)
	{
		if (!expressions.Contains(expression))
		{
			expressions.Add(expression);
			UpdateFace();
		}
	}

	public void RemoveExpression(Expression expression)
	{
		if (expressions.Remove(expression))
		{
			UpdateFace();
		}
	}

	public void SetOverrideExpression(Expression expression)
	{
		if (expression != overrideExpression)
		{
			overrideExpression = expression;
			UpdateFace();
		}
	}

	public void ApplyShape()
	{
		KAnimFile anim = Assets.GetAnim(HASH_HEAD_MASTER_SWAP_KANIM);
		bool should_use_sideways_symbol = ShouldUseSidewaysSymbol(m_controller);
		if (m_blinkMonitor == null)
		{
			m_blinkMonitor = m_accessorizer.GetSMI<BlinkMonitor.Instance>();
		}
		if (m_speechMonitor == null)
		{
			m_speechMonitor = m_accessorizer.GetSMI<SpeechMonitor.Instance>();
		}
		if (m_blinkMonitor.IsNullOrStopped() || !m_blinkMonitor.IsBlinking())
		{
			Accessory accessory = m_accessorizer.GetAccessory(Db.Get().AccessorySlots.Eyes);
			KAnim.Build.Symbol symbol = accessory.symbol;
			ApplyShape(symbol, m_controller, anim, ANIM_HASH_SNAPTO_EYES, should_use_sideways_symbol);
		}
		if (m_speechMonitor.IsNullOrStopped() || !m_speechMonitor.IsPlayingSpeech())
		{
			Accessory accessory2 = m_accessorizer.GetAccessory(Db.Get().AccessorySlots.Mouth);
			KAnim.Build.Symbol symbol2 = accessory2.symbol;
			ApplyShape(symbol2, m_controller, anim, ANIM_HASH_SNAPTO_MOUTH, should_use_sideways_symbol);
		}
		else
		{
			m_speechMonitor.DrawMouth();
		}
	}

	private bool ShouldUseSidewaysSymbol(KBatchedAnimController controller)
	{
		KAnim.Anim currentAnim = controller.GetCurrentAnim();
		if (currentAnim == null)
		{
			return false;
		}
		int currentFrameIndex = controller.GetCurrentFrameIndex();
		if (currentFrameIndex <= 0)
		{
			return false;
		}
		KBatchGroupData batchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(currentAnim.animFile.animBatchTag);
		KAnim.Anim.Frame frame = batchGroupData.GetFrame(currentFrameIndex);
		for (int i = 0; i < frame.numElements; i++)
		{
			KAnim.Anim.FrameElement frameElement = batchGroupData.GetFrameElement(frame.firstElementIdx + i);
			if (frameElement.symbol == ANIM_HASH_SNAPTO_EYES && frameElement.frame >= FIRST_SIDEWAYS_FRAME)
			{
				return true;
			}
		}
		return false;
	}

	private void ApplyShape(KAnim.Build.Symbol variation_symbol, KBatchedAnimController controller, KAnimFile shapes_file, KAnimHashedString symbol_name_in_shape_file, bool should_use_sideways_symbol)
	{
		HashedString hashedString = ANIM_HASH_NEUTRAL;
		if (currentExpression != null)
		{
			hashedString = currentExpression.face.hash;
		}
		KAnim.Anim anim = null;
		KAnim.Anim.FrameElement frameElement = default(KAnim.Anim.FrameElement);
		bool flag = false;
		bool flag2 = false;
		for (int i = 0; i < shapes_file.GetData().animCount; i++)
		{
			if (flag)
			{
				break;
			}
			KAnim.Anim anim2 = shapes_file.GetData().GetAnim(i);
			if (!(anim2.hash == hashedString))
			{
				continue;
			}
			anim = anim2;
			KAnim.Anim.Frame frame = anim.GetFrame(shapes_file.GetData().build.batchTag, 0);
			for (int j = 0; j < frame.numElements; j++)
			{
				KBatchGroupData batchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(shapes_file.GetData().animBatchTag);
				frameElement = batchGroupData.GetFrameElement(frame.firstElementIdx + j);
				if (!(frameElement.symbol != symbol_name_in_shape_file))
				{
					if (flag2 || !should_use_sideways_symbol)
					{
						flag = true;
					}
					flag2 = true;
					break;
				}
			}
		}
		if (anim == null)
		{
			DebugUtil.Assert(test: false, "Could not find shape for expression: " + HashCache.Get().Get(hashedString));
		}
		if (!flag2)
		{
			DebugUtil.Assert(test: false, "Could not find shape element for shape:" + HashCache.Get().Get(variation_symbol.hash));
		}
		KAnim.Build.Symbol symbol = KAnimBatchManager.Instance().GetBatchGroupData(controller.batchGroupID).GetSymbol(symbol_name_in_shape_file);
		KBatchGroupData batchGroupData2 = KAnimBatchManager.Instance().GetBatchGroupData(variation_symbol.build.batchTag);
		KAnim.Build.SymbolFrameInstance symbol_frame_instance = batchGroupData2.symbolFrameInstances[variation_symbol.firstFrameIdx + frameElement.frame];
		symbol_frame_instance.buildImageIdx = m_symbolOverrideController.GetAtlasIdx(variation_symbol.build.GetTexture(0));
		controller.SetSymbolOverride(symbol.firstFrameIdx, ref symbol_frame_instance);
	}

	private void UpdateFace()
	{
		Expression expression = null;
		if (overrideExpression != null)
		{
			expression = overrideExpression;
		}
		else if (expressions.Count > 0)
		{
			expressions.Sort((Expression a, Expression b) => b.priority.CompareTo(a.priority));
			expression = expressions[0];
		}
		if (expression != currentExpression || expression == null)
		{
			currentExpression = expression;
			m_symbolOverrideController.MarkDirty();
		}
		AccessorySlot headEffects = Db.Get().AccessorySlots.HeadEffects;
		if (currentExpression != null)
		{
			Accessory accessory = m_accessorizer.GetAccessory(Db.Get().AccessorySlots.HeadEffects);
			HashedString hashedString = HashedString.Invalid;
			foreach (Expression expression2 in expressions)
			{
				if (expression2.face.headFXHash.IsValid)
				{
					hashedString = expression2.face.headFXHash;
					break;
				}
			}
			Accessory accessory2 = ((hashedString != HashedString.Invalid) ? headEffects.Lookup(hashedString) : null);
			if (accessory != accessory2)
			{
				if (accessory != null)
				{
					m_accessorizer.RemoveAccessory(accessory);
				}
				if (accessory2 != null)
				{
					m_accessorizer.AddAccessory(accessory2);
				}
			}
			m_controller.SetSymbolVisiblity(headEffects.targetSymbolId, accessory2 != null);
		}
		else
		{
			m_controller.SetSymbolVisiblity(headEffects.targetSymbolId, is_visible: false);
		}
	}
}
